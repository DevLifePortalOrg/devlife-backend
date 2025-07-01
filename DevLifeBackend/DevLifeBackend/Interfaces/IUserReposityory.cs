// DevLife.Domain/Interfaces/IUserRepository.cs
using DevLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevLife.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEntity> GetUserByIdAsync(Guid id);
        Task<UserEntity> GetUserByUsernameAsync(string username);
        Task<UserEntity> GetUserByEmailAsync(string email);
        Task AddUserAsync(UserEntity user);
        Task UpdateUserAsync(UserEntity user);
        Task DeleteUserAsync(Guid id);
        Task<IEnumerable<UserEntity>> GetAllUsersAsync();
        Task<bool> UserExistsAsync(Guid id);
        Task<bool> UserExistsByUsernameOrEmailAsync(string username, string email);
    }
}