//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Great.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class CarRentalHistory
    {
        public long Id { get; set; }
        public long Car { get; set; }
        public long StartKm { get; set; }
        public Nullable<long> EndKm { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public long StartFuelLevel { get; set; }
        public Nullable<long> EndFuelLevel { get; set; }
        public string Notes { get; set; }
    
        public virtual Car Car1 { get; set; }
    }
}
