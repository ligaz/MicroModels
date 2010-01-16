using MicroModels;

namespace Sample.CustomerEditor
{
    public class EditCustomerModel : MicroModel
    {
        public EditCustomerModel(Customer customer, CustomerRepository customerRepository)
        {
            Property(() => customer.FirstName);
            Property(() => customer.LastName).Named("Surname");
            Property("FullName", () => string.Format("{0} {1}", customer.FirstName, customer.LastName));
            Command("Save", () => customerRepository.Save(customer));
        }
    }
}
