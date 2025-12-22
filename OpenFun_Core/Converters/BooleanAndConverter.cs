using System.Globalization;

namespace OpenFun_Core.Converters
{
    public class BooleanAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = values.Select(value =>
            {
                if (value is bool boolValue)
                {
                    return boolValue;
                }
                return false;
            })
            .All(x => x == true);

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
