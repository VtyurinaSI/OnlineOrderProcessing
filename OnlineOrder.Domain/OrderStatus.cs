using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineOrder.Domain
{
    /// <summary>Статус заказа</summary>
    public enum OrderStatus
    {
        /// <summary>Создан</summary>
        Created,
        /// <summary>Оплачен</summary>
        Paid,
        /// <summary>Доставлен</summary>
        Delivered,
        /// <summary>Отменен</summary>
        Cancelled,
        /// <summary>Ошибка</summary>
        Error
    }
}
