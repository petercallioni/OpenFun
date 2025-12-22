using System.Globalization;

namespace OpenFun_Core.Converters
{
    public class IsStringNotNullOrEmptyConverter : IValueConverter
    {
        /// <summary>
        /// Returns True if the string given is null or empty. a true bool can be given as the parameter to invert the result.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string word)
            {
                bool result = !string.IsNullOrEmpty(word);

                if (parameter is bool invert)
                {
                    if(invert)
                    {
                        result = !result;
                    }
                }

                return result;
            }

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
