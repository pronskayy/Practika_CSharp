using System;
using System.Globalization;
using System.Windows.Data;

namespace EmployeeManagement.Converters
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is true ? "Доступен" : "Отсутствует";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == "Доступен";
        }
    }
}