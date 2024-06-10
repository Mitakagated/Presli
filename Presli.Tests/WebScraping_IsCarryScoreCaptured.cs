using Presli.Classes;

namespace Presli.Tests;

public class WebScraping_IsCarryScoreCaptured
{
    [Fact]
    public async Task IsWebScraping_ProperlyFindValues_ReturnTrue()
    {
        var webScrapingHelper = new WebScrapingHelper();

        var result = await webScrapingHelper.CarryScoreFinder(WebScrapingHelper.Regions.EUNE, "Seraphine", "EUNE");

        Assert.Equal(66, result);
    }
}