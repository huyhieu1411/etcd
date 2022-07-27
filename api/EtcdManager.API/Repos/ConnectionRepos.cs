using EtcdManager.API.Database;
using EtcdManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EtcdManager.API.Repos
{
    public interface IConnectionRepos
    {
        Task<ResponseModel<List<ConnectionModel>?>> GetConnections(string userName);
        Task<ResponseModel<string>> CreateConnection(ConnectionModel connectionModel);
        Task<ResponseModel<bool>> UpdateConnection(int id, ConnectionModel connectionModel);
        Task<ResponseModel<bool>> DeleteConnection(int id);
        Task<ResponseModel<List<ConnectionModel>>> GetConnections();
        Task<ResponseModel<bool>> DeleteConnectionByName(string name);
        Task<ResponseModel<ConnectionModel>> GetConnectionByName(string name);

    }

    public class ConnectionRepos : IConnectionRepos
    {
        private readonly EtcdDataContext _etcdDataContext;

        public ConnectionRepos(
            EtcdDataContext etcdDataContext
        )
        {
            this._etcdDataContext = etcdDataContext;
        }

        public async Task<ResponseModel<string>> CreateConnection(ConnectionModel connectionModel)
        {
            try
            {
                connectionModel.CreatedAt = DateTime.Now;
                await _etcdDataContext.Connections.AddAsync(connectionModel);
                await _etcdDataContext.SaveChangesAsync();
                return ResponseModel<string>.ResponseWithData("ok");
            }
            catch (Exception ex)
            {
                return ResponseModel<string>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<bool>> DeleteConnection(int id)
        {
            try
            {
                var connection = await _etcdDataContext.Connections.FirstOrDefaultAsync(x => x.Id == id);
                if (connection == null)
                {
                    return ResponseModel<bool>.ResponseWithError("Connection not found");
                }
                _etcdDataContext.Connections.Remove(connection);
                await _etcdDataContext.SaveChangesAsync();
                return ResponseModel<bool>.ResponseWithData(true);
            }
            catch (Exception ex)
            {
                return ResponseModel<bool>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<bool>> DeleteConnectionByName(string name)
        {
            try
            {
                var connection = await _etcdDataContext.Connections.FirstOrDefaultAsync(x => x.Name == name);
                if (connection == null)
                {
                    return ResponseModel<bool>.ResponseWithError("Connection not found");
                }
                _etcdDataContext.Connections.Remove(connection);
                await _etcdDataContext.SaveChangesAsync();
                return ResponseModel<bool>.ResponseWithData(true);
            }
            catch (Exception ex)
            {
                return ResponseModel<bool>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<ConnectionModel>> GetConnectionByName(string name)
        {
            try
            {
                var connection = await _etcdDataContext.Connections.FirstOrDefaultAsync(x => x.Name == name);
                if (connection == null)
                {
                    return ResponseModel<ConnectionModel>.ResponseWithError("Connection not found");
                }
                return ResponseModel<ConnectionModel>.ResponseWithData(connection);
            }
            catch (Exception ex)
            {
                return ResponseModel<ConnectionModel>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<List<ConnectionModel>?>> GetConnections(string userName)
        {
            try
            {
                var connections = await _etcdDataContext.Connections.Where(x => x.UserName == userName).ToListAsync();
                return ResponseModel<List<ConnectionModel>?>.ResponseWithData(connections);
            }
            catch (Exception ex)
            {
                return ResponseModel<List<ConnectionModel>?>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<List<ConnectionModel>>> GetConnections()
        {
            try
            {
                var connections = await _etcdDataContext.Connections.ToListAsync();
                return ResponseModel<List<ConnectionModel>>.ResponseWithData(connections);
            }
            catch (Exception ex)
            {
                return ResponseModel<List<ConnectionModel>>.ResponseWithError(ex.Message);
            }
        }

        public async Task<ResponseModel<bool>> UpdateConnection(int id, ConnectionModel connectionModel)
        {
            try
            {
                var connection = await _etcdDataContext.Connections.FirstOrDefaultAsync(x => x.Id == id);
                if (connection == null)
                {
                    return ResponseModel<bool>.ResponseWithError("Connection not found");
                }
                connection.Name = connectionModel.Name;
                connection.UserName = connectionModel.UserName;
                connection.Password = connectionModel.Password;
                connection.AgentDomain = connectionModel.AgentDomain;
                await _etcdDataContext.SaveChangesAsync();
                return ResponseModel<bool>.ResponseWithData(true);
            }
            catch (Exception ex)
            {
                return ResponseModel<bool>.ResponseWithError(ex.Message);
            }
        }
    }
}