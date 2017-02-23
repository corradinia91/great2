//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Great.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FDL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FDL()
        {
            this.ExpenseAccounts = new HashSet<ExpenseAccount>();
            this.Timesheets = new HashSet<Timesheet>();
        }
    
        public long Id { get; set; }
        public long Year { get; set; }
        public long WeekNr { get; set; }
        public string FileName { get; set; }
        public long Factory { get; set; }
        public string Order { get; set; }
        public Nullable<bool> OutwardCar { get; set; }
        public Nullable<bool> ReturnCar { get; set; }
        public Nullable<bool> OutwardTaxi { get; set; }
        public Nullable<bool> ReturnTaxi { get; set; }
        public Nullable<bool> OutwardFlight { get; set; }
        public Nullable<bool> ReturnFlight { get; set; }
        public string PerformanceDescription { get; set; }
        public long Result { get; set; }
        public string ResultNotes { get; set; }
        public string Notes { get; set; }
        public long Status { get; set; }
        public string LastError { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExpenseAccount> ExpenseAccounts { get; set; }
        public virtual Factory Factory1 { get; set; }
        public virtual FDLStatu FDLStatu { get; set; }
        public virtual FDLResult FDLResult { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Timesheet> Timesheets { get; set; }
    }
}
