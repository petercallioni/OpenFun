namespace Pangram.Utilities
{
    public static class Rank
    {
        public static double TargetS { get; set; } = 100.0;
        public static double TargetA { get; set; } = 90.0;
        public static double TargetF { get; set; } = 30.0;

        private static readonly string[] _rankLabels = { "S", "A", "B", "C", "D", "E", "F" };

        public static string GetRank(int score, int maxScore)
        {
            if (maxScore <= 0 || score <= 0) return "";

            double percentage = ((double)score / (double)maxScore) * 100;

            // 1. Calculate the 'k' (power) required to make the curve 
            // pass through TargetA at the first step.
            // Formula derived from: TargetA = F + (S - F) * (1 - 1/6)^k
            double stepsToF = _rankLabels.Length - 1; // Usually 6 steps
            double range = TargetS - TargetF;
            double targetStepRatio = (TargetA - TargetF) / range;
            double stepWidthRatio = (stepsToF - 1) / stepsToF; // e.g., 5/6

            double powerK = Math.Log(targetStepRatio) / Math.Log(stepWidthRatio);

            // 2. Evaluate where the current percentage falls
            for (int i = 0; i < _rankLabels.Length; i++)
            {
                double threshold = CalculateThreshold(i, stepsToF, powerK);
                if (percentage >= threshold)
                {
                    return _rankLabels[i];
                }
            }

            return "";
        }

        private static double CalculateThreshold(int stepIndex, double totalSteps, double k)
        {
            // Power Curve Formula: Floor + (Range * (RemainingSteps / TotalSteps)^k)
            double progressToBottom = stepIndex / totalSteps;
            return TargetF + (TargetS - TargetF) * Math.Pow(1 - progressToBottom, k);
        }
    }
}
