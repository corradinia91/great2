using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Great.Models
{
    [Table("ExpenseAccount")]
    public partial class ExpenseAccount
    {
        public ExpenseAccount()
        {
            this.Expenses = new HashSet<Expense>();
        }
    
        public long Id { get; set; }
        public string FDL { get; set; }
        public long? CdC { get; set; }
        public string Currency { get; set; }
        public long Status { get; set; }
        public string LastError { get; set; }
        public string FileName { get; set; }
        public bool NotifyAsNew { get; set; }

        [ForeignKey("Currency")]
        public virtual Currency Currency1 { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
        [ForeignKey("Status")]
        public virtual FDLStatus FDLStatus { get; set; }
        [ForeignKey("FDL")]
        public virtual FDL FDL1 { get; set; }
    }
}