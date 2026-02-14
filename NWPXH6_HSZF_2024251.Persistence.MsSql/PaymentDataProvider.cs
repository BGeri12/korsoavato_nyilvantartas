using Microsoft.EntityFrameworkCore;
using NWPXH6_HSZF_2024251.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Persistence.MsSql
{
    
    public class PaymentDataProvider : IPaymentDataProvider
    {

        private AppDbContext context;
        public PaymentDataProvider(AppDbContext ctx) //injektálás
        {
            this.context = ctx;
        }

        //CRUD metodusok
        
        public void CreatePayment(Payment pay)
        {
            context.PaymentsDb.Add(pay);
            context.SaveChanges();
        }
        public List<Payment> ReadAllPayment()
        {
            return context.PaymentsDb.ToList();
        }
        public Payment GetPaymentById(string id)
        {
            return context.PaymentsDb.FirstOrDefault(p => p.Id.Equals(id));
        }
        public void DeletePayment(Payment paymnet)
        {
            context.PaymentsDb.Remove(paymnet);
            context.SaveChanges();
        }
       
        public void UpdatePayment(Payment payment)
        {
            payment.Is_paid = true;
            payment.Date = DateTime.Now;
            context.SaveChanges();
        }


        public List<Payment> ReadPaymentByNeptun(string neptunCode)
        {
            List<Payment> result = new List<Payment>();
            var personToFind = context.PersonsDb
                                       .Include(p => p.Payment)
                                       .FirstOrDefault(p => p.Neptun_code.Equals(neptunCode));

            if (personToFind != null)
            {
                result.AddRange(personToFind.Payment);
            }

            return result;
        }


    }
}
