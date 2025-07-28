using Avalonia.Data.Converters;
using Avalonia.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Avalonia.Media;

namespace youtube_downloader_plus.Converters
{
    public class BinaryColorConverter : IValueConverter
    {
        public static readonly BinaryColorConverter Instance = new(); 

        public object? Convert(object? value, Type targetType, object? parameter,
                                                                CultureInfo culture)
        {
            if (value is bool sourceVal && parameter is string colors
                && targetType.IsAssignableTo(typeof(IBrush)))
            {
                var colorList = colors.Split("-", StringSplitOptions.RemoveEmptyEntries);
                if (colorList.Count() < 2)
                {
                    throw new ArgumentException("Need to colors, - separated");
                }
                return SolidColorBrush.Parse(sourceVal ? colorList[0] : colorList[1]);
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(),
                                                    BindingErrorType.Error);
        }

        public object ConvertBack(object? value, Type targetType,
                                    object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
