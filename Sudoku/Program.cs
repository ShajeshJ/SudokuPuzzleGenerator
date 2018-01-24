using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;
using Microsoft.Win32;

namespace Sudoku
{
    class Program
    {
        private static int elapsedTime;
        static void Main(string[] args)
        {
            var runner = new GameRunner();
            var board = runner.Board;

            var boardImage = DrawBoard(board, ValidBoard(board));

            boardImage.Save(@"sudoku.png");

            var divider = "-------------";

            for (int i = 0; i < 9; i++)
            {
                if (i%3 == 0)
                {
                    Console.WriteLine(divider);
                }

                for (int j = 0; j < 9; j++)
                {
                    if (j%3 == 0)
                    {
                        Console.Write("|");
                    }
                    Console.Write(board[i, j] == -1 ? "_" : ""+board[i, j]);
                }
                Console.WriteLine("|");
            }
            Console.WriteLine(divider);
            //Console.ReadKey();
        }

        private static bool[,] ValidBoard(int[,] board)
        {
            var goodCells = new bool[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    goodCells[i, j] = true;
                }
            }

            var checkHorz = new HashSet<int>();
            var checkVert = new HashSet<int>();

            var badRow = false;
            var badCol = false;

            for (int i = 0; i < 9; i++)
            {
                badRow = false;
                badCol = false;
                checkHorz.Clear();
                checkVert.Clear();

                for (int j = 0; j < 9; j++)
                {
                    if(board[i, j] != -1 && !checkHorz.Add(board[i, j]))
                    {
                        badRow = true;
                    }

                    if(board[j, i] != -1 && !checkVert.Add(board[j, i]))
                    {
                        badCol = true;
                    }
                }

                if (badRow)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        goodCells[i, j] = false;
                    }
                }

                if (badCol)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        goodCells[j, i] = false;
                    }
                }
            }

            var checkBox = new HashSet<int>();

            for (int i = 0; i < 9; i+=3)
            {
                for (int j = 0; j < 9; j+=3)
                {
                    checkBox.Clear();

                    var valid = (board[i, j] == -1 || checkBox.Add(board[i, j]))
                                && (board[i + 1, j] == -1 || checkBox.Add(board[i + 1, j]))
                                && (board[i + 2, j] == -1 || checkBox.Add(board[i + 2, j]))
                                && (board[i, j + 1] == -1 || checkBox.Add(board[i, j + 1]))
                                && (board[i + 1, j + 1] == -1 || checkBox.Add(board[i + 1, j + 1]))
                                && (board[i + 2, j + 1] == -1 || checkBox.Add(board[i + 2, j + 1]))
                                && (board[i, j + 2] == -1 || checkBox.Add(board[i, j + 2]))
                                && (board[i + 1, j + 2] == -1 || checkBox.Add(board[i + 1, j + 2]))
                                && (board[i + 2, j + 2] == -1 || checkBox.Add(board[i + 2, j + 2]));

                    if (!valid)
                    {
                        goodCells[i, j] = false;
                        goodCells[i + 1, j] = false;
                        goodCells[i + 2, j] = false;
                        goodCells[i, j + 1] = false;
                        goodCells[i + 1, j + 1] = false;
                        goodCells[i + 2, j + 1] = false;
                        goodCells[i, j + 2] = false;
                        goodCells[i + 1, j + 2] = false;
                        goodCells[i + 2, j + 2] = false;
                    }
                }
            }

            return goodCells;
        }

        private static Image DrawBoard(int [,] board, bool[,] valid)
        {
            var tmpImg = new Bitmap(1, 1);
            var tmpDrawing = Graphics.FromImage(tmpImg);

            var font = new Font(FontFamily.GenericMonospace, 24, FontStyle.Bold, GraphicsUnit.Pixel);
            var textBrush = new SolidBrush(Color.Black);

            var textSize = tmpDrawing.MeasureString("1", font);

            tmpImg.Dispose();
            tmpDrawing.Dispose();

            var goodImgs = new Bitmap[10];
            var badImgs = new Bitmap[10];
            var goodDrawings = new Graphics[10];
            var badDrawings = new Graphics[10];

            goodImgs[0] = new Bitmap((int)textSize.Width, (int)textSize.Height);
            goodDrawings[0] = Graphics.FromImage(goodImgs[0]);
            goodDrawings[0].Clear(Color.White);
            goodDrawings[0].Save();
            goodDrawings[0].Dispose();

            badImgs[0] = new Bitmap((int)textSize.Width, (int)textSize.Height);
            badDrawings[0] = Graphics.FromImage(badImgs[0]);
            badDrawings[0].Clear(Color.PaleVioletRed);
            badDrawings[0].Save();
            badDrawings[0].Dispose();

            for (int i = 1; i < goodImgs.Length; i++)
            {
                goodImgs[i] = new Bitmap((int)textSize.Width, (int)textSize.Height);
                goodDrawings[i] = Graphics.FromImage(goodImgs[i]);
                goodDrawings[i].Clear(Color.White);
                goodDrawings[i].DrawString($"{i + 1}", font, textBrush, 0, 0);
                goodDrawings[i].Save();
                goodDrawings[i].Dispose();

                badImgs[i] = new Bitmap((int)textSize.Width, (int)textSize.Height);
                badDrawings[i] = Graphics.FromImage(badImgs[i]);
                badDrawings[i].Clear(Color.PaleVioletRed);
                badDrawings[i].DrawString($"{i + 1}", font, textBrush, 0, 0);
                badDrawings[i].Save();
                badDrawings[i].Dispose();
            }

            var tileWidth = goodImgs[0].Width;
            var tileHeight = goodImgs[0].Height;

            var boardImg = new Bitmap(tileWidth*9, tileHeight*9);
            var boardDrawing = Graphics.FromImage(boardImg);

            for(int i = 0; i < 9; i++)
            {
                for(int j = 0; j < 9; j++)
                {
                    if (board[i, j] != -1)
                    {
                        boardDrawing.DrawImage(valid[i, j] ? goodImgs[board[i, j]] : badImgs[board[i, j]], j * tileWidth, i * tileHeight);
                    }
                    else
                    {
                        boardDrawing.DrawImage(valid[i, j] ? goodImgs[0] : badImgs[0], j * tileWidth, i * tileHeight);
                    }
                }
            }

            var smallPen = new Pen(textBrush, 1);
            var bigPen = new Pen(textBrush, 3);

            for(int i = 1; i < 9; i++)
            {
                boardDrawing.DrawLine(smallPen, tileWidth * i, 0, tileWidth * i, tileHeight * 9);
                boardDrawing.DrawLine(smallPen, 0, tileHeight * i, tileWidth * 9, tileHeight * i);
            }

            for (int i = 0; i < 9; i+=3)
            {
                boardDrawing.DrawLine(bigPen, tileWidth * i, 0, tileWidth * i, tileHeight * 9);
                boardDrawing.DrawLine(bigPen, 0, tileHeight * i, tileWidth * 9, tileHeight * i);
            }
            boardDrawing.DrawLine(bigPen, tileWidth * 9 - 1, 0, tileWidth * 9 - 1, tileHeight * 9);
            boardDrawing.DrawLine(bigPen, 0, tileHeight * 9 - 1, tileWidth * 9, tileHeight * 9 - 1);

            boardDrawing.Save();

            return boardImg;
        }
    }
}
