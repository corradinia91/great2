﻿using Great2.Models;
using Great2.Models.Database;
using Great2.Models.DTO;
using Great2.Models.Interfaces;
using Great2.Utils;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Great2.ViewModels.Database
{
    public class ExpenseAccountEVM : EntityViewModelBase, IFDLFile
    {
        #region Properties
        private long _Id;
        public long Id
        {
            get => _Id;
            set => Set(ref _Id, value);
        }

        private string _FDL;
        public string FDL
        {
            get => _FDL;
            set => Set(ref _FDL, value);
        }

        public string Year { get => FDL?.Substring(0, 4);}

        private long? _CdC;
        public long? CdC
        {
            get => _CdC;
            set => Set(ref _CdC, value);
        }

        private string _Currency;
        public string Currency
        {
            get => _Currency;
            set
            {
                SetAndCheckChanged(ref _Currency, value);
                CurrencyCode = CurrencyCodeMapper.GetSymbol(_Currency);
            }
        }

        private double? _DeductionAmount;
        public double? DeductionAmount
        {
            get => _DeductionAmount;
            set
            {
                SetAndCheckChanged(ref _DeductionAmount, value);
                RaisePropertyChanged(nameof(DeductionAmount_Display));
            }
        }

        private string _Notes;
        public string Notes
        {
            get => _Notes;
            set => SetAndCheckChanged(ref _Notes, value);
        }

        private long _Status;
        public long Status
        {
            get => _Status;
            set
            {
                Set(ref _Status, value);
                IsNew = _Status == 0;
                RaisePropertyChanged(nameof(EStatus));
            }
        }

        private string _LastError;
        public string LastError
        {
            get => _LastError;
            set => Set(ref _LastError, value);
        }

        private string _FileName;
        public string FileName
        {
            get => _FileName;
            set => Set(ref _FileName, value);
        }

        private bool _IsRefunded;
        public bool IsRefunded
        {
            get => _IsRefunded;
            set => Set(ref _IsRefunded, value);
        }

        private bool _NotifyAsNew;
        public bool NotifyAsNew
        {
            get => _NotifyAsNew;
            set
            {
                Set(ref _NotifyAsNew, value);
                RaisePropertyChanged(nameof(FDL_New_Display));
            }
        }

        private bool _IsCompiled;
        public bool IsCompiled
        {
            get => _IsCompiled;
            set => Set(ref _IsCompiled, value);
        }

        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set => Set(ref _IsReadOnly, value);
        }

        private long? _LastSAPSendTimestamp;
        public long? LastSAPSendTimestamp
        {
            get => _LastSAPSendTimestamp;
            set => Set(ref _LastSAPSendTimestamp, value);
        }

        private CurrencyDTO _Currency1;
        public CurrencyDTO Currency1
        {
            get => _Currency1;
            set => Set(ref _Currency1, value);
        }

        private ObservableCollectionEx<ExpenseEVM> _Expenses;
        public ObservableCollectionEx<ExpenseEVM> Expenses
        {
            get => _Expenses;
            set => Set(ref _Expenses, value);
        }

        private FDLStatusDTO _FDLStatus;
        public FDLStatusDTO FDLStatus
        {
            get => _FDLStatus;
            set => Set(ref _FDLStatus, value);
        }

        private FDLDTO _FDL1;
        public FDLDTO FDL1
        {
            get => _FDL1;
            set => Set(ref _FDL1, value);
        }

        public string FilePath => ApplicationSettings.Directories.ExpenseAccount + FileName;

        private bool _IsNew;
        public bool IsNew // used for sorting purpose
        {
            get => _IsNew;
            internal set => Set(ref _IsNew, value);
        }

        private string _CurrencyCode;
        public string CurrencyCode
        {
            get => _CurrencyCode;
            set => Set(ref _CurrencyCode, value);
        }

        public EFDLStatus EStatus
        {
            get => (EFDLStatus)Status;
            set
            {
                Status = (long)value;
                RaisePropertyChanged();
            }
        }

        public double? MondayAmount => Expenses?.Sum(x => x.MondayAmount);
        public double? TuesdayAmount => Expenses?.Sum(x => x.TuesdayAmount);
        public double? WednesdayAmount => Expenses?.Sum(x => x.WednesdayAmount);
        public double? ThursdayAmount => Expenses?.Sum(x => x.ThursdayAmount);
        public double? FridayAmount => Expenses?.Sum(x => x.FridayAmount);
        public double? SaturdayAmount => Expenses?.Sum(x => x.SaturdayAmount);
        public double? SundayAmount => Expenses?.Sum(x => x.SundayAmount);
        public double? TotalAmount => Expenses?.Sum(x => x.TotalAmount);

        private bool _InsertExpenseEnabled;
        public bool InsertExpenseEnabled
        {
            get => _InsertExpenseEnabled;
            set => Set(ref _InsertExpenseEnabled, value);
        }
        #endregion;

        #region Display Properties
        public string FDL_New_Display => $"{(NotifyAsNew ? "*" : "")}{FDL}";
        public string TotalAmount_Display => TotalAmount > 0 ? $"{TotalAmount}{CurrencyCode}" : string.Empty;
        public string DeductionAmount_Display => DeductionAmount > 0 ? $"{DeductionAmount}{CurrencyCode}" : string.Empty;
        #endregion

        public ExpenseAccountEVM(ExpenseAccount ea = null)
        {
            Expenses = new ObservableCollectionEx<ExpenseEVM>();
            Expenses.CollectionChanged += (sender, e) => UpdateTotals();
            Expenses.CollectionChanged += (sender, e) => InsertExpenseEnabled = Expenses.Count < ApplicationSettings.ExpenseAccount.MaxExpenseCount;
            Expenses.ItemPropertyChanged += (sender, e) => UpdateTotals();

            if (ea != null)
                Auto.Mapper.Map(ea, this);

            IsChanged = false;
        }

        private void UpdateTotals()
        {
            IsChanged = true;
            RaisePropertyChanged(nameof(MondayAmount));
            RaisePropertyChanged(nameof(TuesdayAmount));
            RaisePropertyChanged(nameof(WednesdayAmount));
            RaisePropertyChanged(nameof(ThursdayAmount));
            RaisePropertyChanged(nameof(FridayAmount));
            RaisePropertyChanged(nameof(SaturdayAmount));
            RaisePropertyChanged(nameof(SundayAmount));
            RaisePropertyChanged(nameof(TotalAmount));
        }

        public override bool Save(DBArchive db)
        {
            ExpenseAccount ea = new ExpenseAccount();

            Auto.Mapper.Map(this, ea);
            db.ExpenseAccounts.AddOrUpdate(ea);
            db.SaveChanges();
            Id = ea.Id;
            IsChanged = false;
            return true;
        }

        public override bool Delete(DBArchive db)
        {
            throw new System.NotImplementedException();
        }

        public override bool Refresh(DBArchive db)
        {
            var exp = db.ExpenseAccounts.SingleOrDefault(x => x.Id == Id);
            if (exp != null)
            {
                Auto.Mapper.Map(exp, this);
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is ExpenseAccountEVM)
            {
                ExpenseAccountEVM exp = obj as ExpenseAccountEVM;
                return Id == exp.Id &&
                       FDL == exp.FDL &&
                       FileName == exp.FileName &&
                       Currency == exp.Currency &&
                       Notes == exp.Notes &&
                       Currency == exp.Currency &&
                       Expenses.SequenceEqual(exp.Expenses);
            }
            return false;
        }

        public override int GetHashCode()
        {
            //Override needed only for dictionaries 
            //https://www.codeproject.com/Tips/1255596/Overriding-Equals-GetHashCode-Laconically-in-CShar

            return base.GetHashCode();
        }
    }
}
