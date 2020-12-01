using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HWP_Monitor.Views
{
    class TimerView
    {
        private bool RunningTimer = true;

        int seconds = 0;
        int minutes = 0;
        int hours = 0;

        Label LabelTimer;

        public TimerView(Label outputTimer)
        {
            LabelTimer = outputTimer;
        }

        public void Reset()
        {
            seconds = 0;
            minutes = 0;
            hours = 0;

            LabelTimer.Text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        public void Start()
        {
            RunningTimer = true;

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                seconds++;
                if (seconds == 59)
                {
                    minutes++;
                    seconds = 0;
                }
                if (minutes == 59)
                {
                    hours++;
                    minutes = 0;
                }

                LabelTimer.Text = string.Format("{0}:{1:00}:{2:00}", hours, minutes, seconds);

                return RunningTimer;
            });
        }

        public void Stop()
        {
            RunningTimer = false;
        }
    }
}
