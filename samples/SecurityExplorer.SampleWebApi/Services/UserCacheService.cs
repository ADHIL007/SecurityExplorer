
using System.Collections.Concurrent;
using SecurityExplorer.SampleWebApi.Entities;

namespace SecurityExplorer.SampleWebApi.Services
{

    public class UserCacheService : IUserCacheService
    {
        private readonly ConcurrentDictionary<int, User> _cache = new ConcurrentDictionary<int, User>();

        bool IUserCacheService.CheckCacheEmpty()
        {
            return _cache.IsEmpty;
        }
        List<User> IUserCacheService.GetAllUsers()
        {
            return _cache.Values.ToList();
        }
        void IUserCacheService.AddUserToCache(User user)
        {

            _cache[user.Id] = user;

        }

        void IUserCacheService.ClearCache()
        {
            _cache.Clear();
        }

        User? IUserCacheService.GetUserFromCache(int id)
        {
            _cache.TryGetValue(id, out var user);
            return user;
        }

        void IUserCacheService.AddOrUpdateCache(User newuser)
        {
            _cache.AddOrUpdate(newuser.Id, newuser, (key, existingUser) => newuser);

        }

        void IUserCacheService.RemoveUserFromCache(int id)
        {
            _cache.TryRemove(id, out _);
        }

    }

}
