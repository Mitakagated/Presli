using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Presli.Classes;

namespace Presli.Models;
public class CurrencyInfoContext : DbContext
{
    public DbSet<CurrencyInfo> CurrencyInfos { get; set; }
    public string ConString { get; }
    public CurrencyInfoContext()
    {
        ConString = Configuration.GetConnectionString();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql($"host = localhost; port = 5432; database = postgres; user id = postgres; password = LoLTheBest56");
    }
}
