using Microsoft.EntityFrameworkCore;
using SecretVault.Domain.Entities;
using SecretVault.Domain.Interfaces;
using SecretVault.Infrastructure.Persistence;

namespace SecretVault.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAync(User user)
        {
            user.Id = Guid.NewGuid();
            user.Email = user.Email.ToLower();
            user.CreatedOn = DateTime.UtcNow;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email.ToLower());
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);

            if (existingUser == null)
                return;
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email.ToLower();
            existingUser.RefreshToken = user.RefreshToken;
            existingUser.TokenExpiry = user.TokenExpiry;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
        }
    }
}
