namespace OpenFun_Core.Models
{
    public class DailySeed : IDailySeed
    {
        public static int GetDailySeed()
        {
            // Get current UTC time and adjust for UTC+10
            DateTime utcNow = DateTime.UtcNow;
            DateTime localDate = utcNow.AddHours(10);

            // Use the date part (year, month, and day) to create a unique seed per day
            int seed = localDate.Year * 10000 + localDate.Month * 100 + localDate.Day;

            return seed;
        }
    }
}
