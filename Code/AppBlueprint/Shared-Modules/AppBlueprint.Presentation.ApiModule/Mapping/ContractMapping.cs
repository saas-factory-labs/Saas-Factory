using AppBlueprint.Contracts.Baseline.AuditLog.Requests;
using AppBlueprint.Contracts.Baseline.AuditLog.Responses;
using AppBlueprint.Contracts.Baseline.ContactPerson.Requests;
using AppBlueprint.Contracts.Baseline.ContactPerson.Responses;
using AppBlueprint.Contracts.Baseline.DataExport.Requests;
using AppBlueprint.Contracts.Baseline.DataExport.Responses;
using AppBlueprint.Contracts.Baseline.File.Requests;
using AppBlueprint.Contracts.Baseline.File.Responses;
using AppBlueprint.Contracts.Baseline.Integrations.Requests;
using AppBlueprint.Contracts.Baseline.Integrations.Responses;
using AppBlueprint.Contracts.Baseline.Notification.Requests;
using AppBlueprint.Contracts.Baseline.Notification.Responses;
using AppBlueprint.Contracts.Baseline.Profile.Requests;
using AppBlueprint.Contracts.Baseline.Profile.Responses;
using AppBlueprint.Contracts.Baseline.Role.Requests;
using AppBlueprint.Contracts.Baseline.Role.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AccountResponse = AppBlueprint.Contracts.Baseline.Account.Responses.AccountResponse;
using CreateAccountRequest = AppBlueprint.Contracts.Baseline.Account.Requests.CreateAccountRequest;
using AppBlueprint.Contracts.Baseline.EmailAddress.Responses;
using AppBlueprint.Contracts.Baseline.Address.Responses;
using AppBlueprint.Contracts.Baseline.PhoneNumber.Responses;
using AppBlueprint.Contracts.Baseline.Permissions.Responses;

namespace AppBlueprint.Presentation.ApiModule.Mapping
{
    // Logical grouping without containing extension methods
    public static class ContractMapping
    {
        public static AccountResponse MapToAccountResponse(this AccountEntity account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            return new AccountResponse
            {
                Id = account.Id,
                Slug = account.Slug,
                Email = account.Email
            };
        }

        public static class Accounts
        {
        }

        public static class ContactPersons
        {
        }

        public static class DataExports
        {
        }

        public static class Files
        {
        }

        public static class AuditLogs
        {
        }

        public static class Notifications
        {
        }

        public static class Profiles
        {
        }

        public static class Roles
        {
        }

        public static class Integrations
        {
        }

        public static class Organizations
        {
        }
    }
}

// ✅ Extension methods must be in a top-level static class
namespace AppBlueprint.Presentation.ApiModule.Mapping.Extensions
{
    public static class AccountsExtensions
    {        public static AccountEntity MapToAccount(this CreateAccountRequest request, string tenantId)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return new AccountEntity
            {
                Name = request.Name,
                TenantId = tenantId,
                Owner = new UserEntity
                {
                    Email = string.Empty,
                    UserName = string.Empty,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Profile = new ProfileEntity
                    {
                        // DisplayName = string.Empty,
                        // Bio = string.Empty,
                        // AvatarUrl = string.Empty,
                        // PhoneNumber = string.Empty,
                        // Address = string.Empty,
                        // City = string.Empty,
                        // State = string.Empty,
                        // Country = string.Empty,
                        // ZipCode = string.Empty,
                        // IsVerified = false
                    }
                },
                Email = request.Email
            };
        }

        public static AccountResponse MapToAccountResponse(this AccountEntity request)
        {
            return new AccountResponse
            {
                Id = request.Id,
                Slug = request.Slug,
                Email = request.Email
            };
        }
    }

    public static class ContactPersonsExtensions
    {
        public static ContactPersonResponse MapToContactPerson(this CreateContactPersonRequest request)
        {
            return new ContactPersonResponse(
                request.FirstName,
                request.LastName,
                new List<EmailAddressResponse?>(),
                new List<AddressResponse>(),
                new List<PhoneNumberResponse>()
            );
        }
    }

    public static class DataExportsExtensions
    {
        public static DataExportResponse MapToDataExport(this CreateDataExportRequest request)
        {
            return new DataExportResponse
            {
                FileName = request.FileName
            };
        }
    }

    public static class FilesExtensions
    {
        public static FileResponse MapToFile(this CreateFileRequest request)
        {
            return new FileResponse
            {
                FileName = request.FileName
            };
        }
    }

    public static class AuditLogsExtensions
    {
        public static AuditLogResponse MapToAuditLog(this CreateAuditLogRequest request)
        {
            return new AuditLogResponse
            {
                OldValue = request.OldValue
            };
        }
    }

    public static class NotificationsExtensions
    {
        public static NotificationResponse MapToNotification(this CreateNotificationRequest request)
        {
            return new NotificationResponse
            {
                Title = request.Title,
                Message = request.Message,
                CreatedAt = request.CreatedAt,
                IsRead = request.IsRead
            };
        }
    }

    public static class ProfilesExtensions
    {
        public static ProfileResponse MapToProfile(this CreateProfileRequest request)
        {
            return new ProfileResponse
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth
            };
        }
    }

    public static class RolesExtensions
    {
        public static RoleResponse MapToRole(this CreateRoleRequest request)
        {
            return new RoleResponse((IReadOnlyList<PermissionResponse>)new List<PermissionResponse>());
        }
    }

    public static class IntegrationsExtensions
    {
        public static IntegrationResponse MapToIntegration(this CreateIntegrationRequest request)
        {
            return new IntegrationResponse
            {
                IsRead = request.IsRead,
                CreatedAt = request.CreatedAt
            };
        }
    }
}
