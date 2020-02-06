using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BugTracker_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker_API.Data
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly DataContext _dataContext;

        public AuthenticationRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public async Task<User> RegisterUser(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(
            string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmacSHA512 = new HMACSHA512())
            {
                passwordSalt = hmacSHA512.Key;
                passwordHash = hmacSHA512.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<User> LoginUser(string email, string password)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(
                x => x.Email == email);

            if (user == null)
                return null;

            if (!ValidPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool ValidPasswordHash(
            string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmacSHA512 = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmacSHA512.ComputeHash(
                    Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; ++i)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
                return true;
            }
        }

        public async Task<bool> EmailExists(string email)
        {
            if (await _dataContext.Users.AnyAsync(x => x.Email == email))
                return true;
            else
                return false;
        }
    }
}