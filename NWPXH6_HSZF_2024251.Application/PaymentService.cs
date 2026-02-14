using NWPXH6_HSZF_2024251.Model;
using NWPXH6_HSZF_2024251.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Application
{
    public interface IEventPaymentService
    {
        public event Action<string, string> NowPaid;

        public event Action<string, string> IsNotPaidYet;

        public event Action<string, string> AlreadyPaid;

        public event Action<string> ThisPersonNotExist;

        public event Action<string> ThisPaymentNotExist;

        public event Action<string> DontHaveAny;

    }
    public interface IPaymentService : IEventPaymentService
    {
        public void CreatePayment(Payment pay);
        public void DeletePayment(string paymentId);
        public Payment GetPaymentById(string paymentId);
        public List<Payment> ReadAllPayment();
        public void UpdatePayment(string neptunCode);

    }
    public class PaymentService : IPaymentService, IEventPaymentService
    {
        public Action<string, string> EventHandlerOne;
        public Action<string> EventHandlerTwo;

        public event Action<string, string> NowPaid;
        public event Action<string, string> IsNotPaidYet;
        public event Action<string, string> AlreadyPaid;
        public event Action<string> ThisPersonNotExist;
        public event Action<string> ThisPaymentNotExist;
        public event Action<string> DontHaveAny;

        private IPaymentDataProvider _paymentData;
        private IPersonService _personService;
        public PaymentService(IPaymentDataProvider dp, IPersonService ps) 
        {
            this._paymentData = dp;
            this._personService = ps;
        }

        public void CreatePayment(Payment pay)
        {
            _paymentData.CreatePayment(pay);
        }

        public List<Payment> ReadAllPayment()
        {
            return _paymentData.ReadAllPayment();
        }

        public Payment GetPaymentById(string paymentId)
        {
            Payment paymentToFind = _paymentData.GetPaymentById(paymentId);
            if (paymentToFind != null)
            {
                return paymentToFind;
            }
            else
            {
                throw new KeyNotFoundException($"Payment with this ID: {paymentId} not found.");
            }
        }

        public void DeletePayment(string paymentId)
        {
            Payment paymentToDelet = GetPaymentById(paymentId);
            _paymentData.DeletePayment(paymentToDelet);

        }

        public void UpdatePayment(string neptunCode)
        {
            var personToUpdate = _personService.GetAllPersonsIncludePayments()
                                  .FirstOrDefault(p => p.Neptun_code.Equals(neptunCode));

            if (personToUpdate == null)
            {
                ThisPersonNotExist?.Invoke(neptunCode);
                return;
            }

            if (personToUpdate.Payment.Count == 0)
            {
                ThisPaymentNotExist?.Invoke(neptunCode);
                return;
            }
            else
            {
                var paymentToUpdate = personToUpdate.Payment.FirstOrDefault(payment => !payment.Is_paid);
                if (paymentToUpdate != null)
                {
                    _paymentData.UpdatePayment(paymentToUpdate);
                    _personService.UpdateOrderStatus(neptunCode,"befizetve");
                    NowPaid?.Invoke(personToUpdate.Name, personToUpdate.Neptun_code);
                }
                else
                {
                    AlreadyPaid?.Invoke(personToUpdate.Name, personToUpdate.Neptun_code);
                }
            }
        }

        
    }
}
