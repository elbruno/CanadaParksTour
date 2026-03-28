using Xunit;

namespace OntarioParksExplorer.Blazor.Tests.Layout;

public class MainLayoutTests
{
    [Fact]
    public void MainLayout_ComponentExists()
    {
        // Verify the MainLayout component exists and can be referenced
        var layoutType = typeof(OntarioParksExplorer.Blazor.Components.Layout.MainLayout);
        Assert.NotNull(layoutType);
        Assert.Equal("MainLayout", layoutType.Name);
    }
}
