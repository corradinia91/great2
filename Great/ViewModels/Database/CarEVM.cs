﻿using Great.Models.Database;
using Great.Models.DTO;
using Great.Utils;
using System.ComponentModel;
using System.Data.Entity.Migrations;

namespace Great.ViewModels.Database
{
    public class CarEVM : EntityViewModelBase, IDataErrorInfo
    {
        #region Properties

        public long Id { get; set; }

        private string _licencePlate;
        public string LicensePlate
        {
            get => _licencePlate;
            set
            {
                Set(ref _licencePlate, value);
                RaisePropertyChanged(nameof(LicensePlate));
            }
        }

        private string _brand;
        public string Brand
        {
            get => _brand;
            set
            {
                Set(ref _brand, value);
                RaisePropertyChanged(nameof(Brand));
            }
        }

        private string _model;
        public string Model
        {
            get => _model;
            set
            {
                Set(ref _model, value);
                RaisePropertyChanged(nameof(Model));
            }
        }

        private long _carRentalCompany;
        public long CarRentalCompany

        {
            get => _carRentalCompany;
            set
            {
                Set(ref _carRentalCompany, value);
                RaisePropertyChanged(nameof(CarRentalCompany));
                RaisePropertyChanged(nameof(CarRentalCompany1));
            }

        }

        private CarRentalCompanyDTO _carRentalCompany1;
        public CarRentalCompanyDTO CarRentalCompany1
        {
            get => _carRentalCompany1;
            set
            {
                Set(ref _carRentalCompany1, value);
                RaisePropertyChanged(nameof(CarRentalCompany1));
            }
        }

        private ObservableCollectionEx<CarRentalHistoryEVM> _carRentalHistories;
        public ObservableCollectionEx<CarRentalHistoryEVM> CarRentalHistories
        {
            get => _carRentalHistories;
            set => Set(ref _carRentalHistories, value);
        }

        #endregion


        #region Errors Validation

        public string Error =>
            this["LicensePlate"] != null
            || this["Brand"] != null
            || this["Model"] != null
            || this["CarRentalCompany1"] != null ? "Error" : null;



        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "LicensePlate":
                        if (string.IsNullOrEmpty(LicensePlate) || string.IsNullOrWhiteSpace(LicensePlate))
                            return "License Plate not valid";
                        break;

                    case "Brand":
                        if (string.IsNullOrEmpty(Brand) || string.IsNullOrWhiteSpace(Brand))
                            return "Brand not valid";
                        break;

                    case "Model":
                        if (string.IsNullOrEmpty(Model) || string.IsNullOrWhiteSpace(Model))
                            return "Model not valid";
                        break;
                    case "CarRentalCompany1":
                        if (CarRentalCompany1 == null || CarRentalCompany1?.Id == 0)
                            return "Rental company not valid";
                        break;

                    default:

                        break;
                }

                return null;
            }
        }
        #endregion
        public CarEVM(Car car = null)
        {
            if (car != null)
                Global.Mapper.Map(car, this);
        }

        public override bool Save(DBArchive db)
        {
            Car car = new Car();

            Global.Mapper.Map(this, car);
            db.Cars.AddOrUpdate(car);
            db.SaveChanges();
            Id = car.Id;

            return true;
        }

        public override bool Delete(DBArchive db)
        {
            throw new System.NotImplementedException();
        }

        public override bool Refresh(DBArchive db)
        {
            throw new System.NotImplementedException();
        }
    }
}
