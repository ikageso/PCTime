using PCTime.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PCTime.View.Converter
{
    /// <summary>
    /// 日付の背景色を返す
    /// </summary>
    class DateToWeek2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
                return Brushes.LightGray;

            DateTime dt1;
            
            if(value.GetType().Equals(typeof(DateTime)))
            {
                dt1 = (DateTime)value;

            }
            else if(value.GetType().Equals(typeof(string)))
            {
                dt1 = DateTime.Parse((string)value);
            }
            else
            {
                return Brushes.LightGray;
            }

            SolidColorBrush br = Brushes.Transparent;

            try
            {              

                var holiday = HolidayChecker.Holiday(dt1);


                if (holiday.week == DayOfWeek.Saturday)
                    br = Brushes.Blue;

                if (holiday.week == DayOfWeek.Sunday)
                    br = Brushes.Red;

                if (holiday.holiday == HolidayChecker.HolidayInfo.HOLIDAY.HOLIDAY)
                    br = Brushes.Red;

                if (holiday.holiday == HolidayChecker.HolidayInfo.HOLIDAY.SYUKUJITSU)
                    br = Brushes.Red;

                if (holiday.holiday == HolidayChecker.HolidayInfo.HOLIDAY.C_HOLIDAY)
                    br = Brushes.Red;

            }
            finally
            {
                // なにもしない
            }

            return br;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
