using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Conway
{
    public partial class Form1 : Form
    {
        private static ConwayBoard board = new ConwayBoard();
        private static ConwayBoard initialBoard = new ConwayBoard();
        private static ConwayRunner runner = new ConwayRunner(board, 200);

        private static readonly int cellW = 8;
        private static readonly int cellH = 8;

        // Minimum and maximum speed of simulation in ticks per second
        private static readonly int minSpeed = 1;
        private static readonly int maxSpeed = 100;

        public Form1()
        {
            InitializeComponent();

            // Set event delegates
            runner.updated += onSimulationUpdated;
            runner.started += onSimulationStarted;
            runner.stopped += onSimulationStopped;

            // perform the UI tasks associated with stopping simulation
            // (e.g. "start" button text)
            onSimulationStopped(this, EventArgs.Empty);

            // Set up min/max speeds
            speedBox.Minimum = minSpeed;
            speedBox.Maximum = maxSpeed;
            speedBar.Minimum = minSpeed;
            speedBar.Maximum = maxSpeed;
            speedBar.TickFrequency = 10;
            setSimulationSpeed(minSpeed);

//            board.setCell(5, 5, true);
//            board.setCell(4, 5, true);
//            board.setCell(6, 5, true);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Draw the board
            Graphics g = e.Graphics;
            board.draw(g, cellW, cellH);
        }

        private void startSimulation()
        {
            runner.start();
        }

        private void stopSimulation()
        {
            runner.stop();
        }

        private void onSimulationUpdated(object sender, EventArgs e)
        {
            // redraw display
            pictureBox1.Invalidate();
        }

        private void onSimulationStarted(object sender, EventArgs e)
        {
            startStopButton.Text = "Stop";
        }

        private void onSimulationStopped(object sender, EventArgs e)
        {
            startStopButton.Text = "Start";
        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
            if (runner.isRunning)
            {
                stopSimulation();
            }
            else
            {
                startSimulation();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            simulationClicked(sender, e);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            simulationClicked(sender, e);
        }

        private void simulationClicked(object sender, MouseEventArgs e)
        {
            // ignore clicks while simulation is running
            //if (runner.isRunning) return;

            int x = e.X / cellW;
            int y = e.Y / cellH;

            // ignore out-of-range clicks
            if ((x < 0) || (y < 0)
                || (x >= ConwayBoard.width)
                || (y >= ConwayBoard.height)) return;

            if (e.Button == MouseButtons.Left)
            {
                board.setCell(x, y, true);
            }
            else if (e.Button == MouseButtons.Right)
            {
                board.setCell(x, y, false);
            }

            // redraw
            pictureBox1.Invalidate();
        }

        private void speedBar_ValueChanged(object sender, EventArgs e)
        {
            setSimulationSpeed(speedBar.Value);
        }

        private void speedBox_ValueChanged(object sender, EventArgs e)
        {
            setSimulationSpeed((int)speedBox.Value);
        }

        // "lock" to avoid infinite recursion when changing speeds
        // (since ValueChanged is emitted when a control's value
        // is modified not just by the user, but by the code as well)
        private bool changingSpeed = false;
        private void setSimulationSpeed(int ticksPerSecond)
        {
            if (changingSpeed) return;
            changingSpeed = true;

            speedBar.Value = ticksPerSecond;
            speedBox.Value = ticksPerSecond;

            runner.setMsPerTick(1000 / ticksPerSecond);

            changingSpeed = false;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            clearSimulation();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearSimulation();
        }

        private void clearSimulation()
        {
            if (runner.isRunning) stopSimulation();
            board.clearBoard();
            // redraw
            pictureBox1.Invalidate();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopSimulation();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Pattern files|*.pat";
            dialog.Title = "Load pattern";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
 //               stopSimulation();
                string boardStr = System.IO.File.ReadAllText(dialog.FileName);
                board.fromString(boardStr);
                pictureBox1.Invalidate();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopSimulation();

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Pattern files|*.pat";
            dialog.Title = "Save pattern";
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                string boardStr = board.asString();
                System.IO.File.WriteAllText(dialog.FileName, boardStr);
//                using (System.IO.StreamWriter file
//                    = new System.IO.StreamWriter(dialog.FileName))
//                {
//
//                }
            }
        }
    }
}
