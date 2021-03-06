﻿using Great2.Models.Database;

namespace Great2.Models.DTO
{
    public class CarDTO
    {
        public long Id { get; set; }
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string CarRentalCompany { get; set; }

        public CarDTO() { }

        public CarDTO(Car car)
        {
            Auto.Mapper.Map(car, this);
        }
    }
}
