namespace OnlineOrder.Domain
{
    /// <summary>Тригеры для управления состоянием заказа</summary>
    public enum OrderTrigger
    {
        /// <summary>Оплата: переход из состояния "Создан" в "Оплачен"</summary>
        Pay,   
        /// <summary>Доставка: переход из состояния "Оплачен" в "Доставлен"</summary>
        Deliver,   
        /// <summary>Отмена: переход из состояния "Создан" или "Оплачен" в "Отменён"</summary>
        Cancel,   
        /// <summary>Ошибка: переход из любого состояния в "Ошибка"</summary>
        Fail       
    }
}
