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
    class DateFormWeekBackGroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush bkbr = (SolidColorBrush)value;

            if (bkbr == null)
                return Brushes.Black.Color;

            Color cr = Brushes.Black.Color;

            try
            {

                if (bkbr.Equals(Brushes.Blue))
                    cr = Brushes.White.Color;

                if (bkbr.Equals(Brushes.Red))
                    cr = Brushes.White.Color;

            }
            finally
            {
                // なにもしない
            }

            return cr;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
