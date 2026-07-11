using SecurityExplorer.SampleWebApi.Entities;

namespace SecurityExplorer.SampleWebApi.Repositories
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);

        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);
        void Delete(User user);

        Task SaveAsync();
    }
}
