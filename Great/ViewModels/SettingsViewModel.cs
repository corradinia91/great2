﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Great.Models;
using Microsoft.Exchange.WebServices.Data;
using Nager.Date;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Linq;

namespace Great.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Sets and gets the Country property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public CountryCode Country
        {
            get
            {
                return UserSettings.Localization.Country;
            }

            set
            {
                if (UserSettings.Localization.Country == value)
                    return;

                var oldValue = UserSettings.Localization.Country;
                UserSettings.Localization.Country = value;

                RaisePropertyChanged(nameof(Country), oldValue, value);
            }
        }

        /// <summary>
        /// Sets and gets the EmailAddress property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public string EmailAddress
        {
            get
            {
                return UserSettings.Email.EmailAddress;
            }

            set
            {
                if (UserSettings.Email.EmailAddress == value)
                    return;
                
                var oldValue = UserSettings.Email.EmailAddress;
                UserSettings.Email.EmailAddress = value;

                RaisePropertyChanged(nameof(EmailAddress), oldValue, value);
            }
        }

        /// <summary>
        /// Sets and gets the EmailPassword property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public string EmailPassword
        {
            get
            {
                return UserSettings.Email.EmailPassword;
            }

            set
            {
                UserSettings.Email.EmailPassword = value;
                RaisePropertyChanged(nameof(EmailPassword));
            }
        }

        /// <summary>
        /// Sets and gets the AutoAddFactories property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public bool AutoAddFactories
        {
            get
            {
                return UserSettings.Advanced.AutoAddFactories;
            }

            set
            {
                UserSettings.Advanced.AutoAddFactories = value;
                RaisePropertyChanged(nameof(AutoAddFactories));
            }
        }


        /// <summary>
        /// Sets and gets the SaturdayColor property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public Color SaturdayColor
        {
            get
            {
                return UserSettings.Themes.SaturdayColor;
            }

            set
            {
                UserSettings.Themes.SaturdayColor = value;
                RaisePropertyChanged(nameof(SaturdayColor));
            }
        }

        /// <summary>
        /// Sets and gets the SundayColor property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public Color SundayColor
        {
            get
            {
                return UserSettings.Themes.SundayColor;
            }

            set
            {
                UserSettings.Themes.SundayColor = value;
                RaisePropertyChanged(nameof(SundayColor));
            }
        }

        /// <summary>
        /// Sets and gets the SickColor property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public Color SickColor
        {
            get
            {
                return UserSettings.Themes.SickColor;
            }

            set
            {
                UserSettings.Themes.SickColor = value;
                RaisePropertyChanged(nameof(SickColor));
            }
        }

        /// <summary>
        /// Sets and gets the HomeworkingColor property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public Color HomeWorkingColor
        {
            get
            {
                return UserSettings.Themes.HomeWorkColor;
            }

            set
            {
                UserSettings.Themes.HomeWorkColor = value;
                RaisePropertyChanged(nameof(HomeWorkingColor));
            }
        }

        /// <summary>
        /// Sets and gets the PendingVacationColor property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public Color PendingVacationColor
        {
            get
            {
                return UserSettings.Themes.PendingVacationColor;
            }

            set
            {
                UserSettings.Themes.PendingVacationColor = value;
                RaisePropertyChanged(nameof(PendingVacationColor));
            }
        }

        /// <summary>
        /// Sets and gets the VacationColor property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public Color VacationColor
        {
            get
            {
                return UserSettings.Themes.VacationColor;
            }

            set
            {
                UserSettings.Themes.VacationColor = value;
                RaisePropertyChanged(nameof(VacationColor));
            }
        }

        /// <summary>
        /// Sets and gets the Holiday property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public Color HolidayColor
        {
            get
            {
                return UserSettings.Themes.HolidayColor;
            }

            set
            {
                UserSettings.Themes.HolidayColor = value;
                RaisePropertyChanged(nameof(HolidayColor));
            }
        }

        /// <summary>
        /// Sets and gets the VacationColor property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public ESkin Skin
        {
            get
            {
                return UserSettings.Themes.Skin;
            }

            set
            {
                UserSettings.Themes.Skin = value;
                RaisePropertyChanged(nameof(Skin));
            }
        }

        /// <summary>
        /// Sets and gets the AutoAssignFactories property.
        /// Changes to that property's value raise the PropertyChanged event.         
        /// </summary>
        public bool AutoAssignFactories
        {
            get
            {
                return UserSettings.Advanced.AutoAssignFactories;
            }

            set
            {
                UserSettings.Advanced.AutoAssignFactories = value;
                RaisePropertyChanged(nameof(AutoAssignFactories));
            }
        }

        public string FDLCancelRequestRecipients
        {
            get
            {
                string recipients = string.Empty;

                if(UserSettings.Email.Recipients.FDLCancelRequest != null)
                {
                    foreach (string address in UserSettings.Email.Recipients.FDLCancelRequest)
                        recipients += recipients == string.Empty ? address : "; " + address;
                }
                return recipients;
            }
            set
            {
                StringCollection recipients = new StringCollection();
                string[] addresses = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < addresses.Length; i++)
                    recipients.Add(addresses[i].Trim());
                UserSettings.Email.Recipients.FDLCancelRequest = recipients;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the SettingsViewModel class.
        /// </summary>
        public SettingsViewModel()
        {
            
        }
    }
}