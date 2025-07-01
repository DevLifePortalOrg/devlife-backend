// DevLife.Infrastructure/Repositories/UserRepository.cs
using DevLife.Api.Data; // Access AppDbContext
using DevLife.Domain.Entities;
using DevLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserEntity> GetUserByIdAsync(Guid id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<UserEntity> GetUserByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserEntity> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(UserEntity user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(UserEntity user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserEntity>> GetAllUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<bool> UserExistsAsync(Guid id)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> UserExistsByUsernameOrEmailAsync(string username, string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Username == username || u.Email == email);
        }
    }
}