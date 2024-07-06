using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MyWebApp.Models;

namespace MyWebApp.Services
{
    public class AuditLogService
    {
        private readonly string _connectionString;

        public AuditLogService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<SelectLog>> GetSelectLogsAsync()
        {
            var logs = new List<SelectLog>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"SELECT 
                                event_time AS EventTime,
                                database_name AS DatabaseName,
                                object_name AS ObjectName,
                                statement AS Statement
                              FROM sys.fn_get_audit_file ('C:\Temp\Audit_Selects*.sqlaudit', DEFAULT, DEFAULT)
                              WHERE statement LIKE 'SELECT%'";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        logs.Add(new SelectLog
                        {
                            EventTime = reader.GetDateTime(0),
                            DatabaseName = reader.GetString(1),
                            ObjectName = reader.GetString(2),
                            Statement = reader.GetString(3)
                        });
                    }
                }
            }

            return logs;
        }
    }
}
