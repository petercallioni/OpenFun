namespace OpenFun_Core.Models
{
    public class DailySeed : IDailySeed
    {
        public int GetDailySeed()
        {
            // Adjust the current UTC date and time by 10 hours
            DateTime adjustedDateTime = DateTime.UtcNow.AddHours(10);

            // Get only the date part (ignoring the time) and convert to an integer representation
            DateTime adjustedDate = adjustedDateTime.Date; // Keep only the date component

            // Use the ticks of the adjusted date as the seed (ticks represent the number of 100-nanosecond intervals since 01/01/0001)
            return adjustedDate.Ticks.GetHashCode();
        }
    }
}
