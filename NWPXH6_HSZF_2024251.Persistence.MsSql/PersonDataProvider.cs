using Microsoft.EntityFrameworkCore;
using NWPXH6_HSZF_2024251.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Persistence.MsSql
{
    public class PersonDataProvider : IPersonDataProvider
    {

        private AppDbContext context;
        public PersonDataProvider(AppDbContext ctx) //injektálás
        {
            this.context = ctx;
        }

        //CRUD metodusok
        
        public void CreatePerson(Person p)
        {
            context.PersonsDb.Add(p);
            context.SaveChanges();
        }
        public List<Person> GetAllPerson()
        {
            return context.PersonsDb.ToList();
        }
        public Person GetPersonById(string id)
        {
            return context.PersonsDb.FirstOrDefault(p => p.Id.Equals(id));
        }
        public Person GetPersonByNeptun(string neptun)
        {
            return context.PersonsDb.FirstOrDefault(p => p.Neptun_code.Equals(neptun));
        }
        public void DeletePerson(Person p)
        {
            context.PersonsDb.Remove(p);
            context.SaveChanges();
        }
        public void UpdateOrderStatus(Person p, string orderStatus)
        {
            p.Mug_order_status = orderStatus;
            context.SaveChanges();
        }

        public void UpdateSpecialRequests(Person p, string specialRequest)
        {
            p.Special_requests = specialRequest;
            context.SaveChanges();
        }

        public void AddPaymentToPerson(string personId, Payment pay)
        {
            pay.Person_Id = personId;
            context.PaymentsDb.Add(pay);
            context.SaveChanges();
        }

        public List<Person> GetAllPersonsIncludePayments()
        {
            return context.PersonsDb.Include(p => p.Payment).ToList();
        }
    }
}
