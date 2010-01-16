using System;
using MicroModels.Tests.Helpers;
using MicroModels.Tests.Sample;
using Moq;
using NUnit.Framework;

namespace MicroModels.Tests
{
    [TestFixture]
    public class PropertyDefinitionExtensionsTests
    {
        protected Customer Customer { get; set; }
        protected MicroModel Model { get; set; }
        protected Mock<ICustomerRepository> RepositoryMock { get; set; }
        protected ICustomerRepository Repository { get; set; }
        
        [SetUp]
        public void SetUp()
        {
            Customer = new Customer();
            Customer.FirstName = "Paul";
            Customer.LastName = "Stovell";
            Customer.DateOfBirth = DateTime.Today.AddYears(-23);
            Model = new MicroModel();
            RepositoryMock = new Mock<ICustomerRepository>();
            Repository = RepositoryMock.Object;
        }

        [Test]
        public void CanPrefixProperty()
        {
            Model.Property("FirstName", () => Customer.FirstName).WithPrefix("Customer");
            Assert.AreEqual("Paul", Model.ReadProperty<string>("CustomerFirstName"));
        }

        [Test]
        public void CanPrefixProperties()
        {
            Model.AllProperties(Customer).WithPrefix("Customer");
            Assert.AreEqual("Paul", Model.ReadProperty<string>("CustomerFirstName"));
            Assert.AreEqual("Stovell", Model.ReadProperty<string>("CustomerLastName"));
            Assert.AreEqual(DateTime.Today.AddYears(-23), Model.ReadProperty<DateTime>("CustomerDateOfBirth"));
        }

        [Test]
        public void CanExcludePropertiesFromAllProperties()
        {
            Model.AllProperties(Customer).Excluding(x => x.Name.StartsWith("First"));
            Assert.IsNull(Model.GetPropertyDescriptor("FirstName", false));
            Assert.IsNotNull(Model.GetPropertyDescriptor("LastName"));
        }

        [Test]
        public void CanForceReadOnlyProperties()
        {
            Model.Property(() => Customer.FirstName);
            Model.Property(() => Customer.LastName).ReadOnly();
            Assert.IsFalse(Model.GetPropertyDescriptor("FirstName").IsReadOnly);
            Assert.IsTrue(Model.GetPropertyDescriptor("LastName").IsReadOnly);
        }

        [Test]
        public void CanRenameProperties()
        {
            Model.Property(() => Customer.FirstName).Named("GivenName");
            Assert.IsNotNull(Model.GetPropertyDescriptor("GivenName"));
        }
    }
}
