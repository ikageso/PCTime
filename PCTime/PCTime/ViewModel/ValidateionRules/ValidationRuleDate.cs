using Microsoft.TeamFoundation.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCTime.ViewModel.ValidateionRules
{
    public class ValidationRuleDate : ValidationRule
    {
        public ValidationRuleDate(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public override bool Validate(object viewModel)
        {
            var targetDate = this.GetPropertyValue(viewModel) as DateTime?;
            var today = DateTime.Now;
            var todayEndTime = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);

            // 未来日をチェック
            if (targetDate > todayEndTime)
            {
                Error = "未来日は指定できません";
                return false;
            }

            var vm = viewModel as EventLogViewModel;

            // 日付の逆転をチェック
            var startDate = vm.StartDate;
            var endDate = vm.EndDate;

            if (startDate > endDate)
            {
                Error = "日付が逆転しています";
                return false;
            }

            return true;
        }

    }
}
