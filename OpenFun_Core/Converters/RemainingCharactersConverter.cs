using System.Globalization;

namespace OpenFun_Core.Converters
{
    public class RemainingCharactersConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string text && text.Length > 0)
            {
                return text.Substring(1);
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}