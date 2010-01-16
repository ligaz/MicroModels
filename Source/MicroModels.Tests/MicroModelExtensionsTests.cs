using System;
using System.Windows.Input;
using MicroModels.Tests.Helpers;
using MicroModels.Tests.Sample;
using Moq;
using NUnit.Framework;

namespace MicroModels.Tests
{
    [TestFixture]
    public class MicroModelExtensionsTests
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
        public void CanDefineExplicitPropertyWithGetAndSet()
        {
            Model.Property("FirstName", () => Customer.FirstName, value => Customer.FirstName = value);
            Assert.IsNotNull(Model.GetPropertyDescriptor("FirstName"));
            Assert.IsFalse(Model.GetPropertyDescriptor("FirstName").IsReadOnly);
        }

        [Test]
        public void CanReadExplicitProperty()
        {
            Model.Property("FirstName", () => Customer.FirstName, value => Customer.FirstName = value);
            Assert.AreEqual("Paul", Model.ReadProperty<string>("FirstName"));
        }

        [Test]
        public void CanWriteExplicitProperty()
        {
            Model.Property("FirstName", () => Customer.FirstName, value => Customer.FirstName = value);
            Model.WriteProperty("FirstName", "Jack");
            Assert.AreEqual("Jack", Model.ReadProperty<string>("FirstName"));
            Assert.AreEqual("Jack", Customer.FirstName);
        }

        [Test]
        public void CanDefineExplicitReadOnlyProperty()
        {
            Model.Property("FullName", () => Customer.FirstName + " " + Customer.LastName);
            Assert.IsNotNull(Model.GetPropertyDescriptor("FullName"));
            Assert.IsTrue(Model.GetPropertyDescriptor("FullName").IsReadOnly);
        }

        [Test]
        public void CanReadExplicitReadOnlyProperty()
        {
            Model.Property("FullName", () => Customer.FirstName + " " + Customer.LastName);
            Assert.AreEqual("Paul Stovell", Model.ReadProperty<string>("FullName"));
        }

        [Test]
        public void CanDefineCommand()
        {
            Model.Command("Save", () => Repository.Save(Customer));
            Assert.IsNotNull(Model.GetPropertyDescriptor("Save"));
        }

        [Test]
        public void CanInvokeCommand()
        {
            Model.Command("Save", () => Repository.Save(Customer));
            Model.ReadProperty<ICommand>("Save").Execute(null);
            RepositoryMock.Verify(x => x.Save(Customer));
        }

        [Test]
        public void CanDefineCommandWithCanExecute()
        {
            Model.Command("Save", () => {}, () => false);
            Assert.IsNotNull(Model.GetPropertyDescriptor("Save"));
        }

        [Test]
        public void CanInvokeCanExecute()
        {
            Model.Command("Save", () => {}, () => false);
            Assert.IsFalse(Model.ReadProperty<ICommand>("Save").CanExecute(null));
        }

        [Test]
        public void CanDefineParameterizedCommand()
        {
            Model.Command<Customer>("Save", x => Repository.Save(x));
            Assert.IsNotNull(Model.GetPropertyDescriptor("Save"));
        }

        [Test]
        public void CanInvokeParameterizedCommand()
        {
            Model.Command<Customer>("Save", x => Repository.Save(x));
            Model.ReadProperty<ICommand>("Save").Execute(Customer);
            RepositoryMock.Verify(x => x.Save(Customer));
        }

        [Test]
        public void CanExposeAllProperties()
        {
            Model.AllProperties(Customer);
            Assert.AreEqual("Paul", Model.ReadProperty<string>("FirstName"));
            Assert.AreEqual("Stovell", Model.ReadProperty<string>("LastName"));
            Assert.AreEqual(DateTime.Today.AddYears(-23), Model.ReadProperty<DateTime>("DateOfBirth"));
        }

        [Test]
        public void CanAddLambdaProperty()
        {
            Model.Property(() => Customer.FirstName);
            Assert.IsNotNull(Model.GetPropertyDescriptor("FirstName"));
            Assert.IsFalse(Model.GetPropertyDescriptor("FirstName").IsReadOnly);
        }

        [Test]
        public void CanReadLambdaProperty()
        {
            Model.Property(() => Customer.FirstName);
            Assert.AreEqual("Paul", Model.ReadProperty<string>("FirstName"));
        }

        [Test]
        public void CanWriteLambdaProperty()
        {
            Model.Property(() => Customer.FirstName);
            Model.WriteProperty("FirstName", "Jack");
            Assert.AreEqual("Jack", Model.ReadProperty<string>("FirstName"));
        }
    }
}
