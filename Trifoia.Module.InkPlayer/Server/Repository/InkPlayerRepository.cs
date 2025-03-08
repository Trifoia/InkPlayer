using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using System.Threading.Tasks;

namespace Trifoia.Module.InkPlayer.Repository
{
    public class InkPlayerRepository : ITransientService
    {
        private readonly IDbContextFactory<InkPlayerContext> _factory;

        public InkPlayerRepository(IDbContextFactory<InkPlayerContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<Models.InkPlayer> GetInkPlayers()
        {
            using var db = _factory.CreateDbContext();
            return db.InkPlayer.ToList();
        }

        public Models.InkPlayer GetInkPlayer(int InkPlayerId)
        {
            return GetInkPlayer(InkPlayerId, true);
        }

        public Models.InkPlayer GetInkPlayer(int InkPlayerId, bool tracking)
        {
            using var db = _factory.CreateDbContext();
            if (tracking)
            {
                return db.InkPlayer.Find(InkPlayerId);
            }
            else
            {
                return db.InkPlayer.AsNoTracking().FirstOrDefault(item => item.InkPlayerId == InkPlayerId);
            }
        }

        public Models.InkPlayer AddInkPlayer(Models.InkPlayer item)
        {
            using var db = _factory.CreateDbContext();
            db.InkPlayer.Add(item);
            db.SaveChanges();
            return item;
        }

        public Models.InkPlayer UpdateInkPlayer(Models.InkPlayer item)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            return item;
        }

        public void DeleteInkPlayer(int InkPlayerId)
        {
            using var db = _factory.CreateDbContext();
            var item = db.InkPlayer.Find(InkPlayerId);
            db.InkPlayer.Remove(item);
            db.SaveChanges();
        }


        public async Task<IEnumerable<Models.InkPlayer>> GetInkPlayersAsync(int ModuleId)
        {
            using var db = _factory.CreateDbContext();
            return await db.InkPlayer.Where(item => item.ModuleId == ModuleId).ToListAsync();
        }

        public async Task<Models.InkPlayer> GetInkPlayerAsync(int InkPlayerId)
        {
            return await GetInkPlayerAsync(InkPlayerId, true);
        }

        public async Task<Models.InkPlayer> GetInkPlayerAsync(int InkPlayerId, bool tracking)
        {
            using var db = _factory.CreateDbContext();
            if (tracking)
            {
                return await db.InkPlayer.FindAsync(InkPlayerId);
            }
            else
            {
                return await db.InkPlayer.AsNoTracking().FirstOrDefaultAsync(item => item.InkPlayerId == InkPlayerId);
            }
        }

        public async Task<Models.InkPlayer> AddInkPlayerAsync(Models.InkPlayer item)
        {
            using var db = _factory.CreateDbContext();
            db.InkPlayer.Add(item);
            await db.SaveChangesAsync();
            return item;
        }

        public async Task<Models.InkPlayer> UpdateInkPlayerAsync(Models.InkPlayer item)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return item;
        }

        public async Task DeleteInkPlayerAsync(int InkPlayerId)
        {
            using var db = _factory.CreateDbContext();
           var item = db.InkPlayer.Find(InkPlayerId);
            db.InkPlayer.Remove(item);
            await db.SaveChangesAsync();
        }
    }
}
