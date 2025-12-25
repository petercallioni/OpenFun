using System.Globalization;

namespace OpenFun_Core.Converters
{
    public class CaseConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string strValue && parameter is string caseType)
            {
                return caseType.ToLower() switch
                {
                    "upper" => strValue.ToUpper(culture),
                    "lower" => strValue.ToLower(culture),
                    "title" => strValue.Length > 0
                        ? char.ToUpper(strValue[0], culture) + strValue.Substring(1).ToLower(culture)
                        : strValue,
                    _ => strValue,
                };
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
