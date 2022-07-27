using EtcdManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EtcdManager.API.Database
{
    public class EtcdDataContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<ConnectionModel> Connections { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AuthenModel> Authens { get; set; }

        public EtcdDataContext(string connectionString)
        {
            this._connectionString = connectionString;
        }
        public EtcdDataContext(DbContextOptions<EtcdDataContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(this._connectionString);
            }
        }

        internal void SeedData()
        {
            if (!this.Users.Any(x => x.UserName == "root"))
            {
                this.Users.Add(new User()
                {
                    UserName = "root",
                    Password = "root"
                });
                this.SaveChanges();
            }
        }
    }
}