using System.Collections.Generic;
using Sample.Invoicing.BusinessObjects;

namespace Sample.Invoicing.Services
{
    public interface IOrderService
    {
        Order GetOrder(int orderId);
        LineItem[] GetLineItems(int orderId);
        void Save(Order order, IEnumerable<LineItem> lineItems);
    }
}