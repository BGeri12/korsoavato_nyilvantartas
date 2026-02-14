using Moq;
using NUnit.Framework;
using NWPXH6_HSZF_2024251.Application;
using NWPXH6_HSZF_2024251.Model;
using NWPXH6_HSZF_2024251.Persistence.MsSql;

namespace NWPXH6_HSZF_2024251.Test
{
    [TestFixture]
    public class PersonServiceTests
    {
        private Mock<IPersonDataProvider> _personDataProviderMock;
        private PersonService _personService;

        [SetUp]
        public void Init()
        {
            _personDataProviderMock = new Mock<IPersonDataProvider>();
            /*
            _personDataProviderMock.Setup(m => m.GetAllPerson()).Returns(new List<Person>()
            {
                new Person("Halász Sárndor","HAL123",false,null,"megrendelve"),
                new Person("Horváth János","HOR123",true,"üveg","befizetésre vár"),
                new Person("Kis Jolán","KIS123",true,null,"befizetve"),
                new Person("Kereskes Erika","KER123",true, "gömbölyű","elkészült"),
            }.AsQueryable());
            */
            _personService = new PersonService(_personDataProviderMock.Object);
        }

        [Test]
        public void GetPersonByIDTest()
        {
            //ARRAGE
            var expectedPerson = new Person("Novák János", "NOV123", true, "üveg", "befizetésre vár");
            expectedPerson.Id = "id1";
            _personDataProviderMock.Setup(m => m.GetPersonById("id1")).Returns(expectedPerson);

            //ACT
            var result = _personService.GetPersonByID("id1");

            //ASSERT
            Assert.That(result != null);
            Assert.That(expectedPerson.Id.Equals(result.Id));
        }

        [Test]

        public void GetPersonByNeptunTest() 
        {
            //ARRAGE
            var exeptedPerson = new Person("Kereskes Erika", "KER123", true, "gömbölyű", "elkészült");
            exeptedPerson.Id = "id2";
            _personDataProviderMock.Setup(m => m.GetPersonByNeptun("KER123")).Returns(exeptedPerson);

            //ACT
            var result = _personService.GetPersonByNeptun("KER123");

            //ASSERT
            Assert.That(result != null);
            Assert.That(exeptedPerson.Neptun_code.Equals(result.Neptun_code));
        }

        [Test]
        public void CreatePersonTest() 
        {
            //ARRAGE
            var person = new Person("unit teszt1", "UNIT01", true, null, "elkészült");

            //ACT
            _personService.CreatePerson(person);

            //ASSERT
            _personDataProviderMock.Verify(m => m.CreatePerson(person), Times.Once);
        }

        [Test]
        public void UpdateOrderStatusTest()
        {
            //ARRAGE
            var person = new Person("Halász Sárndor", "HAL123", false, null, "megrendelve");
            _personDataProviderMock.Setup(m => m.GetPersonByNeptun("HAL123")).Returns(person);

            //ACT
            _personService.UpdateOrderStatus("HAL123", "elkészült");

            //ASSERT
            _personDataProviderMock.Verify(m => m.UpdateOrderStatus(person, "elkészült"), Times.Once);
        }

        [Test]
        public void UpdateSpecialRequestsTest()
        {
            //ARRAGE
            var person = new Person("Halász Sárndor", "HAL123", false, null, "megrendelve");
            _personDataProviderMock.Setup(m => m.GetPersonByNeptun("HAL123")).Returns(person);

            //ACT
            _personService.UpdateSpecialRequests("HAL123", "színes");

            //ASSERT
            _personDataProviderMock.Verify(m => m.UpdateSpecialRequests(person, "színes"), Times.Once);
        }
    }
}
