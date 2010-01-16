using System;

namespace Sample.CustomerEditor
{
    public class CustomerRepository
    {
        public void Save(Customer customer)
        {
            Console.WriteLine("Saving customer:");
            Console.WriteLine("  First name: {0}", customer.FirstName);
            Console.WriteLine("  Last name: {0}", customer.LastName);
        }
    }
}
