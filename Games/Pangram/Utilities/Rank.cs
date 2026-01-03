namespace Pangram.Utilities
{
    public static class Rank
    {
        public static string GetRank(int score, int maxScore)
        {
            string rankText = "";
            if (score <= 0 || maxScore <= 0)
            {
                return "";
            }

            double percentage = (double)score / (double)maxScore * 100;

            rankText = percentage switch
            {
                >= 90 => "S",
                >= 80 => "A",
                >= 70 => "B",
                >= 60 => "C",
                >= 50 => "D",
                >= 40 => "E",
                >= 30 => "F",
                _ => ""
            };

            return rankText;
        }
    }
}
