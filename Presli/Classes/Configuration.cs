using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presli.Classes;
public class Configuration : IConfiguration
{
    private readonly string? dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
    private readonly string? dbName = Environment.GetEnvironmentVariable("DB_NAME");
    private readonly string? dbUser = Environment.GetEnvironmentVariable("DB_USER");
    private readonly string? dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
    public string GetConnectionString()
    {
        return $"Server={dbServer};Port=5432;Database={dbName};User Id={dbUser};Password={dbPassword};";
    }
}
