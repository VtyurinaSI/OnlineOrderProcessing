namespace OnlineOrder.Application.DTOs
{
    public record CreateOrderRequest(string CustomerName, List<OrderedItemDto> Items);
}
