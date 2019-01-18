using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModifieurFermette.Models.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    class FullPathToFileNameConverter : IValueConverter
    {
        private string FullPath;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            this.FullPath = value as string;
            return Path.GetFileName(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.FullPath;
        }
    }
}
