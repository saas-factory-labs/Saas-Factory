using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AppBlueprint.Infrastructure.Services.Users;

public class UserServiceInfrastructure
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _dbContext;
    private readonly TransactionEmailService _emailService;

    private const string UserNotFoundMessage = "User not found";
    private const string EmailAlreadyRegisteredMessage = "Email is already registered";
    private const string EmailFromAddress = "noreply@saas-factory.com";
    private const string EmailSiteName = "SaaS Factory";

    private static readonly TimeSpan EmailTokenValidity = TimeSpan.FromHours(24);
    private static readonly TimeSpan ResetTokenValidity = TimeSpan.FromHours(1);

    private static string ToUrlToken(byte[] bytes) =>
        Convert.ToBase64String(bytes)
            .Replace("/", "_", StringComparison.Ordinal)
            .Replace("+", "-", StringComparison.Ordinal)
            .Replace("=", "", StringComparison.Ordinal);

    public UserServiceInfrastructure(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        ApplicationDbContext dbContext,
        TransactionEmailService emailService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task<UserEntity> RegisterAsync(string firstName, string lastName, string email, string userName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(userName);

        // Validate email doesn't already exist
        UserEntity? existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser is not null)
        {
            throw new InvalidOperationException(EmailAlreadyRegisteredMessage);
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

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user;
    }
    public async Task<UserEntity?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(email);

        return await _userRepository.GetByEmailAsync(email);
    }
    public async Task UpdateProfileAsync(string userId, string firstName, string lastName, string? phoneNumber, string? bio, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);

        UserEntity user = await _userRepository.GetByIdAsync(userId)
            ?? throw new InvalidOperationException(UserNotFoundMessage);

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Profile.PhoneNumber = phoneNumber;
        user.Profile.Bio = bio;
        user.Profile.LastUpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    public async Task DeactivateUserAsync(string userId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userId);

        UserEntity? user = await _userRepository.GetByIdAsync(userId)
            ?? throw new InvalidOperationException(UserNotFoundMessage);

        user.IsActive = false;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    public async Task<string> GenerateEmailVerificationTokenAsync(string userId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userId);

        UserEntity? user = await _userRepository.GetByIdAsync(userId)
            ?? throw new InvalidOperationException(UserNotFoundMessage);

        // Generate a secure random token
        byte[] tokenBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        string token = ToUrlToken(tokenBytes);

        // Create email verification record
        var emailVerification = new EmailVerificationEntity
        {
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.Add(EmailTokenValidity),
            HasBeenOpened = false,
            HasBeenVerified = false,
            UserEntityId = userId,
            User = user
        };

        // Save verification record
        await _dbContext.Set<EmailVerificationEntity>().AddAsync(emailVerification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // In a real-world implementation, we would send an email with the verification link
        // For now, we'll just return the token to the caller
        try
        {
            await _emailService.SendSignUpWelcomeEmail(
                EmailFromAddress,
                user.Email!,
                EmailSiteName
            );
        }
        catch (InvalidOperationException ex)
        {
            // Log the error but don't fail the operation
            // A proper implementation would use a logger
            Console.WriteLine($"Failed to send verification email: {ex.Message}");
        }

        return token;
    }
    public async Task<bool> VerifyEmailAsync(string userId, string token, CancellationToken cancellationToken)
    {
        // Verify user exists
        _ = await _userRepository.GetByIdAsync(userId)
            ?? throw new InvalidOperationException(UserNotFoundMessage);

        // Find the verification record
        EmailVerificationEntity? verification = await _dbContext.Set<EmailVerificationEntity>()
            .FirstOrDefaultAsync(v =>
                v.UserEntityId == userId &&
                v.Token == token &&
                v.ExpireAt > DateTime.UtcNow,
                cancellationToken);

        if (verification is null)
        {
            return false; // Invalid or expired token
        }

        // Mark as verified
        verification.HasBeenVerified = true;
        verification.HasBeenOpened = true;
        verification.LastUpdatedAt = DateTime.UtcNow;

        // Update user's verification status if needed
        // Typically, your UserEntity would have a property like 'IsEmailVerified'
        // But we don't see this in the current model, so we'll just update the verification record

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<string> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(email);

        UserEntity? user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            // For security reasons, don't reveal that the email doesn't exist
            // Just return a dummy token that won't work
            return Guid.NewGuid().ToString();
        }        // Generate a secure random token
        byte[] tokenBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        string token = ToUrlToken(tokenBytes);
        // Create password reset record
        var passwordReset = new PasswordResetEntity
        {
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.Add(ResetTokenValidity),
            IsUsed = false,
            User = user,
            UserId = user.Id,
            TenantId = user.TenantId
        };

        // Save password reset record
        await _dbContext.Set<PasswordResetEntity>().AddAsync(passwordReset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // In a real-world implementation, we would send an email with the reset link
        // Since we're not implementing the actual email sending, we'll just return the token
        try
        {
            await _emailService.SendSignUpWelcomeEmail(
                EmailFromAddress,
                user.Email!,
                EmailSiteName
            );
        }
        catch (InvalidOperationException ex)
        {
            // Log the error but don't fail the operation
            Console.WriteLine($"Failed to send password reset email: {ex.Message}");
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

        UserEntity? user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            return false;
        }        // Find the reset record
        PasswordResetEntity? resetRecord = await _dbContext.Set<PasswordResetEntity>()
            .FirstOrDefaultAsync(r =>
                r.UserId == user.Id &&
                r.Token == token &&
                r.ExpireAt > DateTime.UtcNow &&
                !r.IsUsed,
                cancellationToken);

        if (resetRecord is null)
        {
            return false; // Invalid, expired, or already used token
        }

        // Mark token as used
        resetRecord.IsUsed = true;
        resetRecord.LastUpdatedAt = DateTime.UtcNow;

        // We're not implementing actual password hashing as per instructions
        // In a real application, the password would be hashed here
        // For this exercise, we're assuming the authentication is handled externally by an authentication provider

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}