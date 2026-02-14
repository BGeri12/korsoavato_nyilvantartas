using Moq;
using NUnit.Framework;
using NWPXH6_HSZF_2024251.Application;
using NWPXH6_HSZF_2024251.Model;
using NWPXH6_HSZF_2024251.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NWPXH6_HSZF_2024251.Test
{
    [TestFixture]
    public class FilterServiceTests
    {

        private Mock<IPersonDataProvider> _personDataProviderMock;
        private Mock<IPersonService> _personServiceMock;
        private FilterService _filterService;
        

        [SetUp]
        public void Init()
        {
            _personDataProviderMock = new Mock<IPersonDataProvider>();
            _personServiceMock = new Mock<IPersonService>();

            _filterService = new FilterService(_personDataProviderMock.Object,_personServiceMock.Object);
        }

        [Test]
        public void FilterAlreadyOrderdTest()
        {
            //ARRAGE
            var persons = new List<Person> 
            {
                new Person("Halász Sárndor","HAL123",false,null,"megrendelve"),
                new Person("Horváth János","HOR123",true,"üveg","megrendelve"),
                //new Person("Kis Jolán","KIS123",true,null,"befizetve"),
                //new Person("Kereskes Erika","KER123",true, "gömbölyű","elkészült"),
            };

            _personServiceMock.Setup(m=> m.GetAllPerson()).Returns(persons);

            //ACT
            var result = _filterService.FilterAlreadyOrderd();

            //ASSERT
            Assert.That(result != null);
            Assert.That(result.Count.Equals(2));
            Assert.That(result[0].Name == "Halász Sárndor");
        }

        [Test]
        public void FilterAlreadyPaidTest()
        {
            // ARRAGE
            var persons = new List<Person>
            {
                new Person("Halász Sárndor","HAL123",false,null,"befizetve"),
                new Person("Horváth János","HOR123",true,"üveg","befizetve"),
                new Person("Kis Jolán","KIS123",true,null,"befizetve"),
                new Person("Kereskes Erika","KER123",true, "gömbölyű","befizetve"),
            };

            _personServiceMock.Setup(m => m.GetAllPerson()).Returns(persons);

            //ACT
            var result = _filterService.FilterAlreadyPaid();

            //ASSERT
            Assert.That(result != null);
            Assert.That(result.Count.Equals(4));
            Assert.That(result[0].Name == "Halász Sárndor");
        }

        [Test]
        public void FilterNotPaidYetTest()
        {
            // ARRAGE
            var persons = new List<Person>
            {
                new Person("Halász Sárndor","HAL123",false,null,"befizetésre vár"),
                //new Person("Horváth János","HOR123",true,"üveg","megrendelve"),
                //new Person("Kis Jolán","KIS123",true,null,"befizetve"),
                //new Person("Kereskes Erika","KER123",true, "gömbölyű","elkészült"),
            };

            _personServiceMock.Setup(m => m.GetAllPerson()).Returns(persons);

            //ACT
            var result = _filterService.FilterNotPaidYet();

            //ASSERT
            Assert.That(result != null);
            Assert.That(result.Count.Equals(1));
            Assert.That(result[0].Name == "Halász Sárndor");
        }

        [Test]
        public void FilterFinishedMugTest()
        {
            // ARRAGE
            var persons = new List<Person>
            {
                new Person("Halász Sárndor","HAL123",false,null,"elkészült"),
                new Person("Horváth János","HOR123",true,"üveg","elkészült"),
                new Person("Kis Jolán","KIS123",true,null,"elkészült"),
                //new Person("Kereskes Erika","KER123",true, "gömbölyű","elkészült"),
            };

            _personServiceMock.Setup(m => m.GetAllPerson()).Returns(persons);

            //ACT
            var result = _filterService.FilterFinishedMug();

            //ASSERT
            Assert.That(result != null);
            Assert.That(result.Count.Equals(3));
            Assert.That(result[0].Name == "Halász Sárndor");
        }
    }
}
