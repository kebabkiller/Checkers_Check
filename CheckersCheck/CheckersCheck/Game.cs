using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CheckersCheck
{
    class Game
    {
        public GameField[,] board;
        bool isLeft;

        public Game(bool _isLeft)
        {
            this.isLeft = _isLeft;
            this.board = new GameField[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    this.board[i, j] = new GameField(2); // oznaczenie wszystkich pól, jako puste
                }
            }

            fillFields();
        }

        private void fillFields()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i < 3)
                    {
                        if (i % 2 == 0)
                        {
                            if (j % 2 == 0)
                                this.board[i, j].color = 0;
                        }
                        if (i % 2 != 0)
                        {
                            if (j % 2 != 0)
                            {
                                this.board[i, j].color = 0;
                            }
                        }
                    }
                    if (i > 4)
                    {
                        if (i % 2 == 0)
                        {
                            if (j % 2 == 0)
                                this.board[i, j].color = 1;
                        }
                        if (i % 2 != 0)
                        {
                            if (j % 2 != 0)
                            {
                                this.board[i, j].color = 1;
                            }
                        }
                    }
                }
            }
        }

        public void updateStatus(List<int> gameState)
        {
            int iterator = 0;

            if (isLeft == true)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 7; j >= 0; --j)
                    {
                        if (i % 2 == 0)
                        {
                            if (j % 2 == 0)
                            {
                                this.board[j, i].color = gameState[iterator];
                                iterator++;
                            }
                        }
                        if (i % 2 != 0)
                        {
                            if (j % 2 != 0)
                            {
                                this.board[j, i].color = gameState[iterator];
                                iterator++;
                            }
                        }
                    }
                }
            }

            if (isLeft == false)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (i % 2 == 0)
                        {
                            if (j % 2 == 0)
                            {
                                this.board[j, i].color = gameState[iterator];
                                iterator++;
                            }
                        }
                        if (i % 2 != 0)
                        {
                            if (j % 2 != 0)
                            {
                                this.board[j, i].color = gameState[iterator];
                                iterator++;
                            }                            
                        }
                    }
                }
            }
        }
    }
}
