using Newtonsoft.Json;
using NWPXH6_HSZF_2024251.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Persistence.MsSql
{
    public interface ISeedDataProvider 
    {
        public void LoadingPersonToTheDB(Person person);
        public void LoadingPaymentToTheDB(Payment payment);
        public void SaveAll();
    }
    public class SeedDataProvider : ISeedDataProvider
    {
        private AppDbContext context;
        public SeedDataProvider(AppDbContext ctx) //injektálás
        {
            this.context = ctx;
        }

        public void LoadingPaymentToTheDB(Payment payment)
        {
            context.PaymentsDb.Add(payment);
        }

        public void LoadingPersonToTheDB(Person person)
        {
            context.PersonsDb.Add(person);
        }

        public void SaveAll()
        {
            context.SaveChanges();
        }
    }
}
