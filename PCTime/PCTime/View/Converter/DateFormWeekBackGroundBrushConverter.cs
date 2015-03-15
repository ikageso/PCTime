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
    /// 日付の文字色を返す
    /// </summary>
    class DateFormWeekBackGroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush bkbr = (SolidColorBrush)value;

            if (bkbr == null)
                return Brushes.Black;

            SolidColorBrush br = Brushes.Black;

            try
            {

                if (bkbr.Equals(Brushes.Blue))
                    br = Brushes.White;

                if (bkbr.Equals(Brushes.Red))
                    br = Brushes.White;

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
