using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCTime.Model;
using NUnit.Framework;
namespace PCTime.Model.Tests
{
    [TestFixture, Category("EventLogDataModel")]
    public class EventLogDataModelTests
    {
        [Test, Category("GetEventLog")]
        public void GetEventLogTest()
        {
            var now = DateTime.Now;

            var startDate = now;
            var endDate = now;

            var results = EventLogDataModel.GetEventLog(startDate, endDate);

            Assert.True(results != null);
        }

        [Test, Category("GetEventLog")]
        public void MakeDataTest()
        {
            var now = DateTime.Now;

            var startDate = now;
            var endDate = now;

            var results = EventLogDataModel.GetEventLog(startDate, endDate);

            var list = EventLogDataModel.MakeData(results);

            Assert.True(list != null);
        }
    }
}
