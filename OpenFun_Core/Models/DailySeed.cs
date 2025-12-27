namespace OpenFun_Core.Models
{
    public class DailySeed : IDailySeed
    {
        public int GetDailySeed()
        {
            return DateTime.UtcNow.AddHours(10).Date.Ticks.GetHashCode();
        }
    }
}
