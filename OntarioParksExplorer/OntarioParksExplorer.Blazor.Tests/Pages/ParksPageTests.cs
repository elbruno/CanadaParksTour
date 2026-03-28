using Xunit;

namespace OntarioParksExplorer.Blazor.Tests.Pages;

public class ParksPageTests
{
    [Fact]
    public void Parks_ComponentExists()
    {
        // Verify the Parks page component exists and can be referenced
        var pageType = typeof(OntarioParksExplorer.Blazor.Components.Pages.Parks);
        Assert.NotNull(pageType);
        Assert.Equal("Parks", pageType.Name);
    }
}
