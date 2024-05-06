using System.ComponentModel.DataAnnotations.Schema;
namespace Sync.Bi.Leads.Leads
{
    [Table("Lead")]
    public class Lead
    {
        public int Id { get; set; }
        public LeadConst.IntegrationType IntegrationType { get; set; }
        public string Team { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
    }

}
