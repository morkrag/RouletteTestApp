namespace RouletteApp
{
    public class Result
    {
        public int RoulettePosition { get; set; }
        public int Multiplier { get; set; } = 1;

        public Result(int roulettePostion, int sameColorInARow) 
        {
            RoulettePosition = roulettePostion;
            if (sameColorInARow > 1)
            {
                Multiplier = roulettePostion * sameColorInARow;
            }
        }
    }
}
