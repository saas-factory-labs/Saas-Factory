// using AppBlueprint.Application.DTOs;
// using AppBlueprint.Application.DTOs.Baseline.Contracts.User.Requests;
// using AppBlueprint.Application.DTOs.Baseline.Contracts.User.Responses;
// using AppBlueprint.Infrastructure.Repositories.Interfaces;
//
// namespace AppBlueprint.Application.Services.User;
//
// public class UserService : IUserService
// {
//     private readonly IUserRepository _userRepository;
//     
//
//     
//     public UserService(IUserRepository userRepository)
//     {
//         _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
//     }
//
//     public async Task<UserResponse> GetUserByIdAsync(int userId)
//     {
//         var user = await _userRepository.GetByIdAsync(userId)
//                     ?? throw new KeyNotFoundException("User not found");
//
//         return new UserResponse();
//
//         // return new UserResponse(user.Id, user.FirstName, user.Email);
//     }
//
//     public async Task<UserResponse> CreateUserAsync(CreateUserRequest userDto)
//     {
//         if (await _userRepository.ExistsByEmailAsync(userDto.Email))
//             throw new InvalidOperationException("Email is already taken");
//
//         var hashedPassword = _passwordHasher.HashPassword(userDto.Password);
//         var user = new User(Guid.NewGuid(), userDto.Name, userDto.Email, hashedPassword);
//
//         await _userRepository.AddAsync(user);
//
//         return new UserResponse(user.Id, user.Name, user.Email);
//     }
//
//     public async Task UpdateUserAsync(Guid userId, UpdateUserRequest userDto)
//     {
//         var user = await _userRepository.GetByIdAsync(userId)
//                     ?? throw new KeyNotFoundException("User not found");
//
//         user.UpdateName(userDto.Name);
//         await _userRepository.UpdateAsync(user);
//     }
//
//     public async Task DeleteUserAsync(Guid userId)
//     {
//         var user = await _userRepository.GetByIdAsync(userId)
//                     ?? throw new KeyNotFoundException("User not found");
//
//         await _userRepository.DeleteAsync(user);
//     }
//
//     Task<UserResponse> IUserService.GetUserByIdAsync(Guid userId)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<UserResponse> CreateUserAsync(CreateUserRequest userDto)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task UpdateUserAsync(Guid userId, UpdateUserRequest userDto)
//     {
//         throw new NotImplementedException();
//     }
// }



