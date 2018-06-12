using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Conway
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*            ConwayBoard board = new ConwayBoard();
                        //            board.setCell(-1, -1, true);
                        //            Console.WriteLine("{0}", board.cell(ConwayBoard.width - 1, ConwayBoard.height - 1));
                        //           Console.WriteLine("initial: {0}", board.cell(0, 0));
                        //           test(board);
                        //           Console.WriteLine("final: {0}", board.cell(0, 0));
                        board.setCell(8, 8, true);
                        board.setCell(7, 8, true);
                        board.setCell(9, 8, true);

                        Console.WriteLine("================================================");
                        board.print();

                        board.runTick(); */
//            ConwayBoard board = new ConwayBoard();
//            ConwayRunner runner = new ConwayRunner(board, 1000);
//            runner.updated += (s, e) =>
//            {
//                board.print();
//            };
//            runner.start();
//            runner.stop();



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
