using Dapper;
using Npgsql;
using OnlineOrder.Domain;
using OnlineOrder.Domain.DTOs;

namespace OnlineOrder.Infrastructure
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _cs = DbConfig.SqlConnectionString;
        private NpgsqlConnection CreateConn() => new(_cs);
        public async Task<Order?> GetAsync(Guid id)
        {
            const string sql = @"
              SELECT 
                id, customer_name AS CustomerName, amount, status AS Status,
                created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM orders
              WHERE id = @Id;
            ";
            using var db = CreateConn();
            await db.OpenAsync();
            using var tx = db.BeginTransaction();
            var ord = await db.QuerySingleOrDefaultAsync<Order>(sql, new { Id = id });
            return ord;
        }

        public async Task InsertAsync(Order order)
        {

            const string insOrder = @"
              INSERT INTO orders(id, customer_name, amount, status, created_at, updated_at)
              VALUES(@Id, @CustomerName, @Amount, @Status, @CreatedAt, @UpdatedAt);
            ";
            const string insHist = @"
              INSERT INTO order_state_history(order_id,from_status,to_status,trigger,changed_at)
              VALUES(@OrderId, @From, @To, @Trigger, @ChangedAt);
            ";

            using var db = CreateConn();
            await db.OpenAsync();
            using var tx = db.BeginTransaction();
            await db.ExecuteAsync(insOrder, new
            {
                order.Id,
                order.CustomerName,
                order.Amount,
                Status = order.Status.ToString(),
                order.CreatedAt,
                order.UpdatedAt
            }, tx);

            foreach (var h in order.History)
            {
                await db.ExecuteAsync(insHist, new
                {
                    OrderId = order.Id,
                    From = h.From.ToString(),
                    To = h.To.ToString(),
                    Trigger = h.Trigger.ToString(),
                    ChangedAt = h.ChangedAt
                }, tx);
            }
            tx.Commit();
        }

        public async Task UpdateAsync(Order order)
        {
            const string updOrder = @"
      UPDATE orders
         SET amount     = @Amount,
             status     = @Status,
             updated_at = @UpdatedAt
       WHERE id = @Id;
    ";
            const string insHist = @"
      INSERT INTO order_state_history(
        order_id, from_status, to_status, trigger, changed_at
      )
      VALUES(
        @OrderId, @From, @To, @Trigger, @ChangedAt
      );
    ";

            using var db = CreateConn();
            await db.OpenAsync();
            using var tx = db.BeginTransaction();

            await db.ExecuteAsync(updOrder, new
            {
                order.Amount,
                Status = order.Status.ToString(),
                order.UpdatedAt,
                order.Id
            }, tx);

            foreach (var h in order.History)
            {
                await db.ExecuteAsync(insHist, new
                {
                    OrderId = order.Id,
                    From = h.From.ToString(),
                    To = h.To.ToString(),
                    Trigger = h.Trigger.ToString(),
                    h.ChangedAt
                }, tx);
            }

            tx.Commit();
        }

        public async Task<IEnumerable<OrderStateChange>> GetHistoryAsync(Guid orderId)
        {

            const string sql = @"
              SELECT
                  trigger     AS Trigger,
                  from_status AS From,
                  to_status   AS To,
                  changed_at  AS ChangedAt
                FROM order_state_history
                WHERE order_id = @OrderId
                ORDER BY changed_at;
            ";
            using var db = CreateConn();
            await db.OpenAsync();
            using var tx = db.BeginTransaction();
            var list = await db.QueryAsync<OrderStateChange>(sql, new { OrderId = orderId });
            return list.ToList();
        }
    }
}
