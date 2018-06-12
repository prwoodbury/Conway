using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Conway
{
    class ConwayBoard
    {
        //===================================================================
        // Public
        //===================================================================

        /**
         * Board width (in cells).
         */
        public static readonly int width = 96;

        /**
         * Board height (in cells).
         */
        public static readonly int height = 55;

        /**
         * Constructor.
         */
        public ConwayBoard()
        {
            // Initialize board array
            board = new bool[width, height];
            clearBoard();
        }

        /**
         * Copy constructor.
         */
        public ConwayBoard(ConwayBoard other)
        {
            // Deep copy board array
            board = new bool[other.board.GetLength(0),
                             other.board.GetLength(1)];
            Array.Copy(other.board, board, other.board.Length);
        }

        /**
         * Returns the cell at the given logical position.
         */
        public bool cell(int x, int y)
        {
            // get physical coordinates
            adjustCoords(ref x, ref y);
            return board[x, y];
        }

        /**
         * Modifies the cell at the given logical position.
         */
        public void setCell(int x, int y, bool state)
        {
            // get physical coordinates
            adjustCoords(ref x, ref y);
            board[x, y] = state;
        }

        /**
         * Sets all board cells to empty.
         */
        public void clearBoard()
        {
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    board[i, j] = false;
                }
            }
        }

        /**
         * Advances the board state by a tick, following the rules of the game.
         */
        public void runTick()
        {
            // Create copy of the board's current state to reference while we
            // update the actual board
            ConwayBoard oldBoard = new ConwayBoard(this);

            // Iterate over all cells in old board
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    // Count adjacent live cells
                    int neighborCount = 0;
                    // UL
                    if (oldBoard.cell(i - 1, j - 1)) ++neighborCount;
                    // U
                    if (oldBoard.cell(i, j - 1)) ++neighborCount;
                    // UR
                    if (oldBoard.cell(i + 1, j - 1)) ++neighborCount;
                    // L
                    if (oldBoard.cell(i - 1, j)) ++neighborCount;
                    // R
                    if (oldBoard.cell(i + 1, j)) ++neighborCount;
                    // DL
                    if (oldBoard.cell(i - 1, j + 1)) ++neighborCount;
                    // D
                    if (oldBoard.cell(i, j + 1)) ++neighborCount;
                    // DR
                    if (oldBoard.cell(i + 1, j + 1)) ++neighborCount;

                    if (oldBoard.cell(i, j))
                    {
                        // Live cell

                        // 1. Any live cell with fewer than two neighbors dies
                        if (neighborCount < 2) setCell(i, j, false);
                        // 2. Any live cell with two or three neighbors lives on
                        else if (neighborCount <= 3) { }
                        // 3. Any live cell with more than three neighbors dies
                        else if (neighborCount > 3) setCell(i, j, false);
                    }
                    else
                    {
                        // Dead cell

                        // 4. Any dead cell with three live neighbors becomes a live cell
                        if (neighborCount == 3) setCell(i, j, true);
                    }
                }
            }
        }

        /**
         * Prints the board state to console.
         */
        public void print()
        {
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Console.Write("{0} ", board[i, j] ? "X" : " ");
                }
                Console.Write("\n");
            }
        }

        /**
         * Draws the board state.
         */
        public void draw(Graphics g, int cellW = 8, int cellH = 8)
        {
            // Draw background

            SolidBrush brush = new SolidBrush(Color.White);

            g.FillRectangle(brush, 0, 0, cellW * width, cellH * height);

            // Draw cells

            brush.Color = Color.Black;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    int x = cellW * i;
                    int y = cellH * j;

                    if (cell(i, j)) g.FillRectangle(brush, x, y, cellW, cellH);
                }
            }

            brush.Dispose();

            // Draw grid
            Pen pen = new Pen(Color.Gray);
            for (int j = 0; j < height; j++)
            {
                int y = cellH * j;
                int end = cellW * width;
                g.DrawLine(pen, 0, y, end, y);
            }
            for (int i = 0; i < width; i++)
            {
                int x = cellW * i;
                int end = cellH * height;
                g.DrawLine(pen, x, 0, x, end);
            }
            pen.Dispose();


        }

        public string asString()
        {
            string result = "";

            result += String.Format("{0} ", width);
            result += String.Format("{0} ", height);

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    result += String.Format("{0} ", board[i, j]);
                }
            }

            return result;
        }

        public void fromString(string str)
        {
            string[] split = str.Split(' ');
            int pos = 2;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    board[i, j] = (split[pos++] == "True") ? true : false;
                }
            }
        }

        //===================================================================
        // Private
        //===================================================================

        /**
        * 2D array containing cell states.
        */
        private bool[,] board;

        /**
         * Adjusts a pair of logical coordinates to physical ones, so that
         * coordinates less than 0 wrap to the opposite edge of the board
         * and vice versa.
         */
        private void adjustCoords(ref int x, ref int y)
        {
            // Wrap target coordinates until they lie within the
            // actual board
            if (x < 0) x = -(-x % width) + width;
            else if (x >= width) x %= width;

            if (y < 0) y = -(-y % height) + height;
            else if (y >= height) y %= height;

 //           while (x < 0) x += width;
 //           while (y < 0) y += height;
 //           while (x >= width) x -= width;
 //           while (y >= height) y -= height;
        }
    }
}
