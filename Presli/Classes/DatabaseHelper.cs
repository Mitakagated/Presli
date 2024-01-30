using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DSharpPlus.SlashCommands;
using Npgsql;
using Presli.Models;

namespace Presli.Classes;
public static class DatabaseHelper
{
    private static readonly IConfiguration configuration = new Configuration();
    public static async Task<CurrencyInfo> GetCurrencyInfo(InteractionContext ctx)
    {
        CurrencyInfo? acc;
        var discordId = new { Id = (long)ctx.User.Id };
        using (var db = new NpgsqlConnection(configuration.GetConnectionString()))
        {
            acc = await db.QuerySingleOrDefaultAsync<CurrencyInfo>("SELECT * FROM currencyinfo WHERE discord_id = @Id", discordId);
            if (acc is null)
            {
                await db.ExecuteAsync("INSERT INTO currencyinfo (discord_id, mito_currency, betting_currency) VALUES (@Id, 500, 500)", discordId);
                acc = await db.QuerySingleAsync<CurrencyInfo>("SELECT * FROM currencyinfo WHERE discord_id = @Id", discordId);
            }
        }
        return acc;
    }
}
