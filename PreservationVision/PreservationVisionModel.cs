using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using Microsoft.Win32;
using PreservationVision.Entity;

namespace PreservationVision
{
    public class PreservationVisionModel : INotifyPropertyChanged, IPreservationVisionModel
    {
        private int _timeRelaxSeconds = 300;
        private int _intervalRelaxMinutes = 55;
        private DateTime _lastWorkTime;
        private bool _isVisibleWindow = false;
        private Dispatcher _dispatcher = null;
        private Timer _workTimer;
        private Timer _relaxTimer;
        private Timer _updateTimer;
        private Action<bool> showAction;
        private DateTime _lastRelaxTime;

        public event PropertyChangedEventHandler PropertyChanged;

        public int TimeRelaxSeconds
        {
            get { return _timeRelaxSeconds; }
            set
            {
                if(_timeRelaxSeconds == value)
                    return;
                _timeRelaxSeconds = value;
                RegistryParams.SaveValue(_timeRelaxSeconds, nameof(TimeRelaxSeconds));
                if(LastRelaxTime > LastWorkTime)
                    ResetTimer(ref _relaxTimer, TimeRelaxSeconds * 1000, TimerRelaxOnElapsed);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeRelaxSeconds)));
            }
        }

        public int IntervalRelaxMinutes
        {
            get { return _intervalRelaxMinutes; }
            set
            {
                if(_intervalRelaxMinutes == value)
                    return;
                _intervalRelaxMinutes = value;
                RegistryParams.SaveValue(_intervalRelaxMinutes, nameof(IntervalRelaxMinutes));
                if (LastWorkTime > LastRelaxTime)
                {
                    LastWorkTime = DateTime.Now;
                    ResetTimer(ref _workTimer, IntervalRelaxMinutes * 60 * 1000, TimerOnElapsed);
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IntervalRelaxMinutes)));
            }
        }

        public DateTime LastWorkTime
        {
            get { return _lastWorkTime; }
            set { _lastWorkTime = value; }
        }

        public DateTime LastRelaxTime
        {
            get { return _lastRelaxTime; }
            set { _lastRelaxTime = value; }
        }

        public bool IsAutoRun
        {
            get { return RegistryParams.GetAutoRunValue(); }
            set
            {
                RegistryParams.SetAutoRunValue(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAutoRun)));
            }
        }

        public string TimeUntilEvent => LastRelaxTime > LastWorkTime ? 
            GetTimeUnitEnd(LastRelaxTime, TimeRelaxSeconds).ToString(@"hh\:mm\:ss") 
            : GetTimeUnitEnd(LastWorkTime, IntervalRelaxMinutes * 60).ToString(@"hh\:mm\:ss");

        public bool IsVisibleWindow
        {
            get { return _isVisibleWindow; }
            set
            {
                if(_isVisibleWindow == value)
                    return;
                _isVisibleWindow = value;
                _dispatcher?.Invoke(() => showAction?.Invoke(_isVisibleWindow));
                _dispatcher?.Invoke(()=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisibleWindow))));
                if(!_isVisibleWindow)
                    TimerRelaxOnElapsed(this, null);
            }
        }

        

        public PreservationVisionModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            ReadParams();
            TimerRelaxOnElapsed(this, null);
            //_lastWorkTime = DateTime.Now;
            //_workTimer = new Timer(_intervalRelaxMinutes * 60000);
            //_workTimer.Elapsed += TimerOnElapsed;
            _updateTimer = new Timer(1000) { AutoReset = true};
            _updateTimer.Elapsed += UpdateTimerOnElapsed;
            _updateTimer.Start();
        }

        private void ReadParams()
        {
            RegistryParams.ReadValue(ref _intervalRelaxMinutes, nameof(IntervalRelaxMinutes));
            RegistryParams.ReadValue(ref _timeRelaxSeconds, nameof(TimeRelaxSeconds));
        }

        

        private void UpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _dispatcher?.Invoke(() =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeUntilEvent))));
            
        }

        public PreservationVisionModel(Dispatcher dispatcher, Action<bool> showAction) : this(dispatcher)
        {
            this.showAction = showAction;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _workTimer?.Stop();
            IsVisibleWindow = true;
            LastRelaxTime = DateTime.Now;
            ResetTimer(ref _relaxTimer, TimeRelaxSeconds * 1000, TimerRelaxOnElapsed);
        }

        private void TimerRelaxOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _relaxTimer?.Stop();
            IsVisibleWindow = false;
            LastWorkTime = DateTime.Now;
            ResetTimer(ref _workTimer, IntervalRelaxMinutes * 60 * 1000, TimerOnElapsed);
        }

        void ResetTimer(ref Timer timer, int interval, ElapsedEventHandler handler)
        {
            timer?.Dispose();
            timer = new Timer(interval);
            timer.Elapsed += handler;
            timer.Start();
        }

        TimeSpan GetTimeUnitEnd(DateTime start, int secondsTimer)
        {
            return start + 
                TimeSpan.FromSeconds(secondsTimer)
                - (DateTime.Now);
        }
    }
}
