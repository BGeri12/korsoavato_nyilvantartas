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

namespace NWPXH6_HSZF_2024251.Test
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private Mock<IPaymentDataProvider> _paymentDataProviderMock;
        private Mock<IPersonService> _personServiceMock;
        private IPaymentService _paymentService;

        [SetUp]
        public void Init()
        {
            _paymentDataProviderMock = new Mock<IPaymentDataProvider>();
            _personServiceMock = new Mock<IPersonService>();

            _paymentService = new PaymentService(_paymentDataProviderMock.Object, _personServiceMock.Object);
        }

        [Test]
        public void CreatePaymentTest()
        {
            //ARRAGE
            var payment = new Payment
            {
                Id = "test_id",
                Is_paid = false,
                Amount = 1000,
                Date = null
            };

            //ACT
            _paymentService.CreatePayment(payment);

            //ASSERT
            _paymentDataProviderMock.Verify(m => m.CreatePayment(payment), Times.Once);
        }
    }
}
