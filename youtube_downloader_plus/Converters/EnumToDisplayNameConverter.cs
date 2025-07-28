using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using youtube_downloader_plus.ViewModels;

namespace youtube_downloader_plus.Converters
{
    public class EnumToDisplayNameConverter : IValueConverter
    {
        public static readonly EnumToDisplayNameConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter,
                                                                CultureInfo culture)
        {
            var valueType = value.GetType();
            var valueTypeDefinition = valueType?.GetGenericTypeDefinition();
            var argumentsType = valueType?.GetGenericArguments()?.FirstOrDefault();

            if (valueType.IsGenericType && valueTypeDefinition == typeof(List<>) && argumentsType.IsAssignableTo(typeof(Enum)) &&
                targetType.IsAssignableTo(typeof(IEnumerable)))
            {
                var sourceVal = (IEnumerable)value;
                MemberInfo[] memberInfos =
                argumentsType.GetMembers(BindingFlags.Public | BindingFlags.Static);

                List<string> names = new List<string>();
                foreach (var src in sourceVal)
                {
                    var memberInfo = memberInfos.FirstOrDefault(m => m.Name == src.ToString());
                    names.Add(memberInfo.GetCustomAttribute<DisplayAttribute>()?.GetName());
                }
                return names;
            }
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
