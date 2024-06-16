using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Presli.Classes;
public class WebScrapingHelper
{
    private string? _previousGame;
    public DateTime _lastSearched;
    private readonly string[] _regions = { "na1", "euw1", "eun1", "kr", "br1", "jp1", "ru", "oc1", "tr1", "la1", "la2", "ph2", "sg2", "th2", "tw2", "vn2" };
    public enum Regions
    {
        NA,
        EUW,
        EUNE,
        KR,
        BR,
        JP,
        RU,
        OCE,
        TR,
        LAN,
        LAS,
        PH,
        SG,
        TH,
        TW,
        VN
    }
    public async Task<int> CarryScoreFinder(Regions region, string summonerName, string riotID)
    {
        var page = await InitializeUGGBrowser(region, summonerName, riotID).ConfigureAwait(false);

        var rowLocator = page.Locator("div.row-three").Nth(0);
        var result = await rowLocator
            .Filter(new() { HasTextRegex = new Regex("[A-Z]{3,4}") })
            .Filter(new() { HasTextRegex = new Regex("(\\d)+:(\\d)+") })
            .InnerTextAsync()
            .ConfigureAwait(false);

        if (_previousGame == null)
        {
            _previousGame = result;
            _lastSearched = DateTime.UtcNow;
        }
        else if (result != _previousGame)
        {
            _previousGame = result;
            await rowLocator.ClickAsync().ConfigureAwait(false);
            var carryScoreLocator = page.Locator("div.is-profile-owner_loss").Or(page.Locator("div.is-profile-owner_win"));
            var carryScore = await carryScoreLocator.Locator("div.carry-score").InnerTextAsync().ConfigureAwait(false);
            return int.Parse(carryScore);
        }

        _lastSearched = DateTime.UtcNow;
        return 0;
    }

    private async Task<IPage> InitializeBrowser()
    {
        var playwright = await Playwright.CreateAsync().ConfigureAwait(false);
        var browser = await playwright.Firefox.LaunchAsync().ConfigureAwait(false);
        var page = await browser.NewPageAsync().ConfigureAwait(false);

        return page;
    }

    private async Task<IPage> InitializeUGGBrowser(Regions region, string summonerName, string riotID)
    {
        var playwright = await Playwright.CreateAsync().ConfigureAwait(false);
        var browser = await playwright.Firefox.LaunchAsync().ConfigureAwait(false);
        var page = await browser.NewPageAsync().ConfigureAwait(false);

        await page.GotoAsync($"https://u.gg/lol/profile/{_regions[(int)region]}/{summonerName}-{riotID}/overview").ConfigureAwait(false);
        await page.GetByRole(AriaRole.Button, new() { Name = "Consent" }).ClickAsync().ConfigureAwait(false);
        await page.GetByText("Update").ClickAsync().ConfigureAwait(false);

        return page;
    }
}
