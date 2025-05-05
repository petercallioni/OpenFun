using OpenFun_Core.Models;

namespace OpenFun_CoreTests.Mock
{
    public class TestDailySeed : IDailySeed
    {
        private readonly int seed;
        public TestDailySeed(int seed)
        {
            this.seed = seed;
        }

        public int GetDailySeed()
        {
            return seed;
        }
    }
}
