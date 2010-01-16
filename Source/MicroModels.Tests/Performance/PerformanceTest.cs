using System;
using System.Diagnostics;
using MicroModels.Tests.Helpers;
using NUnit.Framework;
using System.ComponentModel;

namespace MicroModels.Tests.Performance
{
    /// <summary>
    /// This crude suite of tests gives an idea of how much overhead a MicroModel can have. Although the 
    /// hard coded class uses CLR properties, to provide a more accurate simulation, the tests use type 
    /// descriptors (via ReadProperty/WriteProperty) since that is what WPF data binding will call. 
    /// </summary>
    [TestFixture]
    public class PerformanceTest
    {
        #region SUT

        public class HardCodedExample
        {
            public HardCodedExample()
            {
                FirstName = "X";
                LastName = "Y";
                DateOfBirth = DateTime.Today;
            }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
        }

        public class MicroModelExample : MicroModel
        {
            public MicroModelExample()
            {
                var customer = new HardCodedExample();
                this.Property(() => customer.FirstName);
                this.Property(() => customer.LastName);
                this.Property(() => customer.DateOfBirth);
            }
        }

        private static void Time(string message, int times, Action action)
        {
            Trace.Write(string.Format("{0} ({1:n0} times): ", message, times));
            var watch = Stopwatch.StartNew();
            for (var i = 0; i < times; i++)
            {
                action();
            }
            watch.Stop();
            Trace.WriteLine(string.Format("{0} milliseconds", watch.ElapsedMilliseconds));
        }

        #endregion

        [Test]
        public void Construct()
        {
            Time("Constructing hard coded object", 10000, () => new HardCodedExample());
            Time("Constructing micromodel object", 10000, () => new MicroModelExample());
        }

        [Test]
        public void GetDescriptor()
        {
            // The test above shows that CLR objects are faster to new-up, but even for a CLR property, WPF
            // will call TypeDescriptor.GetProperties, which has to create the type descriptor. This test is
            // a litle more realistic. CLR objects do have another benefit though in that they can cache the 
            // descriptor.
            Time("Get type descriptor for hard coded object", 10000, () => TypeDescriptor.GetProperties(new HardCodedExample()));
            Time("Get type descriptor for micromodel object", 10000, () => TypeDescriptor.GetProperties(new MicroModelExample()));
        }

        [Test]
        public void ReadProperty()
        {
            var hardCoded = new HardCodedExample();
            var microModel = new MicroModelExample();
            Time("Read property from hard coded object via descriptor", 10000, () => hardCoded.ReadProperty<string>("FirstName"));
            Time("Read property from micromodel object via descriptor", 10000, () => microModel.ReadProperty<string>("FirstName"));
        }

        [Test]
        public void WriteProperty()
        {
            var hardCoded = new HardCodedExample();
            var microModel = new MicroModelExample();
            Time("Write property to hard coded object via descriptor", 10000, () => hardCoded.WriteProperty("FirstName", "Fred"));
            Time("Write property to micromodel object via descriptor", 10000, () => microModel.WriteProperty("FirstName", "Fred"));
        }

        [Test]
        public void ConstructAndReadProperty()
        {
            Time("Read property from hard coded object (new each time) via descriptor", 10000, () => new HardCodedExample().ReadProperty<string>("FirstName"));
            Time("Read property from micromodel object (new each time) via descriptor", 10000, () => new MicroModelExample().ReadProperty<string>("FirstName"));
        }

        [Test]
        public void ConstructAndWriteProperty()
        {
            Time("Write property to hard coded object (new each time) via descriptor", 10000, () => new HardCodedExample().WriteProperty("FirstName", "Fred"));
            Time("Write property to micromodel object (new each time) via descriptor", 10000, () => new MicroModelExample().WriteProperty("FirstName", "Fred"));
        }
    }
}
