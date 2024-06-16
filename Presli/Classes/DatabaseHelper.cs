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
        return acc!;
    }

    public static async Task<long> AddMito(ulong discordId, long mito)
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
            acc!.Mito += mito;
            await db.SaveChangesAsync().ConfigureAwait(false);
            return acc.Mito;
        }
    }

    public static async Task<long> RemoveMito(ulong discordId, long mito)
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
            if (acc!.Mito - mito <= 0)
            {
                await db.SaveChangesAsync().ConfigureAwait(false);
                return acc.Mito - mito;
            }
            acc.Mito -= mito;
            await db.SaveChangesAsync().ConfigureAwait(false);
            return acc.Mito;
        }
    }

    public static async Task<long> AddBettingCurrency(ulong discordId, long bettingCurrency)
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
            acc!.BettingCurrency += bettingCurrency;
            await db.SaveChangesAsync().ConfigureAwait(false);
            return acc.BettingCurrency;
        }
    }

    public static async Task<long> RemoveBettingCurrency(ulong discordId, long bettingCurrency)
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
            if (acc!.BettingCurrency - bettingCurrency <= 0)
            {
                await db.SaveChangesAsync().ConfigureAwait(false);
                return acc.BettingCurrency - bettingCurrency;
            }
            acc.BettingCurrency -= bettingCurrency;
            await db.SaveChangesAsync().ConfigureAwait(false);
            return acc.BettingCurrency;
        }
    }

    public static async Task MitoRestore(ulong discordId)
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
            if (acc!.Mito <= 350)
            {
                acc.Mito = 350;
            }
            await db.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public static async Task BettingCurrencyRestore(ulong discordId)
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
            if (acc!.BettingCurrency <= 350)
            {
                acc.BettingCurrency = 350;
            }
            await db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
