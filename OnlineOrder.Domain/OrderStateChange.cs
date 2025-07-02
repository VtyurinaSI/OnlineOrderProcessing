namespace OnlineOrder.Domain
{
    public record OrderStateChange
    (
        OrderTrigger Trigger,
        OrderStatus From,
        OrderStatus To,
        DateTime ChangedAt
    );
}
