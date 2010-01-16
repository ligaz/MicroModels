using System;
using System.Collections.Generic;
using Sample.Invoicing.BusinessObjects;

namespace Sample.Invoicing.Services
{
    public class OrderService : IOrderService
    {
        public Order GetOrder(int orderId)
        {
            return new Order
            {
                Id = orderId,
                OrderDate = DateTime.Today,
                CustomerName = "Customer " + orderId,
                CustomerAddress = orderId + " Smith St"
            };
        }

        public LineItem[] GetLineItems(int orderId)
        {
            return new[]
            {
                new LineItem() {ProductName = "Bar of Soap", Quantity = 2, UnitPrice = 1.45M},
                new LineItem() {ProductName = "Wooden Spoon", Quantity = 1, UnitPrice = 2.15M},
                new LineItem() {ProductName = "Oranges", Quantity = 5, UnitPrice = 0.45M},
                new LineItem() {ProductName = "Frozen Pizza", Quantity = 1, UnitPrice = 4.25M},
            };
        }

        public void Save(Order order, IEnumerable<LineItem> lineItems)
        {
            Console.WriteLine("Order saved: {0}", order.Id);
        }
    }
}


