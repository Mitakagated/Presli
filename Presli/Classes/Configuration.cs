using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presli.Classes;
public static class Configuration
{
    private static readonly string? dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    private static readonly string? dbName = Environment.GetEnvironmentVariable("DB_NAME");
    private static readonly string? dbUser = Environment.GetEnvironmentVariable("DB_USER");
    private static readonly string? dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
    private static readonly string? dbPort = Environment.GetEnvironmentVariable("DB_PORT");
    public static string GetConnectionString()
    {
        return $"host={dbHost}; port={dbPort}; database={dbName}; user id={dbUser}; password={dbPassword}";
    }
}
