using System.Collections.Generic;
using MicroModels;
using Sample.Invoicing.BusinessObjects;
using Sample.Invoicing.Services;

namespace Sample.Invoicing.Views
{
    public class InvoiceViewModel : MicroModel
    {
        public InvoiceViewModel(Order order, IEnumerable<LineItem> lineItems, IOrderService orderService)
        {
            AllProperties(order);

            Collection("LineItems", () => lineItems)
                .Each((item, model) => model.Property("LineTotal", () => item.UnitPrice * item.Quantity));

            Command("Save", () => orderService.Save(order, lineItems));
        }
    }
}
