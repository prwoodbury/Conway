using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conway
{
    /**
     * Provides logic for asynchronously ticking a ConwayBoard at specified
     * intervals.
     */
    class ConwayRunner
    {
        //===================================================================
        // Public
        //===================================================================

        /**
         * Constructor.
         */
        public ConwayRunner(ConwayBoard board, int msPerTick)
        {
            this.board = board;
            this.msPerTick = msPerTick;
        }

        /**
         * Begins running the simulation.
         */
        public async Task start()
        {
            // Can't start if already running
            if (running) return;
            running = true;

            // Notify subscribers
            started?.Invoke(this, EventArgs.Empty);

            Action work = async () =>
            {
                bool done = false;
                int initialSpeed = msPerTick;

                // poll for events which interrupt the simulation wait step
                Action work2 = async () =>
                {
                    while (true)
                    {
                        // simulation stopped
                        if (!running)
                        {
                            done = true;
                            break;
                        }

                        // speed changed
                        lock (msPerTick_lock) {
                            if (msPerTick != initialSpeed)
                            {
                                break;
                            }
                        }
                    }
                };

                Task waitTask = Task.Run(work2);

                // Run board logic at scheduled intervals until simulation stopped
                do
                {
                    // Restart wait task if completed (e.g. due to speed change)
                    if (waitTask.IsCompleted) waitTask = Task.Run(work2);

                    // Wait until next scheduled update.
                    // Stop waiting when either the specified delay period has expired,
                    // the simulation has been stopped,
                    // or the speed has changed.
                    await Task.WhenAny(Task.Delay(msPerTick), waitTask);

                    // If waiting ended due to speed change, restart loop
                    // without running board update (to avoid "fast forward"
                    // effect when speed is rapidly and repeatedly changed)
                    if (initialSpeed != msPerTick)
                    {
                        initialSpeed = msPerTick;
                        continue;
                    }

                    // Stop without update if simulation stopped
                    if (done) break;

                    // Run next board update
                    board.runTick();

                    // Notify subscribers of update
                    updated?.Invoke(this, EventArgs.Empty);
                } while (true);
            };
            await Task.Run(work);
        }

        /**
         * Stops the simulation.
         */
        public void stop()
        {
            // Can't stop if not running
            if (!running) return;
            running = false;

            // Notify subscribers
            stopped?.Invoke(this, EventArgs.Empty);
        }

        /**
        * Sets number of milliseconds per simulation tick.
        */
            public void setMsPerTick(int msPerTick)
        {
            lock (msPerTick_lock)
            {
                this.msPerTick = msPerTick;
            }
        }

        /**
         * Returns true if simulation running.
         */
        public bool isRunning
        {
            get { return running; }
        }

        //===================================================================
        // Public event handlers
        //===================================================================

        public event EventHandler started;
        public event EventHandler stopped;
        public event EventHandler updated;

        //===================================================================
        // Private
        //===================================================================

        /**
         * The target board.
         */
        private ConwayBoard board;

        /**
         * Flag indicating whether currently running.
         */
        private bool running = false;

        /**
         * Number of milliseconds between board updates.
         */
        public int msPerTick;
        private object msPerTick_lock = new object();

    }
}
