using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApp.Models;

namespace WebApp.Checkers
{
    public class ActiveUsersChecker : IHealthCheck
    {
        private readonly AcuWebSite _acuWebSite;

        public ActiveUsersChecker(AcuWebSite acuWebSite)
        {
            _acuWebSite = acuWebSite;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new ())
        {
            try
            {
                var activeUsers = await GetActiveUsersAsync();
                return HealthCheckResult.Healthy(BuildDescription(activeUsers));
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }
        }

        private static string BuildDescription(ICollection<AcuUser> activeUsers)
        {
            var description = $"{activeUsers.Select(x => x.Username).Distinct().Count()} active user(s) in last hour";

            if (activeUsers.Any())
            {
                description += " : " + string.Join(" ", activeUsers.Select(x => $"{x.Username} ({x.LastActivityDate:HH:mm})"));
            }

            return description;
        }

        private async Task<ICollection<AcuUser>> GetActiveUsersAsync()
        {
            const string sqlGetActiveUsers = @"SELECT TOP(100) Username, FirstName, LastName, LastActivityDate FROM Users 
                                               WHERE LastActivityDate >= DATEADD(hour, -1, SYSDATETIMEOFFSET())
                                               ORDER BY LastActivityDate DESC";
            await using var connection = new SqlConnection(_acuWebSite.ConnectionString);
            var activeUsers = await connection.QueryAsync<AcuUser>(sqlGetActiveUsers);
            return activeUsers.ToList();
        }
    }
}
