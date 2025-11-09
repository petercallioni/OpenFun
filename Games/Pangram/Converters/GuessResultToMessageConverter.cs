using Pangram.Models;
using System.Globalization;

namespace Pangram.Converters
{
    public class GuessResultToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GuessWordResults result)
            {
                return result switch
                {
                    GuessWordResults.VALID => Colors.Lime,
                    GuessWordResults.INVALID => Colors.Tomato,
                    GuessWordResults.ALREADY_GUESSED => Colors.LightGray,
                    GuessWordResults.FORBIDDEN_CHARACTERS => Colors.Purple,
                    GuessWordResults.DOES_NOT_CONTAIN_MAIN_LETTER => Colors.Red,
                    GuessWordResults.NONE => Colors.Orange,
                    _ => Colors.Transparent,
                };
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}