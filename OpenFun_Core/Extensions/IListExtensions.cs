namespace OpenFun_Core.Extensions
{
    public static class IListExtensions
    {
        /// <summary>
        /// Shuffles the elements of the list randomly using the Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="random">
        /// An optional instance of Random to control randomness. 
        /// If null, a new Random instance is created.
        /// </param>
        public static void Shuffle<T>(this IList<T> list, Random? random = null)
        {
            // If no Random instance is provided, create a new one.
            if (random == null)
            {
                random = new Random();
            }

            int n = list.Count;

            // Iterate through the list in reverse order to shuffle elements.
            while (n > 1)
            {
                n--;

                // Generate a random index between 0 and n (inclusive).
                int k = random.Next(n + 1);

                // Swap the randomly chosen element with the current element.
                T temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }
    }
}