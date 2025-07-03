using OnlineOrder.Domain;
using OnlineOrder.Infrastructure;

namespace ConsoleDbTesting
{
    internal class Program
    {
        private static OrderRepository repo = null!;
        private static Order? order;
        static async Task Main()
        {

            await MakeOrder();
            await Pay();

            await FindOrder();
            if (order is null) return;

            await TakeHistory();

            await Deliv();
            await TakeHistory();
        }
        private static async Task Deliv()
        {
            await Task.Delay(2000);
            order!.Deliver();
            await repo.UpdateAsync(order);

        }
        private static async Task TakeHistory()
        {
            var history = await repo.GetHistoryAsync(order!.Id);
            Console.WriteLine("History:");
            foreach (var h in history)
            {
                Console.WriteLine($"{h.From} -> {h.To} by {h.Trigger} at {h.ChangedAt}");
            }
        }
        private static async Task FindOrder()
        {
            var loaded = await repo.GetAsync(order!.Id);
            if (loaded == null)
                Console.WriteLine("Order not found!");

            else
                Console.WriteLine($"Loaded order {loaded.Id}, customer: {loaded.CustomerName}, status = {loaded.Status}, amount = {loaded?.Amount}");

        }
        private static async Task Pay()
        {
            order!.Pay();
            await repo.UpdateAsync(order);
        }
        private static async Task MakeOrder()
        {
            repo = new();
            order = new("Соня");
            order.AddItem(new("Колбаса", 150m, 10));
            await repo.InsertAsync(order);
            Console.WriteLine($"Inserted order {order.Id}, status = {order.Status}, amount = {order?.Amount}");
        }
    }
}
