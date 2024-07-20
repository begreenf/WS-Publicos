using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MyWebApp.Services
{
    public class QueryLogService
    {
        private readonly string _connectionString;

        public QueryLogService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task LogQueryAsync(string queryType)
        {
            string tableName = queryType switch
            {
                "exchange-rate" => "ExchangeRateLog",
                "inflation-index" => "InflationIndexLog",
                "financial-health" => "FinancialHealthLog",
                "credit-history" => "CreditHistoryLog",
                _ => throw new ArgumentException("Invalid query type")
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = $"INSERT INTO {tableName} (Timestamp) VALUES (GETDATE())";
                using (var command = new SqlCommand(query, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<Dictionary<string, int>> GetQueryCountsAsync()
        {
            var counts = new Dictionary<string, int>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                counts["exchange-rate"] = await GetCountAsync(connection, "ExchangeRateLog");
                counts["inflation-index"] = await GetCountAsync(connection, "InflationIndexLog");
                counts["financial-health"] = await GetCountAsync(connection, "FinancialHealthLog");
                counts["credit-history"] = await GetCountAsync(connection, "CreditHistoryLog");
            }

            return counts;
        }

        public async Task<Dictionary<string, int>> GetQueryCountsByDateAsync(DateTime startDate, DateTime endDate)
        {
            var counts = new Dictionary<string, int>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                counts["exchange-rate"] = await GetCountByDateAsync(connection, "ExchangeRateLog", startDate, endDate);
                counts["inflation-index"] = await GetCountByDateAsync(connection, "InflationIndexLog", startDate, endDate);
                counts["financial-health"] = await GetCountByDateAsync(connection, "FinancialHealthLog", startDate, endDate);
                counts["credit-history"] = await GetCountByDateAsync(connection, "CreditHistoryLog", startDate, endDate);
            }

            return counts;
        }

        private async Task<int> GetCountAsync(SqlConnection connection, string tableName)
        {
            var query = $"SELECT COUNT(*) FROM {tableName}";
            using (var command = new SqlCommand(query, connection))
            {
                return (int)await command.ExecuteScalarAsync();
            }
        }

        private async Task<int> GetCountByDateAsync(SqlConnection connection, string tableName, DateTime startDate, DateTime endDate)
        {
            var query = $"SELECT COUNT(*) FROM {tableName} WHERE Timestamp >= @StartDate AND Timestamp < @EndDate";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate.AddDays(1)); 
                return (int)await command.ExecuteScalarAsync();
            }
        }
    }
}
