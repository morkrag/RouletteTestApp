using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteApp
{
    public class Result
    {
        public int _roulettePosition;
        public int _multiplier = 1;

        public Result(int roulettePostion, int sameColorInARow) 
        {
            _roulettePosition = roulettePostion;
            if (sameColorInARow > 1)
            {
                _multiplier = roulettePostion * sameColorInARow;
            }
        }
    }
}
