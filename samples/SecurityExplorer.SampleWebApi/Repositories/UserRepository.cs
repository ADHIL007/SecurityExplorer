using SecurityExplorer.SampleWebApi.Data;
using SecurityExplorer.SampleWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace SecurityExplorer.SampleWebApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        public UserRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;

        }
        async Task<User> IUserRepository.AddAsync(User user)
        {
            User user_ = (await _dbContext.Users.AddAsync(user)).Entity;

            return user_;

        }

        void IUserRepository.Delete(User user)
        {
            _dbContext.Users.Remove(user);
        }

        async Task<List<User>> IUserRepository.GetAllAsync()
        {
            return await _dbContext.Users.Where(user => string.IsNullOrEmpty(user.Email) == false && user.Email.EndsWith("@gmail.com") && user.Name.StartsWith("A")).AsNoTracking().ToListAsync();


        }

        async Task<User?> IUserRepository.GetByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        async Task IUserRepository.SaveAsync()
        {
            await _dbContext.SaveChangesAsync();

        }
    }
}
