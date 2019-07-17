﻿using GalaSoft.MvvmLight;
using Great.Models.Database;
using LiveCharts;
using System;
using System.Linq;
using System.Collections.Generic;
using Great.ViewModels.Database;
using Great.Utils.Extensions;
using LiveCharts.Wpf;
using GalaSoft.MvvmLight.Command;
using Great.Utils.Messages;

namespace Great.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        #region Properties
        private Func<ChartPoint, string> HoursLabel { get; set; }

        private SeriesCollection _Factories;
        public SeriesCollection Factories
        {
            get => _Factories;
            set => Set(ref _Factories, value);
        }

        private SeriesCollection _Hours;
        public SeriesCollection Hours
        {
            get => _Hours;
            set => Set(ref _Hours, value);
        }

        private SeriesCollection _days;
        public SeriesCollection Days
        {
            get => _days;
            set => Set(ref _days, value);
        }



        private int _SelectedYear;
        public int SelectedYear
        {
            get => _SelectedYear;
            set
            {
                Set(ref _SelectedYear, value);
                RefreshAllData();
            }
        }

        private string[] _MonthsLabels;
        public string[] MonthsLabels
        {
            get => _MonthsLabels;
            set => Set(ref _MonthsLabels, value);
        }
        #endregion

        #region Commands Definitions
        public RelayCommand NextYearCommand { get; set; }
        public RelayCommand PreviousYearCommand { get; set; }
        #endregion

        public StatisticsViewModel()
        {
            Factories = new SeriesCollection();
            Hours = new SeriesCollection();
            Days = new SeriesCollection();

            NextYearCommand = new RelayCommand(() => SelectedYear++);
            PreviousYearCommand = new RelayCommand(() => SelectedYear--);

            HoursLabel = chartPoint => chartPoint.Y.ToString("N2") + "h";

            SelectedYear = DateTime.Now.Year;

            MonthsLabels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            MessengerInstance.Register<ItemChangedMessage<TimesheetEVM>>(this, RefreshTimesheet);
            MessengerInstance.Register<DeletedItemMessage<TimesheetEVM>>(this, RefreshTimesheet);
            MessengerInstance.Register<ItemChangedMessage<DayEVM>>(this, RefreshDay);
        }

        private void RefreshAllData()
        {
            using (new WaitCursor())
            {
                LoadFactoriesData();
                LoadHoursData();
                LoadDayStatistic();
            }
        }

        private void LoadFactoriesData()
        {
            Dictionary<string, int> factoriesData = new Dictionary<string, int>();
            Factories.Clear();

            using (DBArchive db = new DBArchive())
            {
                string YearStr = SelectedYear.ToString();

                factoriesData = (from fdl in db.FDLs
                                 from timesheets in fdl.Timesheets
                                 where fdl.Id.Substring(0, 4) == YearStr && fdl.Factory1 != null
                                 group fdl.Factory1 by fdl.Factory1.Name into factories
                                 select factories).ToDictionary(x => x.Key, x => x.Count());

                foreach (KeyValuePair<string, int> entry in factoriesData)
                {
                    PieSeries factory = new PieSeries
                    {
                        Title = entry.Key,
                        Values = new ChartValues<int> { entry.Value },
                        DataLabels = true
                    };

                    Factories.Add(factory);
                }
            }
        }

        private void LoadHoursData()
        {
            Dictionary<string, int> factoriesData = new Dictionary<string, int>();

            using (DBArchive db = new DBArchive())
            {
                long startDate = new DateTime(SelectedYear, 1, 1).ToUnixTimestamp();
                long endDate = new DateTime(SelectedYear, 12, 31).ToUnixTimestamp();
                var Days = db.Days.Where(day => day.Timestamp >= startDate && day.Timestamp <= endDate).ToList().Select(d => new DayEVM(d));

                var MontlyHours = Days?.GroupBy(d => d.Date.Month)
                                       .Select(g => new
                                       {
                                           TotalTime = g.Sum(x => (x.TotalTime ?? 0)),
                                           Ordinary = g.Sum(x => (x.TotalTime ?? 0) - (x.Overtime34 ?? 0) - (x.Overtime35 ?? 0) - (x.Overtime50 ?? 0) - (x.Overtime100 ?? 0)),
                                           Overtime34 = g.Sum(x => x.Overtime34 ?? 0),
                                           Overtime35 = g.Sum(x => x.Overtime35 ?? 0),
                                           Overtime50 = g.Sum(x => x.Overtime50 ?? 0),
                                           Overtime100 = g.Sum(x => x.Overtime100 ?? 0),
                                           HoursOfLeave = g.Sum(x => (x.EType== EDayType.VacationPaidDay || x.EType == EDayType.VacationPaidDay)? 8: (x.HoursOfLeave ?? 0))
                                       });

                ChartValues<float> TotalTimeValues = new ChartValues<float>();
                ChartValues<float> OrdinaryValues = new ChartValues<float>();
                ChartValues<float> Overtime34Values = new ChartValues<float>();
                ChartValues<float> Overtime35Values = new ChartValues<float>();
                ChartValues<float> Overtime50Values = new ChartValues<float>();
                ChartValues<float> Overtime100Values = new ChartValues<float>();
                ChartValues<float> HoursOfLeave = new ChartValues<float>();

                foreach (var month in MontlyHours)
                {
                    TotalTimeValues.Add(month.TotalTime);
                    OrdinaryValues.Add(month.Ordinary);
                    Overtime34Values.Add(month.Overtime34);
                    Overtime35Values.Add(month.Overtime35);
                    Overtime50Values.Add(month.Overtime50);
                    Overtime100Values.Add(month.Overtime100);
                    HoursOfLeave.Add(month.HoursOfLeave);
                }

                Hours = new SeriesCollection()
                {
                    new StackedColumnSeries()
                    {
                        Title = "Ordinary Hours",
                        Values = OrdinaryValues,
                        DataLabels = false,
                        LabelPoint = HoursLabel
                    },
                    new StackedColumnSeries()
                    {
                        Title = "Overtime 34%",
                        Values = Overtime34Values,
                        DataLabels = false,
                        LabelPoint = HoursLabel
                    },
                    new StackedColumnSeries()
                    {
                        Title = "Overtime 35%",
                        Values = Overtime35Values,
                        DataLabels = false,
                        LabelPoint = HoursLabel
                    },
                    new StackedColumnSeries()
                    {
                        Title = "Overtime 50%",
                        Values = Overtime50Values,
                        DataLabels = false,
                        LabelPoint = HoursLabel
                    },
                    new StackedColumnSeries()
                    {
                        Title = "Overtime 100%",
                        Values = Overtime100Values,
                        DataLabels = false,
                        LabelPoint = HoursLabel
                    },

                    new StackedColumnSeries()
                    {
                        Title = "Hours of leave",
                        Values = HoursOfLeave,
                        DataLabels = false,
                        LabelPoint = HoursLabel
                    },
                    new LineSeries
                    {
                        Title = "Total",
                        Values = TotalTimeValues,
                        DataLabels = true,
                        LabelPoint = HoursLabel
                    }
                };
            }
        }

        private void LoadDayStatistic()
        {
            Days.Clear();

            using (DBArchive db = new DBArchive())
            {
                string YearStr = SelectedYear.ToString();

                //count all trip days
                var businessTripDays = (from d in db.Days
                                        from ts in d.Timesheets
                                        where ts.FDL1 != null
                                        && ts.FDL1.Id.Substring(0, 4) == YearStr
                                        select d).Count();

                //count all office days without fdl
                var officeDays = (from d in db.Days
                                  from ts in d.Timesheets
                                  where d.DayType.Id == (long)EDayType.WorkDay && ts.FDL1 == null
                                  select d
                            ).Count();

                //count all home working days 
                var homeWorkingDays = (from d in db.Days
                                       where d.DayType.Id == (long)EDayType.HomeWorking
                                       select d
                            ).Count();

                //count all sick days without
                var sickDays = (from d in db.Days
                                where d.DayType.Id == (long)EDayType.SickLeave
                                select d
                            ).Count();

                //count all vacations days without
                var vacationDays = (from d in db.Days
                                    where d.DayType.Id == (long)EDayType.VacationDay
                                    select d
                            ).Count();

                //count all vacations paid days without
                var vacationPaidDays = (from d in db.Days
                                    where d.DayType.Id == (long)EDayType.VacationPaidDay
                                    select d
                            ).Count();

                Days.Add(new PieSeries
                {
                    Title = "Office",
                    Values = new ChartValues<int> { officeDays },
                    DataLabels = true
                });

                Days.Add(new PieSeries
                {
                    Title = "Home Work",
                    Values = new ChartValues<int> { homeWorkingDays },
                    DataLabels = true
                });

                Days.Add(new PieSeries
                {
                    Title = "Business Trip",
                    Values = new ChartValues<int> { businessTripDays },
                    DataLabels = true
                });

                Days?.Add(new PieSeries
                {
                    Title = "Sick",
                    Values = new ChartValues<int> { sickDays },
                    DataLabels = true
                });

               Days?.Add(new PieSeries
                {
                    Title = "Vacation",
                    Values = new ChartValues<int> { vacationDays },
                    DataLabels = true
                });

                Days?.Add(new PieSeries
                {
                    Title = "Paid Vacation",
                    Values = new ChartValues<int> { vacationPaidDays },
                    DataLabels = true
                });
            }


        }
        private void RefreshTimesheet(ItemChangedMessage<TimesheetEVM> item)
        {
            RefreshAllData();
        }
        private void RefreshTimesheet(DeletedItemMessage<TimesheetEVM> item)
        {
            RefreshAllData();
        }
        private void RefreshDay(ItemChangedMessage<DayEVM> item)
        {
            RefreshAllData();
        }
    }
}
