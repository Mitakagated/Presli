using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Presli.Models;

namespace Presli.Classes;
public static class DatabaseHelper
{
    public static async Task<CurrencyInfo> GetCurrencyInfo(ulong discordId)
    {
        CurrencyInfo? acc;
        using (var db = new CurrencyInfoContext())
        {
            acc = await db.CurrencyInfos.FindAsync(discordId).ConfigureAwait(false);
            if (acc is null)
            {
                await db.AddAsync(new CurrencyInfo { DiscordId = discordId, BettingCurrency = 500, Mito = 500 }).ConfigureAwait(false);
                await db.SaveChangesAsync();
                acc = await db.CurrencyInfos.FindAsync(discordId).ConfigureAwait(false);
            }
        }
        return acc;
    }
}
