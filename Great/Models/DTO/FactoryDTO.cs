﻿using Great2.Models.Database;

namespace Great2.Models.DTO
{
    public class FactoryDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool OverrideAddressOnFDL { get; set; }

        public FactoryDTO() { }

        public FactoryDTO(Factory factory)
        {
            Auto.Mapper.Map(factory, this);
        }
    }
}
