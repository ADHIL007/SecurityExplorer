using SecurityExplorer.SampleWebApi.Entities;

namespace SecurityExplorer.SampleWebApi.Services
{
    public interface IUserCacheService
    {
        bool CheckCacheEmpty();
        List<User> GetAllUsers();
        void AddUserToCache(User user);
        User? GetUserFromCache(int id);
        void AddOrUpdateCache(User user);
        void RemoveUserFromCache(int id);
        void ClearCache();
    }
}
