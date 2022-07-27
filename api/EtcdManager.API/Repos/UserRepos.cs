using EtcdManager.API.Database;
using EtcdManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EtcdManager.API.Repos
{
    public interface IUserRepos
    {
        Task<ResponseModel<List<User>>> GetUsers();
        Task<ResponseModel<User>> CreateUser(User user);
        Task<ResponseModel<User>> DeleteUser(User user);
        Task<ResponseModel<bool>> GetByUserNameAndPassword(string userName, string password);
        Task<ResponseModel<User>> GetByUserName(string userName);
        Task<ResponseModel<User>> GetUserInfo();
        Task<ResponseModel<bool>> UpdateUser(User user);
        Task<ResponseModel<User>> GetUserById(int id);
        Task<ResponseModel<User>> GetUserByName(string name);
    }

    public class UserRepos : IUserRepos
    {
        private readonly EtcdDataContext _etcdDataContext;

        public UserRepos(EtcdDataContext etcdDataContext)
        {
            this._etcdDataContext = etcdDataContext;
        }

        public async Task<ResponseModel<User>> CreateUser(User user)
        {
            try
            {
                await _etcdDataContext.Users.AddAsync(user);
                await _etcdDataContext.SaveChangesAsync();
                return ResponseModel<User>.ResponseWithData(user);
            }
            catch (Exception ex)
            {
                return ResponseModel<User>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<User>> DeleteUser(User user)
        {
            try
            {
                var userInfo = await _etcdDataContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                if (userInfo == null)
                {
                    return ResponseModel<User>.ResponseWithError("User not found");
                }
                _etcdDataContext.Users.Remove(userInfo);
                await _etcdDataContext.SaveChangesAsync();
                return ResponseModel<User>.ResponseWithData(userInfo);
            }
            catch (Exception ex)
            {
                return ResponseModel<User>.ResponseWithError(ex.Message);
            }
        }

        public Task<ResponseModel<List<User>>> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<User>> GetByUserName(string userName)
        {
            try
            {
                var userInfo = await _etcdDataContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);
                if (userInfo == null)
                {
                    return ResponseModel<User>.ResponseWithError("User not found");
                }
                return ResponseModel<User>.ResponseWithData(userInfo);
            }
            catch (Exception ex)
            {
                return ResponseModel<User>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<bool>> GetByUserNameAndPassword(string userName, string password)
        {
            try
            {
                var userInfo = await _etcdDataContext.Users.FirstOrDefaultAsync(x => x.UserName == userName && x.Password == password);
                if (userInfo == null)
                {
                    return ResponseModel<bool>.ResponseWithError("User not found");
                }
                return ResponseModel<bool>.ResponseWithData(true);
            }
            catch (Exception ex)
            {
                return ResponseModel<bool>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<User>> GetUserById(int id)
        {
            try
            {
                var userInfo = await _etcdDataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (userInfo == null)
                {
                    return ResponseModel<User>.ResponseWithError("User not found");
                }
                return ResponseModel<User>.ResponseWithData(userInfo);
            }
            catch (Exception ex)
            {
                return ResponseModel<User>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<User>> GetUserByName(string name)
        {
            try
            {
                var userInfo = await _etcdDataContext.Users.FirstOrDefaultAsync(x => x.UserName == name);
                if (userInfo == null)
                {
                    return ResponseModel<User>.ResponseWithError("User not found");
                }
                return ResponseModel<User>.ResponseWithData(userInfo);
            }
            catch (Exception ex)
            {
                return ResponseModel<User>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<User>> GetUserInfo()
        {
            try
            {
                var userInfo = await _etcdDataContext.Users.FirstOrDefaultAsync(x => x.Id == 1);
                if (userInfo == null)
                {
                    return ResponseModel<User>.ResponseWithError("User not found");
                }
                return ResponseModel<User>.ResponseWithData(userInfo);
            }
            catch (Exception ex)
            {
                return ResponseModel<User>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<List<User>>> GetUsers()
        {
            try
            {
                var userInfo = await _etcdDataContext.Users.ToListAsync();
                if (userInfo == null)
                {
                    return ResponseModel<List<User>>.ResponseWithError("User not found");
                }
                return ResponseModel<List<User>>.ResponseWithData(userInfo);
            }
            catch (Exception ex)
            {
                return ResponseModel<List<User>>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<bool>> UpdateUser(User user)
        {
            try
            {
                var userInfo = await _etcdDataContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                if (userInfo == null)
                {
                    return ResponseModel<bool>.ResponseWithError("User not found");
                }
                userInfo.UserName = user.UserName;
                userInfo.Password = user.Password;
                _etcdDataContext.Users.Update(userInfo);
                _etcdDataContext.SaveChanges();
                return ResponseModel<bool>.ResponseWithData(true);
            }
            catch (Exception ex)
            {
                return ResponseModel<bool>.ResponseWithError(ex.Message);
            }
        }
    }
}