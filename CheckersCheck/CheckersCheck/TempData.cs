using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersCheck
{
    public class TempData
    {
        public string playerWhite = "test1";
        public string playerBlack = "test2";
        public bool isWhiteTurn = true; //true - kolej na ruch gracza białego, false - ruch gracza czarnego

        //statystyki
        public int whitePieces = 12;
        public int blackPieces = 12;
        public int whiteKing = 0;
        public int blackKing = 0;

    }
}
