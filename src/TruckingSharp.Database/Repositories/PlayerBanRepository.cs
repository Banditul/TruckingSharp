﻿using Dapper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TruckingSharp.Database.Entities;

namespace TruckingSharp.Database.Repositories
{
    public sealed class PlayerBanRepository
    {
        private readonly IDatabaseConnection _databaseConnectionFactory;

        public PlayerBanRepository(IDatabaseConnection databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        #region Async

        public async Task<long> AddAsync(PlayerBan entity)
        {
            try
            {
                const string command = "INSERT INTO player_bans (reason, duration, admin_id, owner_id) VALUES (@Reason, @Duration, @AdminId, @OwnerId);";

                using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();

                return await sqlConnection.ExecuteAsync(command, new
                {
                    entity.Reason,
                    entity.Duration,
                    entity.AdminId,
                    entity.OwnerId
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to to insert async player ban with owner id: {entity.OwnerId}.");
                throw;
            }
        }

        public async Task<int> DeleteAsync(PlayerBan entity)
        {
            try
            {
                const string command = "DELETE FROM player_bans WHERE id = @Id;";

                using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();

                return await sqlConnection.ExecuteAsync(command, new
                {
                    entity.Id
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to to delete async player ban with owner id: {entity.OwnerId}.");
                throw;
            }
        }

        public async Task<PlayerBan> FindAsync(int id)
        {
            try
            {
                const string command = "SELECT * FROM player_bans WHERE owner_id = @Id;";

                using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();

                return await sqlConnection.QueryFirstOrDefaultAsync<PlayerBan>(command, new
                {
                    Id = id
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to to fetch async player ban with owner id: {id}.");
                throw;
            }
        }

        public async Task<PlayerBan> FindAsync(string name)
        {
            try
            {
                const string command = "SELECT player_bans.* FROM player_bans LEFT JOIN player_accounts ON player_bans.owner_id = player_accounts.id WHERE player_accounts.name = @Name;";

                using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();

                return await sqlConnection.QueryFirstOrDefaultAsync<PlayerBan>(command, new
                {
                    Name = name
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to to fetch async player ban with owner name: {name}.");
                throw;
            }
        }

        public async Task<IEnumerable<PlayerBan>> GetAllAsync()
        {
            try
            {
                const string command = "SELECT * FROM player_bans;";

                using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();

                return await sqlConnection.QueryAsync<PlayerBan>(command);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fetch all players bans async.");
                throw;
            }
        }

        public async Task<int> UpdateAsync(PlayerBan entity)
        {
            try
            {
                const string command = "UPDATE player_bans SET reason = @Reason, duration = @Duration, admin_id = @AdminId, owner_id = @OwnerId WHERE id = @Id;";

                using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();

                return await sqlConnection.ExecuteAsync(command, new
                {
                    entity.Reason,
                    entity.Duration,
                    entity.AdminId,
                    entity.OwnerId,
                    entity.Id
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to to update async player ban with owner id: {entity.OwnerId}.");
                throw;
            }
        }

        #endregion Async
    }
}