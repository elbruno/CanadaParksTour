using Xunit;

namespace OntarioParksExplorer.Blazor.Tests.Pages;

public class FavoritesPageTests
{
    [Fact]
    public void Favorites_ComponentExists()
    {
        // Verify the Favorites page component exists and can be referenced
        var pageType = typeof(OntarioParksExplorer.Blazor.Components.Pages.Favorites);
        Assert.NotNull(pageType);
        Assert.Equal("Favorites", pageType.Name);
    }
}
