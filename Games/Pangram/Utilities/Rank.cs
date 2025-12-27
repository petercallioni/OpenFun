using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                >= 80 => "S",
                >= 70 => "A",
                >= 60 => "B",
                >= 50 => "C",
                >= 40 => "D",
                >= 30 => "E",
                >= 20 => "F",
                _ => ""
            };

            return rankText;
        }
    }
}
