﻿using Great.Utils;
using Itenso.TimePeriod;
using System;

namespace Great.Models
{
    public partial class Timesheet
    {
        #region Converted Properties
        public DateTime Date
        {
            get { return UnixTimestamp.GetDateTime(Timestamp); }
            set { Timestamp = UnixTimestamp.GetTimestamp(value); }
        }

        public TimeSpan? TravelStartTimeAM_t
        {
            get { return TravelStartTimeAM.HasValue ? TimeSpan.FromSeconds(TravelStartTimeAM.Value) : (TimeSpan?)null; }
            set { TravelStartTimeAM = (long)value?.TotalSeconds; }
        }

        public TimeSpan? TravelEndTimeAM_t
        {
            get { return TravelEndTimeAM.HasValue ? TimeSpan.FromSeconds(TravelEndTimeAM.Value) : (TimeSpan?)null; }
            set { TravelEndTimeAM = (long)value?.TotalSeconds; }
        }

        public TimeSpan? TravelStartTimePM_t
        {
            get { return TravelStartTimePM.HasValue ? TimeSpan.FromSeconds(TravelStartTimePM.Value) : (TimeSpan?)null; }
            set { TravelStartTimePM = (long)value?.TotalSeconds; }
        }

        public TimeSpan? TravelEndTimePM_t
        {
            get { return TravelEndTimePM.HasValue ? TimeSpan.FromSeconds(TravelEndTimePM.Value) : (TimeSpan?)null; }
            set { TravelEndTimePM = (long)value?.TotalSeconds; }
        }

        public TimeSpan? WorkStartTimeAM_t
        {
            get { return WorkStartTimeAM.HasValue ? TimeSpan.FromSeconds(WorkStartTimeAM.Value) : (TimeSpan?)null; }
            set { WorkStartTimeAM = (long)value?.TotalSeconds; }
        }

        public TimeSpan? WorkEndTimeAM_t
        {
            get { return WorkEndTimeAM.HasValue ? TimeSpan.FromSeconds(WorkEndTimeAM.Value) : (TimeSpan?)null; }
            set { WorkEndTimeAM = (long)value?.TotalSeconds; }
        }

        public TimeSpan? WorkStartTimePM_t
        {
            get { return WorkStartTimePM.HasValue ? TimeSpan.FromSeconds(WorkStartTimePM.Value) : (TimeSpan?)null; }
            set { WorkStartTimePM = (long)value?.TotalSeconds; }
        }

        public TimeSpan? WorkEndTimePM_t
        {
            get { return WorkEndTimePM.HasValue ? TimeSpan.FromSeconds(WorkEndTimePM.Value) : (TimeSpan?)null; }
            set { WorkEndTimePM = (long)value?.TotalSeconds; }
        }
        #endregion

        #region Time Periods
        public TimePeriodCollection TimePeriods
        {
            get
            {
                TimePeriodCollection timePeriods = new TimePeriodCollection();

                if (WorkingPeriods != null)
                    timePeriods.AddAll(WorkingPeriods);

                if (TravelPeriods != null)
                    timePeriods.AddAll(TravelPeriods);

                return timePeriods.Count > 0 ? timePeriods : null;
            }
        }

        public TimePeriodCollection WorkingPeriods
        {
            get
            {
                TimePeriodCollection workingPeriods = new TimePeriodCollection();
                
                if (WorkEndTimeAM_t.HasValue && WorkStartTimeAM_t.HasValue)
                    workingPeriods.Add(new TimeRange(Date + WorkStartTimeAM_t.Value, Date + WorkEndTimeAM_t.Value));
                if (WorkEndTimePM_t.HasValue && WorkStartTimePM_t.HasValue)
                    workingPeriods.Add(new TimeRange(Date + WorkStartTimePM_t.Value, Date + WorkEndTimePM_t.Value));
                
                return workingPeriods.Count > 0 ? workingPeriods : null;
            }
        }

        public TimePeriodCollection TravelPeriods
        {
            get
            {
                TimePeriodCollection travelPeriods = new TimePeriodCollection();

                if (TravelStartTimeAM_t.HasValue && TravelEndTimeAM_t.HasValue && !WorkStartTimeAM_t.HasValue && !WorkEndTimeAM_t.HasValue)
                    travelPeriods.Add(new TimeRange(Date + TravelStartTimeAM_t.Value, Date + TravelEndTimeAM_t.Value));
                if (TravelStartTimeAM_t.HasValue && !TravelEndTimeAM_t.HasValue && WorkStartTimeAM_t.HasValue && WorkEndTimeAM_t.HasValue)
                    travelPeriods.Add(new TimeRange(Date + TravelStartTimeAM_t.Value, Date + WorkStartTimeAM_t.Value));
                if (!TravelStartTimeAM_t.HasValue && TravelEndTimeAM_t.HasValue && WorkStartTimeAM_t.HasValue && WorkEndTimeAM_t.HasValue)
                    travelPeriods.Add(new TimeRange(Date + WorkEndTimeAM_t.Value, Date + TravelEndTimeAM_t.Value));

                if (TravelStartTimePM_t.HasValue && TravelEndTimePM_t.HasValue && !WorkStartTimePM_t.HasValue && !WorkEndTimePM_t.HasValue)
                    travelPeriods.Add(new TimeRange(Date + TravelStartTimePM_t.Value, Date + TravelEndTimePM_t.Value));
                if (TravelStartTimePM_t.HasValue && !TravelEndTimePM_t.HasValue && WorkStartTimePM_t.HasValue && WorkEndTimePM_t.HasValue)
                    travelPeriods.Add(new TimeRange(Date + TravelStartTimePM_t.Value, Date + WorkStartTimePM_t.Value));
                if (!TravelStartTimePM_t.HasValue && TravelEndTimePM_t.HasValue && WorkStartTimePM_t.HasValue && WorkEndTimePM_t.HasValue)
                    travelPeriods.Add(new TimeRange(Date + WorkEndTimePM_t.Value, Date + TravelEndTimePM_t.Value));
                
                return travelPeriods.Count > 0 ? travelPeriods : null;
            }
        }
        #endregion

        #region Overtimes
        public TimeSpan? Overtime34
        {
            get
            {
                TimeSpan overtime34 = new TimeSpan();

                if (Date.DayOfWeek == DayOfWeek.Saturday)
                {
                    if (TimePeriods?.TotalDuration.Hours > 4)
                        overtime34 = TimeSpan.FromHours(4);
                    else
                        overtime34 = TimePeriods != null ? TimePeriods.TotalDuration : new TimeSpan();
                }
                else
                {
                    if (TimePeriods?.TotalDuration.Hours > 8)
                    {
                        if (TimePeriods?.TotalDuration.Hours >= 10)
                            overtime34 = TimeSpan.FromHours(2);
                        else
                            overtime34 = TimePeriods.TotalDuration - TimeSpan.FromHours(8);
                    }
                }

                return overtime34.Ticks > 0 ? overtime34 : (TimeSpan?)null;
            }
        }

        public TimeSpan? Overtime35
        {
            get
            {
                TimeSpan overtime35 = new TimeSpan();
                TimePeriodSubtractor<TimeRange> subtractor = new TimePeriodSubtractor<TimeRange>();

                TimePeriodCollection overtime35period = new TimePeriodCollection() {
                    new TimeRange(Date, Date + new TimeSpan(6, 0, 0)),
                    new TimeRange(Date + new TimeSpan(22, 0, 0), Date + new TimeSpan(23, 59, 59))
                };
                                
                ITimePeriodCollection difference = subtractor.SubtractPeriods(overtime35period, TimePeriods);

                overtime35 = subtractor.SubtractPeriods(overtime35period, difference).TotalDuration;
                
                return overtime35.Ticks > 0 ? overtime35 : (TimeSpan?)null;
            }
        }

        public TimeSpan? Overtime50
        {
            get
            {
                TimeSpan overtime50 = new TimeSpan();

                if (Date.DayOfWeek == DayOfWeek.Saturday && TimePeriods?.TotalDuration.Hours > 4)
                {
                    overtime50 = TimePeriods.TotalDuration - TimeSpan.FromHours(4);
                }
                else
                {
                    if (TimePeriods?.TotalDuration.Hours > 10)
                    {
                        overtime50 = TimePeriods.TotalDuration - TimeSpan.FromHours(10);
                    }
                }

                return overtime50.Ticks > 0 ? overtime50 : (TimeSpan?)null;
            }
        }

        public TimeSpan? Overtime100
        {
            get
            {
                TimeSpan overtime100 = new TimeSpan();

                if (Date.DayOfWeek == DayOfWeek.Sunday) //TODO: aggiungere festivi
                {
                    overtime100 = TimePeriods != null ? TimePeriods.TotalDuration : new TimeSpan();
                }

                return overtime100.Ticks > 0 ? overtime100 : (TimeSpan?)null;
            }
        }
        #endregion

        #region Display Properties        
        public string FDL_Display { get { return "TODO"; } }
        #endregion

        public Timesheet Clone()
        {
            return new Timesheet()
            {
                Id = this.Id,
                Timestamp = this.Timestamp,
                TravelStartTimeAM = this.TravelStartTimeAM,
                TravelEndTimeAM = this.TravelEndTimeAM,
                WorkStartTimeAM = this.WorkStartTimeAM,
                WorkEndTimeAM = this.WorkEndTimeAM,
                TravelStartTimePM = this.TravelStartTimePM,
                TravelEndTimePM = this.TravelEndTimePM,
                WorkStartTimePM = this.WorkStartTimePM,
                WorkEndTimePM = this.WorkEndTimePM,
                FDL = this.FDL
            };
        }
    }
}