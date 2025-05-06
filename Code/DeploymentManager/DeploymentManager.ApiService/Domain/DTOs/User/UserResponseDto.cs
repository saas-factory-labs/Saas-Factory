using Domain.Enum.User;
using JetBrains.Annotations;

namespace DeploymentManager.ApiService.Domain.DTOs.User;

[UsedImplicitly]
public sealed record UserResponseDto(
    string Id,
    DateTime CreatedAt,
    DateTime? ModifiedAt,
    string Email,
    UserRole UserRole
);
