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
    
    public partial class Timesheet
    {
        public long Id { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<System.DateTime> TravelStartTimeAM { get; set; }
        public Nullable<System.DateTime> TravelEndTimeAM { get; set; }
        public Nullable<System.DateTime> TravelStartTimePM { get; set; }
        public Nullable<System.DateTime> TravelEndTimePM { get; set; }
        public Nullable<System.DateTime> WorkStartTimeAM { get; set; }
        public Nullable<System.DateTime> WorkEndTimeAM { get; set; }
        public Nullable<System.DateTime> WorkStartTimePM { get; set; }
        public Nullable<System.DateTime> WorkEndTimePM { get; set; }
        public Nullable<long> FDL { get; set; }
    
        public virtual FDL FDL1 { get; set; }
    }
}
