using Bunit;
using MudBlazor.Services;
using OntarioParksExplorer.Blazor.Components.Pages;
using Xunit;

namespace OntarioParksExplorer.Blazor.Tests.Pages;

public class HomePageTests : TestContext
{
    public HomePageTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        ComponentFactories.AddStub<OntarioParksExplorer.Blazor.Components.Shared.RecommendationsWidget>();
    }

    [Fact]
    public void Home_RendersSuccessfully()
    {
        // Act
        var cut = Render<Home>();

        // Assert - component renders without exceptions
        Assert.NotNull(cut);
    }

    [Fact]
    public void Home_ContainsBrowseParksButton()
    {
        // Act
        var cut = Render<Home>();

        // Assert
        Assert.Contains("Browse Parks", cut.Markup);
    }

    [Fact]
    public void Home_ContainsViewMapButton()
    {
        // Act
        var cut = Render<Home>();

        // Assert
        Assert.Contains("View Map", cut.Markup);
    }

    [Fact]
    public void Home_ContainsAiChatButton()
    {
        // Act
        var cut = Render<Home>();

        // Assert
        Assert.Contains("AI Chat", cut.Markup);
    }

    [Fact]
    public void Home_ContainsWelcomeText()
    {
        // Act
        var cut = Render<Home>();

        // Assert
        Assert.Contains("Welcome to Ontario Parks Explorer", cut.Markup);
    }
}
