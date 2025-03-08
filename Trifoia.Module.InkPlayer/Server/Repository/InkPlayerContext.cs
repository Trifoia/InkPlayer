using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Infrastructure;
using Oqtane.Repository.Databases.Interfaces;

namespace Trifoia.Module.InkPlayer.Repository
{
    public class InkPlayerContext : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<Models.InkPlayer> InkPlayer { get; set; }

        public InkPlayerContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
        {
            // ContextBase handles multi-tenant database connections
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Models.InkPlayer>().ToTable(ActiveDatabase.RewriteName("TrifoiaInkPlayer"));
        }
    }
}
