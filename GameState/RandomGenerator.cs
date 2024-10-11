namespace GameState
{
    public class RandomGenerator
    {
        private uint state;

        public RandomGenerator(int seed)
        {
            state = (uint)seed;
        }

        public int Next(int maxValue)
        {
            state = state * 1103515245 + 12345;
            return (int)((state / 65536) % 32768) % maxValue;
        }

        public int Next(int minValue, int maxValue)
        {
            return Next(maxValue - minValue) + minValue;
        }

        public float NextFloat()
        {
            return Next(10000) / 10000f;
        }

        public float NextFloat(float minValue, float maxValue)
        {
            return NextFloat() * (maxValue - minValue) + minValue;
        }
    }
}
