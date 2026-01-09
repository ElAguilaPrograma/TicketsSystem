using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TicketsSystem.Data.DTOs;

namespace TicketsSystem_Data.Repositories
{
    public interface IUserRepository
    {
        Task CreateNewUser(User newUser);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User?> Login(string email);
    }

    public class UserRepository: IUserRepository
    {
        private readonly SystemTicketsContext _context;
        private readonly IConfiguration _configuration;

        public UserRepository(
            SystemTicketsContext systemTicketsContext, 
            IConfiguration configuration)
        {
            _context = systemTicketsContext;
            _configuration = configuration;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            return users;
        }

        public async Task CreateNewUser(User newUser)
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> Login(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

    }
}
