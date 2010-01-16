using System;
using System.Collections.Generic;
using System.Linq;
using MicroModels.Description;
using MicroModels.Extensions;
using Moq;
using NUnit.Framework;

namespace MicroModels.Tests
{
    [TestFixture]
    public class MicroModelBaseTests
    {
        #region SUT

        public class SampleModel : MicroModelBase
        {
        }

        public class SampleExtensionAttribute : Attribute, IModelExtension
        {
            public int Priority
            {
                get { return 10; }
            }

            public void Apply(IMicroModel model)
            {
                model.AddProperty(new Mock<IPropertyDefinition>().Object);
            }
        }

        [SampleExtension]
        public class SampleModelWithExtension : MicroModelBase
        {
            
        }

        private void Seal()
        {
            ((ISelfDescribing)Model).SealTypeDescription();
        }

        #endregion

        protected IMicroModel Model { get; set; }
        
        [SetUp]
        public void SetUp()
        {
            Model = new SampleModel();
        }

        [Test]
        public void CanAddExtension()
        {
            var extension = new Mock<IModelExtension>();
            Model.AddExtension(extension.Object);
        }

        [Test]
        public void ApplyIsCalledOnExtensionsDuringSeal()
        {
            var extension = new Mock<IModelExtension>();
            Model.AddExtension(extension.Object);
            Seal();
            extension.Verify(x => x.Apply(Model));
        }

        [Test]
        public void CanRemoveExtension()
        {
            var extension = new Mock<IModelExtension>();
            extension.Setup(x => x.Apply(It.IsAny<IMicroModel>())).Throws(new Exception("Should have been removed"));
            Model.AddExtension(extension.Object);
            Model.RemoveExtension(extension.Object);
            Seal();
        }

        [Test]
        public void RemoveExtensionIgnoresMissingExtensions()
        {
            var extension = new Mock<IModelExtension>();
            Model.RemoveExtension(extension.Object);
        }

        [Test]
        public void AddExtensionIgnoresAddedExtensions()
        {
            var extension = new Mock<IModelExtension>();
            Model.AddExtension(extension.Object);
            Model.AddExtension(extension.Object);
            Model.AddExtension(extension.Object);
            Model.AddExtension(extension.Object);
            Assert.AreEqual(1, Model.GetExtensions().Count());
        }

        [Test]
        public void ExtensionsCanAddProperties()
        {
            var extension = new Mock<IModelExtension>();
            extension.Setup(x => x.Apply(Model)).Callback(() => Model.AddProperty(new Mock<IPropertyDefinition>().Object));
            Model.AddExtension(extension.Object);
            Seal();
        }

        [Test]
        public void ExtensionsAreDiscoveredViaAttributes()
        {
            var model = new SampleModelWithExtension() as IMicroModel;
            Assert.AreEqual(0, model.GetExtensions().Count());
            Assert.AreEqual(0, model.GetProperties().Count());
            ((ISelfDescribing)model).SealTypeDescription();
            Assert.AreEqual(1, model.GetExtensions().Count());
            Assert.AreEqual(1, model.GetProperties().Count());
        }

        [Test]
        public void ExtensionsCannotAddAdditionalExtensions()
        {
            var extension = new Mock<IModelExtension>();
            extension.Setup(x => x.Apply(Model)).Callback(() => Model.AddExtension(new Mock<IModelExtension>().Object));
            Model.AddExtension(extension.Object);
            Assert.Throws<NotSupportedException>(Seal);
        }

        [Test]
        public void ExtensionsCannotRemoveExtensions()
        {
            var extension = new Mock<IModelExtension>();
            extension.Setup(x => x.Apply(Model)).Callback(() => Model.RemoveExtension(new Mock<IModelExtension>().Object));
            Model.AddExtension(extension.Object);
            Assert.Throws<NotSupportedException>(Seal);
        }

        [Test]
        public void CanAddProperty()
        {
            Model.AddProperty(new Mock<IPropertyDefinition>().Object);
            Assert.AreEqual(1, Model.GetProperties().Count());
        }

        [Test]
        public void CanRemoveProperty()
        {
            var property = new Mock<IPropertyDefinition>().Object;
            Model.AddProperty(property);
            Assert.AreEqual(1, Model.GetProperties().Count());
            Model.RemoveProperty(property);
            Assert.AreEqual(0, Model.GetProperties().Count());
        }

        [Test]
        public void CannotAddPropertyAfterSeal()
        {
            Seal();
            Assert.Throws<NotSupportedException>(
                () => Model.AddProperty(new Mock<IPropertyDefinition>().Object)
                );
        }

        [Test]
        public void ExtensionsAreAppliedInPriorityOrder()
        {
            var calls = new Queue<string>();
            var extension1 = new Mock<IModelExtension>();
            extension1.SetupGet(x => x.Priority).Returns(3);
            extension1.Setup(x => x.Apply(Model)).Callback(() => calls.Enqueue("3"));
            var extension2 = new Mock<IModelExtension>();
            extension2.SetupGet(x => x.Priority).Returns(1);
            extension2.Setup(x => x.Apply(Model)).Callback(() => calls.Enqueue("1"));
            var extension3 = new Mock<IModelExtension>();
            extension3.SetupGet(x => x.Priority).Returns(2);
            extension3.Setup(x => x.Apply(Model)).Callback(() => calls.Enqueue("2"));

            Model.AddExtension(extension1.Object);
            Model.AddExtension(extension2.Object);
            Model.AddExtension(extension3.Object);
            Seal();

            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual("1", calls.Dequeue());
            Assert.AreEqual("2", calls.Dequeue());
            Assert.AreEqual("3", calls.Dequeue());
        }
    }
}
