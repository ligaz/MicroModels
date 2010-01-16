using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace MicroModels.Tests.Assumptions
{
    [TestFixture]
    public class ExpressionAssumptionTests
    {
        private Expression<Func<string>> GetCustomerName(Customer customer)
        {
            return () => customer.FirstName + " " + customer.LastName;
        }

        // The idea here is that the same expression always generates the same looking graph, with the only 
        // difference being that constants will change. This allows us to make assumptions such as if we 
        // already parsed an expression looking for property names, we can cache it by .ToString(), to avoid 
        // parsing it again (we'd still want to compile it each time).
        [Test]
        public void ExpressionsCreatedFromTheSameLambdaHaveSameToStringAndAreThereforeInterchangeable()
        {
            var c1 = new Customer() {FirstName = "Paul", LastName = "Stovell"};
            var c2 = new Customer() {FirstName = "Darren", LastName = "Niemke"};
            var ex1 = GetCustomerName(c1);
            var ex2 = GetCustomerName(c2);

            var s1 = ex1.ToString();
            var s2 = ex2.ToString();
            Assert.AreEqual(s1, s2);
            Assert.IsTrue(s1.Contains("() => "));
            Assert.IsTrue(s1.Contains("customer.FirstName"));
            Assert.IsTrue(s1.Contains("customer.LastName"));
        }
    }
}
