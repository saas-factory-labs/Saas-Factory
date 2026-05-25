using AppBlueprint.UiKit.Components.Layout;
using Bunit;
using FluentAssertions;
using TUnit;
using BunitTestContext = Bunit.TestContext;

namespace AppBlueprint.Tests.Blazor;

public sealed class SidebarMenuToggleTests
{
    [Test]
    public void SidebarDoesNotRenderEllipsisForSectionHeadings()
    {
        using var context = new BunitTestContext();

        var cut = context.RenderComponent<Sidebar>(parameters =>
            parameters.Add(p => p.SidebarOpen, true));

        cut.Markup.Should().NotContain("•••");
    }

    [Test]
    public void SidebarRendersExpandToggleButton()
    {
        using var context = new BunitTestContext();

        var cut = context.RenderComponent<Sidebar>(parameters =>
            parameters.Add(p => p.SidebarOpen, true));

        var buttons = cut.FindAll("button[data-sidebar-toggle]");
        buttons.Should().NotBeEmpty();
    }
}
