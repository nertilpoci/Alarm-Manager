using System;
using System.Timers;

namespace DandTSoftware.Timers
{
    public delegate void TimeReachedEventHandler(DateTime Time);

    /// <summary>
    /// Provides the means to detect when midnight is reached.
    /// </summary>
    public class MidnightTimer
    {
        private static Timer m_timer = null;

        /// <summary>
        /// Occurs whens midnight occurs
        /// </summary>
        public event TimeReachedEventHandler TimeReached;

        /// <summary>
        /// Starts the Timer to fire at midnight, every night (based on server time).
        /// </summary>
        public void Start()
        {
            // Subtract the current time, from midnigh (tomorrow).
            // This will return a value, which will be used to
            // SetTimer the Timer interval
            TimeSpan ts = GetMidnight().Subtract(DateTime.Now);

            // We only want the Hours, Minuters and Seconds until midnight
            TimeSpan tsMidnight = new TimeSpan(ts.Hours, ts.Minutes, ts.Seconds);

            // Set the Timer
            m_timer = new Timer(tsMidnight.TotalMilliseconds);

            // Set the event handler
            m_timer.Elapsed += new ElapsedEventHandler(t_Elapsed);

            // Start the timer
            m_timer.Start();
        }

        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            // now raise a event
            OnTimeReached();

            // Stop the orginal timer
            m_timer.Stop();

            // reset the timer
            this.Start();
        }

        private DateTime GetMidnight()
        {
            // Lets work out the next occuring midnight
            // Add 1 day and use hours 0, min 0 and second 0 (remember this is 24 hour time)
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 0, 0, 0);
        }

        /// <summary>
        /// Standard Event/Delegate handler, if its not null, fire the event
        /// </summary>
        private void OnTimeReached()
        {
            if (this.TimeReached != null)
            {
                this.TimeReached(this.GetMidnight());
            }
        }
    }
}
