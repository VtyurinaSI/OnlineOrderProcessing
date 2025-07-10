namespace OnlineOrder.Domain.DTOs
{
    public record OrderStateChange
    (
        OrderTrigger Trigger,
        OrderStatus From,
        OrderStatus To,
        DateTime ChangedAt
    );
}
