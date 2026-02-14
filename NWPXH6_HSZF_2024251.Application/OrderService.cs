using NWPXH6_HSZF_2024251.Model;
using NWPXH6_HSZF_2024251.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Application
{
    public interface IOrderService 
    {
        public List<Person> OrderByDate();
        public List<Person> OrderByName();

    }
    public class OrderService : IOrderService
    {
        private IPersonService _personService;

        public OrderService(IPersonService personService)
        {
            _personService = personService;
        }

        public List<Person> OrderByDate()
        {
            var data = _personService.GetAllPersonsIncludePayments();

            var result = data.SelectMany(person => person.Payment
                                           .Where(payment => payment.Date.HasValue)
                                           .Select(payment => new Person
                                           {
                                               Id = person.Id,
                                               Name = person.Name,
                                               Neptun_code = person.Neptun_code,
                                               Is_student = person.Is_student,
                                               Special_requests = person.Special_requests,
                                               Mug_order_status = person.Mug_order_status,
                                               Payment = new List<Payment> { payment } 
                                           }))
                             .OrderBy(p => p.Payment.First().Date) 
                             .ToList();

            return result;
        }

        public List<Person> OrderByName()
        {
            var data = _personService.GetAllPersonsIncludePayments();
            return data.OrderBy(p => p.Name).ToList();
        }
    }
}
