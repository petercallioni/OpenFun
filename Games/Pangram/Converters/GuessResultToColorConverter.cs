using Pangram.Models;
using System.Globalization;

namespace Pangram.Converters
{
    public class GuessResultToMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GuessWordResults result)
            {
                return result switch
                {
                    GuessWordResults.INVALID => "Word Not Valid",
                    GuessWordResults.ALREADY_GUESSED => "Already Guessed",
                    GuessWordResults.FORBIDDEN_CHARACTERS => "Contains Invalid Characters",
                    GuessWordResults.DOES_NOT_CONTAIN_MAIN_LETTER => "Does Not Contain Main Letter",
                    GuessWordResults.VALID => "",
                    GuessWordResults.NONE => "",
                    _ => "",
                };
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}