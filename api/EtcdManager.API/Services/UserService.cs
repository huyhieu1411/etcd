using EtcdManager.API.Models;
using EtcdManager.API.Repos;

namespace EtcdManager.API.Services
{
    public interface IUserService
    {
        Task<ResponseModel<bool>> ChangeMyPassword(ChangePasswordModel changePasswordModel);
        Task<ResponseModel<User>> CreateUser(User userModel);
        Task<ResponseModel<User>> UpdateUser(User userModel);
        Task<ResponseModel<User>> DeleteUser(User userModel);
        Task<ResponseModel<User>> GetUserById(int id);
        Task<ResponseModel<User>> GetUserByName(string name);
        Task<ResponseModel<List<User>>> GetUsers();
        Task<ResponseModel<User>> GetUserInfo();
    }

    public class UserService : IUserService
    {
        private IUserRepos _userRepos;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenRepos _authenRepos;

        public UserService(
            IUserRepos userRepos,
            IHttpContextAccessor httpContextAccessor,
            IAuthenRepos authenRepos
        )
        {
            this._userRepos = userRepos;
            this._httpContextAccessor = httpContextAccessor;
            this._authenRepos = authenRepos;
        }

        public async Task<ResponseModel<bool>> ChangeMyPassword(ChangePasswordModel changePasswordModel)
        {
            try
            {
                var userInfo = await _userRepos.GetUserInfo();
                if (userInfo.Data == null)
                {
                    return ResponseModel<bool>.ResponseWithError("User not found");
                }
                userInfo.Data.Password = changePasswordModel.NewPassword;
                await _userRepos.UpdateUser(userInfo.Data);
                return ResponseModel<bool>.ResponseWithData(true);
            }
            catch (Exception ex)
            {
                return ResponseModel<bool>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<User>> CreateUser(User userModel)
        {
            try
            {
                var existUser = await _userRepos.GetByUserName(userModel.UserName);
                if (existUser.Data != null)
                {
                    return ResponseModel<User>.ResponseWithError("User already exists");
                }
                var userInfo = await _userRepos.CreateUser(userModel);
                return ResponseModel<User>.ResponseWithData(userInfo.Data);
            }
            catch (Exception ex)
            {
                return ResponseModel<User>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<User>> DeleteUser(User userModel)
        {
            try
            {
                var userInfo = await _userRepos.GetUserById(userModel.Id);
                if (userInfo.Data == null)
                {
                    return ResponseModel<User>.ResponseWithError("User not found");
                }

                if (userInfo.Data.UserName == "root")
                {
                    return ResponseModel<User>.ResponseWithError("Can not delete root user");
                }

                await _userRepos.DeleteUser(userInfo.Data);
                return ResponseModel<User>.ResponseWithData(userInfo.Data);
            }
            catch (Exception ex)
            {
                return ResponseModel<User>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<List<User>>> GetUsers()
        {
            return await _userRepos.GetUsers();
        }

        public async Task<ResponseModel<User>> GetUserById(int id)
        {
            return await _userRepos.GetUserById(id);
        }

        public async Task<ResponseModel<User>> GetUserByName(string name)
        {
            return await _userRepos.GetUserByName(name);
        }

        public async Task<ResponseModel<User>> UpdateUser(User userModel)
        {
            try
            {
                var userInfo = await _userRepos.GetByUserName(userModel.UserName);
                if (userInfo.Data == null)
                {
                    return ResponseModel<User>.ResponseWithError("User not found");
                }

                // for now nothing to update
                await _userRepos.UpdateUser(userInfo.Data);
                return ResponseModel<User>.ResponseWithData(userInfo.Data);
            }
            catch (Exception ex)
            {
                return ResponseModel<User>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<User>> GetUserInfo()
        {
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            var user = await _authenRepos.GetByToken(accessToken);
            if (user.Success)
            {
                return await GetUserByName(user.Data.UserName);
            }
            return ResponseModel<User>.ResponseWithError("User not found");
        }
    }
}