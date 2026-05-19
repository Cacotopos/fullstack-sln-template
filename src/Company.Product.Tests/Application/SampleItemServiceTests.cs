using Company.Product.Application.Interfaces;
using Company.Product.Application.Services;
using Company.Product.Contracts.SampleItems;
using Company.Product.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Company.Product.Tests.Application;

public sealed class SampleItemServiceTests
{
    private readonly Mock<ISampleItemRepository> _repo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ICurrentUserService> _user = new();
    private readonly SampleItemService _service;

    public SampleItemServiceTests()
    {
        _user.Setup(x => x.UserId).Returns("user-123");
        _service = new SampleItemService(_repo.Object, _uow.Object, _user.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnDtos()
    {
        var items = new List<SampleItem>
        {
            SampleItem.Create("A", "DescA", "user-123"),
            SampleItem.Create("B", "DescB", "user-123")
        };
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(items);

        var result = await _service.GetAllAsync(TestContext.Current.CancellationToken);

        result.Should().HaveCount(2);
        result.First().Name.Should().Be("A");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDto_WhenExists()
    {
        var item = SampleItem.Create("X", "DescX", "user-123");
        _repo.Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>())).ReturnsAsync(item);

        var result = await _service.GetByIdAsync(item.Id, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Name.Should().Be("X");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenMissing()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((SampleItem?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldAddAndSave()
    {
        var request = new CreateSampleItemRequest("New", "NewDesc");

        var result = await _service.CreateAsync(request, TestContext.Current.CancellationToken);

        _repo.Verify(r => r.AddAsync(It.Is<SampleItem>(x => x.Name == "New"), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        result.Name.Should().Be("New");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdate_WhenExists()
    {
        var item = SampleItem.Create("Old", "OldDesc", "user-123");
        _repo.Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>())).ReturnsAsync(item);
        var request = new UpdateSampleItemRequest("Updated", "UpdatedDesc");

        var result = await _service.UpdateAsync(item.Id, request, TestContext.Current.CancellationToken);

        _repo.Verify(r => r.UpdateAsync(It.Is<SampleItem>(x => x.Name == "Updated"), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        result!.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenExists()
    {
        var item = SampleItem.Create("Del", "DelDesc", "user-123");
        _repo.Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>())).ReturnsAsync(item);
        _repo.Setup(r => r.DeleteAsync(item.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _service.DeleteAsync(item.Id, TestContext.Current.CancellationToken);

        result.Should().BeTrue();
        _repo.Verify(r => r.DeleteAsync(item.Id, It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
