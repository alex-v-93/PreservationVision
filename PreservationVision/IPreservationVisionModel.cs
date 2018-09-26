using System;

namespace PreservationVision
{
    public interface IPreservationVisionModel
    {
        int TimeRelaxSeconds { get; set; }
        int IntervalRelaxMinutes { get; set; }
        DateTime LastWorkTime { get; set; }
        DateTime LastRelaxTime { get; set; }
        string TimeUntilEvent { get; }
        bool IsVisibleWindow { get; set; }
        bool IsAutoRun { get; set; }
    }
}