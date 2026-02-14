using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Model
{
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [StringLength(6)]
        [Required]
        public string Neptun_code { get; set; }
        
        [Required]
        public bool Is_student { get; set; }

        [StringLength(100)]
        public string? Special_requests { get; set; }

        [StringLength(30)]
        [Required]
        public string Mug_order_status { get; set; }

        // Egy a több vagy több a több kapcsolat esetén:

        public virtual ICollection<Payment> Payment { get; set; }
        /* josn file:
          {
                "persons": [
                            {
                              "name": "Kovács Péter",
                              "neptun_code": "ABC123",
                              "is_student": true,
                              "special_requests": null,
                              "mug_order_status": "Rendelés leadva",
                              "payment": [
                                {
                                  "is_paid": false,
                                  "amount": 2990,
                                  "date": null
                                }
                              ]
                            }
                           ]
           }
         */


        // Egy az egy-hez kapcsolat esetén:
        //public virtual Payment Payment { get; set; }

        // amiatt a json file át van szerkesztve
        /* josn file:
         {
                 "person": [
                            {
                              "name": "Kovács Péter",
                              "neptun_code": "ABC123",
                              "is_student": true,
                              "special_requests": null,
                              "mug_order_status": "Rendelés leadva",
                              "payment": {
                                "is_paid": false,
                                "amount": 2990,
                                "date": null
                              }
                            }
                          ]
           }
         */


        public Person(string name, string neptun_code, bool is_student, string special_requests, string mug_order_status)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Neptun_code = neptun_code;
            Is_student = is_student;
            Special_requests = special_requests;
            Mug_order_status = mug_order_status;
            Payment = new HashSet<Payment>();
            //Payment = new Payment();
        }

        public Person() 
        {
            Payment = new HashSet<Payment>();
            //Payment = new Payment();
        }

        
    }
}
