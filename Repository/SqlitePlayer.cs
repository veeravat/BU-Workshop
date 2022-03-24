using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetRPG.Context;
using dotnetRPG.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnetRPG.Repository
{
    public class SqlitePlayer : IPlayer
    {
        private PlayerDBContext db;

        public SqlitePlayer(PlayerDBContext db)
        {
            this.db = db;
        }

        public async Task CreatePlayer(Player player)
        {
            Player newplayer = new()
            {
                Id = Guid.NewGuid(),
                Name = player.Name,
                Balance = player.Balance,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await db.players.AddAsync(newplayer);
            await db.SaveChangesAsync();
        }

        public async Task DeletePlayer(Guid Id)
        {
            // select * from player where player.Id = Id 
            var existedPlayer = await db.players
                                .Where(p => p.Id == Id)
                                .FirstOrDefaultAsync();
            if (existedPlayer is null)
            {
                throw new Exception("Player not found");
            }

            db.players.Remove(existedPlayer);
            await db.SaveChangesAsync();
        }

        public async Task<Player> GetPlayer(Guid Id)
        {
            var player = await db.players.Where(p => p.Id == Id).FirstOrDefaultAsync();
            if (player is null)
            {
                throw new Exception("Player not found");
            }

            return player;
        }

        public async Task<List<Player>> GetPlayers()
        {
            var players = await db.players.ToListAsync();
            if (players is null)
            {
                throw new Exception("Player not found");
            }
            return players;
        }

        public async Task UpdatePlayer(Guid Id, Player player)
        {
            var existedPlayer = await db.players
                    .Where(p => p.Id == Id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            if (existedPlayer is null)
            {
                throw new Exception("Player not found");
            }
            existedPlayer = existedPlayer with
            {
                Name = player.Name,
                Balance = player.Balance
            };
            db.players.Update(existedPlayer);
            await db.SaveChangesAsync();

        }
    }
}