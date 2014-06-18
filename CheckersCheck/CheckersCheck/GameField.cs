using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersCheck
{
    public class GameField
    {
        public int color;
        public bool isDame;

        public GameField(int _color)
        {
            this.color = _color;
            this.isDame = false;
        }
    }
}
