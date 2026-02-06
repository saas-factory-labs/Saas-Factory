using AppBlueprint.Contracts.Baseline.User.Requests;
using AppBlueprint.Contracts.Baseline.User.Responses;

namespace AppBlueprint.Application.Services.User;

public interface IUserService
{
    Task<UserResponse> GetUserByIdAsync(Guid userId);
    Task<UserResponse> CreateUserAsync(CreateUserRequest userDto);
    Task UpdateUserAsync(Guid userId, UpdateUserRequest userDto);
    Task DeleteUserAsync(Guid userId);
}
