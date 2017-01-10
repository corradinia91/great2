using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Great.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;

namespace Great.ViewModels
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public const int MIN_YEAR = 1900;
        public const int MAX_YEAR = 2100;

        #region Properties
        /// <summary>
        /// The <see cref="CurrentYear" /> property's name.
        /// </summary>

        private int _currentYear = DateTime.Now.Year;

        /// <summary>
        /// Sets and gets the CurrentYear property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public int CurrentYear
        {
            get
            {
                return _currentYear;
            }

            set
            {
                if (_currentYear == value)
                {
                    return;
                }
                
                var oldValue = _currentYear;

                if (value < MIN_YEAR)
                    _currentYear = MIN_YEAR;
                else if (value > MAX_YEAR)
                    _currentYear = MAX_YEAR;
                else
                    _currentYear = value;

                RaisePropertyChanged(nameof(CurrentYear), oldValue, value);

                UpdateWorkingDays();
            }
        }

        /// <summary>
        /// The <see cref="CurrentMonth" /> property's name.
        /// </summary>

        private int _currentMonth = DateTime.Now.Month;

        /// <summary>
        /// Sets and gets the CurrentMonth property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public int CurrentMonth
        {
            get
            {
                return _currentMonth;
            }

            set
            {
                if (_currentMonth == value)
                {
                    return;
                }

                var oldValue = _currentMonth;
                _currentMonth = value;
                RaisePropertyChanged(nameof(CurrentMonth), oldValue, value);

                UpdateWorkingDays();
            }
        }

        /// <summary>
        /// The <see cref="WorkingDays" /> property's name.
        /// </summary>

        private IList<WorkingDay> _workingDays;

        /// <summary>
        /// Sets and gets the WorkingDays property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public IList<WorkingDay> WorkingDays
        {
            get
            {
                return _workingDays;
            }

            set
            {
                _workingDays = value;
                RaisePropertyChanged(nameof(WorkingDays));
            }
        }

        /// <summary>
        /// The <see cref="SelectedWorkingDay" /> property's name.
        /// </summary>

        private WorkingDay _selectedWorkingDay;

        /// <summary>
        /// Sets and gets the SelectedWorkingDay property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public WorkingDay SelectedWorkingDay
        {
            get
            {
                return _selectedWorkingDay;
            }

            set
            {
                var oldValue = _selectedWorkingDay;
                _selectedWorkingDay = value;
                RaisePropertyChanged(nameof(SelectedWorkingDay), oldValue, value);
            }
        }

        private DBEntities _db { get; set; }
        #endregion

        #region Commands
        public RelayCommand NextYearCommand { get; set; }
        public RelayCommand PreviousYearCommand { get; set; }
        public RelayCommand<int> SetMonthCommand { get; set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(DBEntities db)
        {
            _db = db;

            NextYearCommand = new RelayCommand(SetNextYear);
            PreviousYearCommand = new RelayCommand(SetPreviousYear);
            SetMonthCommand = new RelayCommand<int>(SetMonth);

            UpdateWorkingDays();
        }

        private void UpdateWorkingDays()
        {
            IList<WorkingDay> days = new List<WorkingDay>();
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            
            foreach (DateTime day in AllDatesInMonth(CurrentYear, CurrentMonth))
            {   
                WorkingDay workingDay = new WorkingDay
                {
                    WeekNr = cal.GetWeekOfYear(day, dfi.CalendarWeekRule, dfi.FirstDayOfWeek),
                    Day = day,
                    Timesheets = _db.Timesheets.SqlQuery("select * from Timesheet where Date = @date", new SQLiteParameter("date", day.ToString("yyyy-MM-dd"))).ToList()
                };

                days.Add(workingDay);
            }

            WorkingDays = days;
        }

        public static IEnumerable<DateTime> AllDatesInMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= days; day++)
            {
                yield return new DateTime(year, month, day);
            }
        }

        private void SetNextYear()
        {
            CurrentYear++;
        }

        private void SetPreviousYear()
        {
            CurrentYear--;
        }

        private void SetMonth(int month)
        {
            if (month > 0 && month <= 12)
                CurrentMonth = month;
        }
    }
}