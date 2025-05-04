using OpenFun_Core.Models;

namespace OpenFun_CoreTests.Mock
{
    public class TestDailySeed : IDailySeed
    {
        public static int GetDailySeed()
        {
            return 1;
        }
    }
}
