using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NWPXH6_HSZF_2024251.Model
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public bool Is_paid { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime? Date { get; set; }


        [ForeignKey("Person")]
        public string? Person_Id { get; set; }
        public virtual Person Person { get; set; }


        public Payment(bool is_paid, decimal amount,  DateTime? date, string? person_Id )
        {
            Id = Guid.NewGuid().ToString();
            Is_paid = is_paid;
            Amount = amount;
            Date = date;
            Person_Id = person_Id;
        }

        public Payment() { }
        
    }
}
