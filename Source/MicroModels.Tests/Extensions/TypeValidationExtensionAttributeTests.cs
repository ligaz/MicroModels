using MicroModels.Extensions;
using NUnit.Framework;
using MicroModels.Description;
using System;

namespace MicroModels.Tests.Extensions
{
    [TestFixture]
    public class TypeValidationExtensionAttributeTests
    {
        public class SampleModel : MicroModelBase
        {
        }

        public IMicroModel Model { get; set;}

        [SetUp]
        public void SetUp()
        {
            Model = new SampleModel();
            Model.AddExtension(new TypeValidationExtensionAttribute());
        }

        [Test]
        public void CannotSpecifyPropertiesWithSameName()
        {
            Model.Property("FirstName", () => "Hello");
            Model.Property("FirstName", () => "Goodbye");
            Assert.Throws<InvalidOperationException>(() => ((ISelfDescribing) Model).SealTypeDescription());
        }

        [Test]
        public void ValidationTakesPlaceDuringApply()
        {
            Model.Property("FirstName", () => "Hello");
            Model.Property("FirstName", () => "Goodbye").WithPrefix("X");
            ((ISelfDescribing)Model).SealTypeDescription();
        }
    }
}
