using NWPXH6_HSZF_2024251.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Persistence.MsSql
{
    public interface IPersonDataProvider
    {
        public void CreatePerson(Person p);
        public List<Person> GetAllPerson();
        public Person GetPersonById(string id);
        public Person GetPersonByNeptun(string neptun);
        public void DeletePerson(Person p);
        public void UpdateOrderStatus(Person p, string orderStatus);
        public void UpdateSpecialRequests(Person p, string specialRequest);
        public void AddPaymentToPerson(string personId, Payment pay);
        public List<Person> GetAllPersonsIncludePayments();


    }
}
