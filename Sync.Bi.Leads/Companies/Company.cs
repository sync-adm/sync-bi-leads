using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Bi.Leads.Companies
{

    [Table("Company")]
    public class Company
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }

        public string CGCNumber { get; set; }   //cgcNumber
    }
}
