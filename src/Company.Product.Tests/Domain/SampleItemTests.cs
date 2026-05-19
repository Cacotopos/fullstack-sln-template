using Company.Product.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Company.Product.Tests.Domain;

public sealed class SampleItemTests
{
    [Fact]
    public void Create_ShouldSetProperties_WhenValid()
    {
        var userId = "user-123";
        var name = "Test Item";
        var description = "Test Description";

        var item = SampleItem.Create(name, description, userId);

        item.Id.Should().NotBeEmpty();
        item.Name.Should().Be(name);
        item.Description.Should().Be(description);
        item.CreatedByUserId.Should().Be(userId);
        item.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        item.UpdatedAt.Should().Be(item.CreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenNameIsInvalid(string name)
    {
        var act = () => SampleItem.Create(name, "desc", "user-123");
        act.Should().Throw<ArgumentException>().WithParameterName(nameof(name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenUserIdIsInvalid(string userId)
    {
        var act = () => SampleItem.Create("Name", "desc", userId);
        act.Should().Throw<ArgumentException>().WithParameterName(nameof(userId));
    }

    [Fact]
    public void Update_ShouldChangeNameAndDescription_WhenValid()
    {
        var item = SampleItem.Create("Old", "OldDesc", "user-123");
        var newName = "New Name";
        var newDesc = "New Description";

        item.Update(newName, newDesc);

        item.Name.Should().Be(newName);
        item.Description.Should().Be(newDesc);
        item.UpdatedAt.Should().BeAfter(item.CreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_ShouldThrow_WhenNameIsInvalid(string name)
    {
        var item = SampleItem.Create("Old", "OldDesc", "user-123");
        var act = () => item.Update(name, "desc");
        act.Should().Throw<ArgumentException>().WithParameterName(nameof(name));
    }
}
