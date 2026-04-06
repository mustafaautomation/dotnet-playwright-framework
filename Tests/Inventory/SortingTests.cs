using NUnit.Framework;
using FluentAssertions;

namespace Quvantic.PlaywrightTests.Tests.Inventory;

[TestFixture]
[Category("regression")]
[Parallelizable(ParallelScope.Self)]
public class SortingTests : BaseTest
{
    [SetUp]
    public async Task LoginFirst()
    {
        await LoginAsStandardUser();
    }

    [Test]
    [Description("Sort by name A to Z")]
    public async Task SortByName_AtoZ()
    {
        await InventoryPage.SortBy("az");
        var name = await InventoryPage.GetFirstProductName();
        name.Should().NotBeNullOrEmpty();
    }

    [Test]
    [Description("Sort by name Z to A")]
    public async Task SortByName_ZtoA()
    {
        await InventoryPage.SortBy("za");
        var name = await InventoryPage.GetFirstProductName();
        name.Should().NotBeNullOrEmpty();
    }

    [Test]
    [Description("Sort by price low to high")]
    public async Task SortByPrice_LowToHigh()
    {
        await InventoryPage.SortBy("lohi");
        var price = await InventoryPage.GetFirstProductPrice();
        price.Should().Contain("$");
    }

    [Test]
    [Description("Sort by price high to low")]
    public async Task SortByPrice_HighToLow()
    {
        await InventoryPage.SortBy("hilo");
        var price = await InventoryPage.GetFirstProductPrice();
        price.Should().Contain("$");
    }

    [Test]
    [Description("All products visible after sort")]
    public async Task AllProducts_VisibleAfterSort()
    {
        await InventoryPage.SortBy("lohi");
        var count = await InventoryPage.GetItemCount();
        count.Should().Be(6);

        await InventoryPage.SortBy("za");
        count = await InventoryPage.GetItemCount();
        count.Should().Be(6);
    }
}
