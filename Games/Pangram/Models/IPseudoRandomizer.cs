namespace Pangram.Models
{
    public interface IPseudoRandomizer
    {
        static abstract int GenerateDailySeed();
    }
}