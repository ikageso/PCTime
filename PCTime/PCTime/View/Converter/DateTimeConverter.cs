using PCTime.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PCTime.View.Converter
{
    /// <summary>
    /// カルチャの日付を返す
    /// </summary>
    class DateTimeConverter : IValueConverter
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
                return string.Empty;
            }

            return dt1.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
