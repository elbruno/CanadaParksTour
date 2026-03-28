using Xunit;

namespace OntarioParksExplorer.Blazor.Tests.Pages;

public class ParkDetailPageTests
{
    [Fact]
    public void ParkDetail_ComponentExists()
    {
        // Verify the ParkDetail page component exists and can be referenced
        var pageType = typeof(OntarioParksExplorer.Blazor.Components.Pages.ParkDetail);
        Assert.NotNull(pageType);
        Assert.Equal("ParkDetail", pageType.Name);
    }
}
