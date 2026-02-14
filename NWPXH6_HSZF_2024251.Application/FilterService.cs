using NWPXH6_HSZF_2024251.Model;
using NWPXH6_HSZF_2024251.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Application
{
    public interface IFilterService 
    {
        public List<Person> FilterAlreadyOrderd();

        public List<Person> FilterAlreadyPaid();

        public List<Person> FilterNotPaidYet();

        public List<Person> FilterFinishedMug();
    }
    public class FilterService : IFilterService
    {
        private IPersonService _personService;

        public FilterService(IPersonDataProvider personDataProvider, IPersonService personService)
        {
            _personService = personService;
        }

        public List<Person> FilterAlreadyOrderd()
        {
            var data = _personService.GetAllPerson();
            return data.Where(p => p.Mug_order_status.Equals("megrendelve")).ToList();
        }

        public List<Person> FilterAlreadyPaid()
        {
            var data = _personService.GetAllPerson();
            return data.Where(p => p.Mug_order_status.Equals("befizetve")).ToList();
        }

        public List<Person> FilterNotPaidYet()
        {
            var data = _personService.GetAllPerson();
            return data.Where(p => p.Mug_order_status == "befizetésre vár").ToList();
        }

        public List<Person> FilterFinishedMug()
        {
            var data = _personService.GetAllPerson();
            return data.Where(p => p.Mug_order_status == "elkészült").ToList();
        }
    }
}
