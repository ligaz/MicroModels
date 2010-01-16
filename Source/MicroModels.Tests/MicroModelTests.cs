using System.Linq;
using MicroModels.Description;
using MicroModels.Extensions;
using NUnit.Framework;

namespace MicroModels.Tests
{
    [TestFixture]
    public class MicroModelTests
    {
        protected IMicroModel Model { get; set; }
        
        [SetUp]
        public void SetUp()
        {
            Model = new MicroModel();
        }

        [Test] 
        public void HasTypeValidationExtension()
        {
            ((ISelfDescribing)Model).SealTypeDescription();
            Assert.AreEqual(1, Model.GetExtensions().Count());
            Assert.IsTrue(Model.GetExtensions().ElementAt(0) is TypeValidationExtensionAttribute);
        }
    }
}
