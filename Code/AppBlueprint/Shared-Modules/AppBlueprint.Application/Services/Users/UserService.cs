using AppBlueprint.Domain.Entities.User;
using AppBlueprint.Domain.Interfaces.Repositories;
using AppBlueprint.Domain.Interfaces.Services;
using AppBlueprint.Domain.Interfaces.UnitOfWork;
using System.Security.Cryptography;

namespace AppBlueprint.Application.Services.Users;

public class UserService : IUserService
{
    private const string EmailFromAddress = "noreply@saas-factory.com";
    private const string SiteName = "SaaS Factory";
    private static readonly TimeSpan EmailTokenValidity = TimeSpan.FromHours(24);
    private static readonly TimeSpan ResetTokenValidity = TimeSpan.FromHours(1);

    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationRepository _emailVerificationRepository;
    private readonly IPasswordResetRepository _passwordResetRepository;
    private readonly IEmailService _emailService;

    public UserService(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IEmailVerificationRepository emailVerificationRepository,
        IPasswordResetRepository passwordResetRepository,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _emailVerificationRepository = emailVerificationRepository ?? throw new ArgumentNullException(nameof(emailVerificationRepository));
        _passwordResetRepository = passwordResetRepository ?? throw new ArgumentNullException(nameof(passwordResetRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task<UserEntity> RegisterAsync(string firstName, string lastName, string email, string userName, CancellationToken cancellationToken)
    {
        // Validate email doesn't already exist
        UserEntity? existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("Email is already registered");
        }

        // Create new user
        UserEntity user = new()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = userName,
            IsActive = true,
            Profile = new ProfileEntity
            {
                CreatedAt = DateTime.UtcNow
            }
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<UserEntity?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByEmailAsync(email, cancellationToken);
    }

    public async Task UpdateProfileAsync(string userId, string firstName, string lastName, string? phoneNumber, string? bio, CancellationToken cancellationToken)
    {
        UserEntity? user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found");

        user.FirstName = firstName;
        user.LastName = lastName;

        if (user.Profile is not null)
        {
            user.Profile.PhoneNumber = phoneNumber;
            user.Profile.Bio = bio;
            user.Profile.LastUpdatedAt = DateTime.UtcNow;
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateUserAsync(string userId, CancellationToken cancellationToken)
    {
        UserEntity? user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found");

        user.IsActive = false;
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> GenerateEmailVerificationTokenAsync(string userId, CancellationToken cancellationToken)
    {
        UserEntity? user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found");

        // Generate a secure random token
        byte[] tokenBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        string token = Convert.ToBase64String(tokenBytes)
            .Replace("/", "_", StringComparison.Ordinal)
            .Replace("+", "-", StringComparison.Ordinal)
            .Replace("=", "", StringComparison.Ordinal);

        // Create email verification record
        var emailVerification = new EmailVerificationEntity
        {
            Email = user.Email,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(EmailTokenValidity),
            HasBeenOpened = false,
            HasBeenVerified = false,
            UserId = userId,
            User = user
        };

        // Save verification record
        await _emailVerificationRepository.AddAsync(emailVerification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // In a real-world implementation, we would send an email with the verification link
        // For now, we'll just return the token to the caller
        try
        {
            await _emailService.SendSignUpWelcomeEmail(
                EmailFromAddress,
                user.Email,
                SiteName,
                cancellationToken
            );
        }
        catch (InvalidOperationException)
        {
            // Log the error but don't fail the operation
            // A proper implementation would use a logger
        }

        return token;
    }

    public async Task<bool> VerifyEmailAsync(string userId, string token, CancellationToken cancellationToken)
    {
        _ = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found");

        // Find the verification record
        EmailVerificationEntity? verification = await _emailVerificationRepository
            .GetByUserIdAndTokenAsync(userId, token, cancellationToken);

        if (verification is null || verification.ExpiresAt <= DateTime.UtcNow)
        {
            return false; // Invalid or expired token
        }

        // Mark as verified
        verification.HasBeenVerified = true;
        verification.HasBeenOpened = true;
        verification.LastUpdatedAt = DateTime.UtcNow;

        await _emailVerificationRepository.UpdateAsync(verification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<string> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(email);

        UserEntity? user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            // For security reasons, don't reveal that the email doesn't exist
            // Just return a dummy token that won't work
            return Guid.NewGuid().ToString();
        }

        // Generate a secure random token
        byte[] tokenBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        string token = Convert.ToBase64String(tokenBytes)
            .Replace("/", "_", StringComparison.Ordinal)
            .Replace("+", "-", StringComparison.Ordinal)
            .Replace("=", "", StringComparison.Ordinal);

        // Create password reset record
        var passwordReset = new PasswordResetEntity
        {
            Token = token,
            ExpireAt = DateTime.UtcNow.Add(ResetTokenValidity),
            IsUsed = false,
            User = user,
            UserId = user.Id,
            TenantId = user.TenantId
        };

        // Save password reset record
        await _passwordResetRepository.AddAsync(passwordReset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // In a real-world implementation, we would send an email with the reset link
        // Since we're not implementing the actual email sending, we'll just return the token
        try
        {
            // Use transaction email service to send reset email
            // This would typically include the token in a reset link
            await _emailService.SendSignUpWelcomeEmail(
                EmailFromAddress,
                user.Email,
                "Password Reset",
                cancellationToken
            );
        }
        catch (InvalidOperationException)
        {
            // Log the error but don't fail the operation
            // A proper implementation would use a logger
            // For now, we'll silently continue
        }

        return token;
    }

    public async Task<bool> CompletePasswordResetAsync(string email, string token, string newPassword, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(token);
        ArgumentNullException.ThrowIfNull(newPassword);

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
        {
            return false;
        }

        UserEntity? user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            return false;
        }

        // Find the reset record
        PasswordResetEntity? resetRecord = await _passwordResetRepository
            .GetByUserIdAndTokenAsync(user.Id, token, cancellationToken);

        if (resetRecord is null || resetRecord.ExpireAt <= DateTime.UtcNow || resetRecord.IsUsed)
        {
            return false; // Invalid, expired, or already used token
        }

        // Mark token as used
        resetRecord.IsUsed = true;
        resetRecord.LastUpdatedAt = DateTime.UtcNow;

        await _passwordResetRepository.UpdateAsync(resetRecord, cancellationToken);

        // We're not implementing actual password hashing as per instructions
        // In a real application, the password would be hashed here
        // For this exercise, we're assuming the authentication is handled externally by Supabase

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
