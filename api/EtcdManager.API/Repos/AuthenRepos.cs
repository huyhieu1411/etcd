using EtcdManager.API.Database;
using EtcdManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EtcdManager.API.Repos
{
    public interface IAuthenRepos
    {
        Task<ResponseModel<bool>> TokenIsValid(string token);
        Task<ResponseModel<string>> Login(string userName, string password);
        Task<ResponseModel<string>> Logout();
        Task<ResponseModel<AuthenModel>> GetByToken(string token);
    }

    public class AuthenRepos : IAuthenRepos
    {
        private readonly EtcdDataContext _etcdDataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenRepos(
            EtcdDataContext etcdDataContext,
            IHttpContextAccessor httpContextAccessor
        )
        {
            this._etcdDataContext = etcdDataContext;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseModel<string>> DeleteToken(string token)
        {
            try
            {
                var tokenInfo = await _etcdDataContext.Authens.FirstOrDefaultAsync(x => x.Token == token);
                if (tokenInfo == null)
                {
                    return ResponseModel<string>.ResponseWithError("Token not found");
                }
                _etcdDataContext.Authens.Remove(tokenInfo);
                await _etcdDataContext.SaveChangesAsync();
                return ResponseModel<string>.ResponseWithData("");
            }
            catch (Exception ex)
            {
                return ResponseModel<string>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<AuthenModel>> GetByToken(string token)
        {
            try
            {
                var authen = await _etcdDataContext.Authens.FirstOrDefaultAsync(x => x.Token == token);
                if (authen == null)
                {
                    return ResponseModel<AuthenModel>.ResponseWithError("Token not found");
                }
                return ResponseModel<AuthenModel>.ResponseWithData(authen);
            }
            catch (Exception ex)
            {
                return ResponseModel<AuthenModel>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<string>> Login(string userName, string password)
        {
            try
            {
                var user = await _etcdDataContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);
                if (user == null)
                {
                    return ResponseModel<string>.ResponseWithError("User not found");
                }
                var authen = new AuthenModel();
                var token = Guid.NewGuid().ToString();
                authen.Token = token;
                authen.UserName = user.UserName;
                authen.ExpiredAt = DateTime.Now.AddDays(1);
                await _etcdDataContext.AddAsync(authen);
                await _etcdDataContext.SaveChangesAsync();
                return ResponseModel<string>.ResponseWithData(token);
            }
            catch (Exception ex)
            {
                return ResponseModel<string>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<string>> Logout()
        {
            try
            {
                var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return ResponseModel<string>.ResponseWithError("Token not found");
                }
                return await DeleteToken(accessToken);
            }
            catch (Exception ex)
            {
                return ResponseModel<string>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<bool>> TokenIsValid(string token)
        {
            try
            {
                var authen = await _etcdDataContext.Authens.FirstOrDefaultAsync(x => x.Token == token);
                if (authen == null)
                {
                    return ResponseModel<bool>.ResponseWithError("Authentication not found");
                }
                if (authen.ExpiredAt < DateTime.Now)
                {
                    return ResponseModel<bool>.ResponseWithError("Authentication expired");
                }
                return ResponseModel<bool>.ResponseWithData(true);
            }
            catch (Exception ex)
            {
                return ResponseModel<bool>.ResponseWithError(ex.Message);
            }
        }
    }
}