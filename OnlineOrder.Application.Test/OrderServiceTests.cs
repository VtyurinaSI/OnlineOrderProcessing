using FluentAssertions;
using Moq;
using OnlineOrder.Application;
using OnlineOrder.Application.Contracts;
using OnlineOrder.Application.DTOs;
using OnlineOrder.Domain;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repo = new();
    private readonly OrderService _svc;

    public OrderServiceTests() => _svc = new OrderService(_repo.Object);

    [Fact]
    public async Task CreateOrderAsync_valid_request_persists_order()
    { 
        var req = new CreateOrderRequest("Bob",
            new List<OrderedItemDto> { new("Pizza", 10m, 2) });
         
        var order = await _svc.CreateOrderAsync(req, CancellationToken.None);
         
        _repo.Verify(r => r.InsertAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        order.CustomerName.Should().Be("Bob");
        order.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateOrderAsync_invalid_request_throws()
    {
        var bad = new CreateOrderRequest("", new());
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            () => _svc.CreateOrderAsync(bad, CancellationToken.None));
        _repo.VerifyNoOtherCalls();                   
    }

    [Fact]
    public async Task PayAsync_order_exists_and_transition_ok_updates_repo()
    {
        var order = new Order("Alice");
        _repo.Setup(r => r.GetAsync(order.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(order);

        var ok = await _svc.PayAsync(order.Id, CancellationToken.None);

        ok.Should().BeTrue();
        _repo.Verify(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_order_not_found_returns_false()
    {
        _repo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Order?)null);

        var ok = await _svc.CancelAsync(Guid.NewGuid(), CancellationToken.None);

        ok.Should().BeFalse();
        _repo.Verify(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
