using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TennisInventoryApp.Models;

namespace TennisInventoryApp.Data.AdoNet
{
    public class AdoNetPlayerRepository
    {
        private readonly string _connectionString;

        public AdoNetPlayerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // CREATE
        public async Task<int> AddAsync(Player player)
        {
            const string sql = @"
                INSERT INTO Players (Id, FullName, Email, Phone, BirthDate, IsActive, RegisteredAt, Rating) 
                VALUES (@Id, @FullName, @Email, @Phone, @BirthDate, @IsActive, @RegisteredAt, @Rating)";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            
            command.Parameters.AddWithValue("@Id", player.Id);
            command.Parameters.AddWithValue("@FullName", player.FullName);
            command.Parameters.AddWithValue("@Email", player.Email);
            command.Parameters.AddWithValue("@Phone", (object)player.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@BirthDate", (object)player.BirthDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", player.IsActive);
            command.Parameters.AddWithValue("@RegisteredAt", player.RegisteredAt);
            command.Parameters.AddWithValue("@Rating", player.Rating);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        // READ - все игроки
        public async Task<List<Player>> GetAllAsync()
        {
            var players = new List<Player>();
            const string sql = "SELECT * FROM Players ORDER BY Rating DESC";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                players.Add(new Player
                {
                    Id = reader.GetGuid(0),
                    FullName = reader.GetString(1),
                    Email = reader.GetString(2),
                    Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                    BirthDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    IsActive = reader.GetBoolean(5),
                    RegisteredAt = reader.GetDateTime(6),
                    Rating = reader.GetDecimal(7)
                });
            }

            return players;
        }

        // READ - по ID
        public async Task<Player> GetByIdAsync(Guid id)
        {
            const string sql = "SELECT * FROM Players WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Player
                {
                    Id = reader.GetGuid(0),
                    FullName = reader.GetString(1),
                    Email = reader.GetString(2),
                    Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                    BirthDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    IsActive = reader.GetBoolean(5),
                    RegisteredAt = reader.GetDateTime(6),
                    Rating = reader.GetDecimal(7)
                };
            }

            return null;
        }

        // UPDATE
        public async Task<int> UpdateAsync(Player player)
        {
            const string sql = @"
                UPDATE Players 
                SET FullName = @FullName, Email = @Email, Phone = @Phone, 
                    BirthDate = @BirthDate, IsActive = @IsActive, Rating = @Rating
                WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            
            command.Parameters.AddWithValue("@Id", player.Id);
            command.Parameters.AddWithValue("@FullName", player.FullName);
            command.Parameters.AddWithValue("@Email", player.Email);
            command.Parameters.AddWithValue("@Phone", (object)player.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@BirthDate", (object)player.BirthDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", player.IsActive);
            command.Parameters.AddWithValue("@Rating", player.Rating);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        // DELETE
        public async Task<int> DeleteAsync(Guid id)
        {
            const string sql = "DELETE FROM Players WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }
    }
}