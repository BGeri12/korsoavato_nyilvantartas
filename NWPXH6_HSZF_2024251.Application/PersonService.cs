using Microsoft.CodeAnalysis.CSharp.Syntax;
using NWPXH6_HSZF_2024251.Model;
using NWPXH6_HSZF_2024251.Persistence.MsSql;

namespace NWPXH6_HSZF_2024251.Application
{
    public interface IPersonService
    {
        public void CreatePerson(Person p);
        public List<Person> GetAllPerson();
        public Person GetPersonByID(string personId);
        public Person GetPersonByNeptun(string neptun);
        public void DeletePerson(string personId);
        public void AddPaymentToPerson(string personId, Payment pay);
        public void UpdateSpecialRequests(string neptunCode, string specialRequest);
        public void UpdateOrderStatus(string neptunCode, string orderStatus);
        public List<Person> GetAllPersonsIncludePayments();
        public Payment OrderPaymentsDescTopNull(Person person);
        public Payment OrderPaymentsDesc(Person person);

    }
    public class PersonService : IPersonService
    {
        //Üzleti logoka megvalósítása
        // data provideren keresztűl kap adatot és azzaz dolgozik
        // majd továbbitja a Presentation layer felé

        private IPersonDataProvider _personData;

        public PersonService(IPersonDataProvider dp) 
        {
            this._personData = dp;
        }
        public void CreatePerson(Person p)
        {
            _personData.CreatePerson(p);
        }
        public List<Person> GetAllPerson()
        {
            return _personData.GetAllPerson();
        }
        public Person GetPersonByID(string personId)
        {
            Person personToFind = _personData.GetPersonById(personId);
            if (personToFind != null)
            {
                return personToFind;
            }
            else
            {
                throw new KeyNotFoundException($"Person with this ID: {personId} not found.");
            }
        }
        public Person GetPersonByNeptun(string neptun)
        {
            Person personToFind = _personData.GetPersonByNeptun(neptun);
            if (personToFind != null)
            {
                return personToFind;
            }
            else
            {
                throw new KeyNotFoundException($"Person with this neptun code: {neptun} not found.");
            }
        }
        public void DeletePerson(string personId)
        {
            var personToDelete = GetPersonByID(personId);
            if (personToDelete != null)
            {
                _personData.DeletePerson(personToDelete);
            }
        }
        public void UpdateOrderStatus(string neptunCode, string orderStatus)
        {
            Person personToUpdate = GetPersonByNeptun(neptunCode);
            _personData.UpdateOrderStatus(personToUpdate, orderStatus);
        }
        public void UpdateSpecialRequests(string neptunCode, string specialRequest)
        {
            var personToUpdate = GetPersonByNeptun(neptunCode);
            _personData.UpdateSpecialRequests(personToUpdate, specialRequest);
        }
        public void AddPaymentToPerson(string personId, Payment pay)
        {
            _personData.AddPaymentToPerson(personId, pay);
        }
        public List<Person> GetAllPersonsIncludePayments()
        {
            return _personData.GetAllPersonsIncludePayments();
        }
        public Payment OrderPaymentsDesc(Person person)
        {
            return person.Payment
                          .OrderByDescending(p => p.Date)
                          .FirstOrDefault();

            /*
            2024-09-01
            2023-08-15
            2022-05-10
            null
            */
        }

        public Payment OrderPaymentsDescTopNull(Person person)
        {
            return person.Payment
                          .OrderBy(pay => pay.Date == null ? 0 : 1)
                          .ThenByDescending(pay => pay.Date)
                          .FirstOrDefault();

            /*
            null
            2023-08-15
            2022-05-10
            */
        }
    }
}
