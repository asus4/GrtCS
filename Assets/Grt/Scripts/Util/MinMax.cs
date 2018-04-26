namespace GRT
{
    public struct MinMax
    {
        public double minValue;
        public double maxValue;

        public MinMax(double minValue, double maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public bool UpdateMinMax(double newValue)
        {
            if (newValue < minValue)
            {
                minValue = newValue;
                return true;
            }
            if (newValue > maxValue)
            {
                maxValue = newValue;
                return true;
            }
            return false;
        }

    }
}
