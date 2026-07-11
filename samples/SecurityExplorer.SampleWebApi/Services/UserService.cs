using SecurityExplorer.SampleWebApi.DTOs;
using SecurityExplorer.SampleWebApi.Entities;
using SecurityExplorer.SampleWebApi.Repositories;

namespace SecurityExplorer.SampleWebApi.Services
{
    public class UserService(IUserRepository userRepository, IUserCacheService userCacheService) : IUserService
    {


        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUserCacheService _userCacheService = userCacheService;
        public async Task<User> CreateUserAsync(CreateUserRequest dto)
        {
            var user = new User
            {

                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Department = dto.Department,



            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();
            return user;

        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            if (!_userCacheService.CheckCacheEmpty())
            {
                Console.WriteLine("Users retrieved from cache.");
                return _userCacheService.GetAllUsers();
            }
            else
            {
                Console.WriteLine("Users retrieved from repository.");
                var users = await _userRepository.GetAllAsync();
                foreach (var user in users)
                {
                    _userCacheService.AddUserToCache(user);
                }
                return users;
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = _userCacheService.GetUserFromCache(id);
            if (user != null)
            {
                Console.WriteLine("User retrieved from cache.");
                return user;
            }
            else
            {
                Console.WriteLine("User retrieved from repository.");
                var userFromRepo = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
                _userCacheService.AddUserToCache(userFromRepo);
                return userFromRepo;
            }
        }

        public async Task<User> UpdateUserEmailAsync(int id, string email)
        {
            User user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
            user.Email = email;
            _userCacheService.AddOrUpdateCache(user);
            await _userRepository.SaveAsync();

            return user;
        }

        public async Task DeleteUserByIDAsync(int id)
        {

            var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");

            _userRepository.Delete(user);
            _userCacheService.RemoveUserFromCache(id);
            await _userRepository.SaveAsync();


        }
    }

}
