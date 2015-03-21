using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCTime.ViewModel;
using NUnit.Framework;
using Microsoft.TeamFoundation.MVVM;
namespace PCTime.ViewModel.Tests
{
    [TestFixture, Category("EventLogViewModel")]
    public class EventLogViewModelTest
    {
        [Test, Category("nomal")]
        public void EventLogViewModelTest01()
        {
            var vm = new EventLogViewModel();

            var now = DateTime.Now;

            vm.StartDate = now.AddMonths(-1);
            vm.EndDate = now;

            Assert.True(!(vm as WindowViewModel).HasErrors());
        }

        [Test, Category("error")]
        public void EventLogViewModelTest02()
        {
            var vm = new EventLogViewModel();

            var now = DateTime.Now;

            vm.EndDate = now.AddMonths(-1);
            vm.StartDate = now;

            Assert.True((vm as WindowViewModel).HasErrors());
        }

        [Test, Category("error")]
        public void EventLogViewModelTest03()
        {
            var vm = new EventLogViewModel();

            var now = DateTime.Now;

            vm.StartDate = now.AddMonths(-1);
            vm.EndDate = now.AddDays(1);

            Assert.True((vm as WindowViewModel).HasErrors());
        }

        [Test, Category("error")]
        public void EventLogViewModelTest04()
        {
            var vm = new EventLogViewModel();

            var now = DateTime.Now;

            vm.StartDate = now.AddDays(1);
            vm.EndDate = now.AddDays(1);

            Assert.True((vm as WindowViewModel).HasErrors());
        }

    }
}
