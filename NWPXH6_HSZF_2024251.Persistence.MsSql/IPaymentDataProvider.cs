using NWPXH6_HSZF_2024251.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Persistence.MsSql
{
    public interface IPaymentDataProvider
    {
        public void CreatePayment(Payment payment);
        public List<Payment> ReadAllPayment();
        public Payment GetPaymentById(string id);
        public void DeletePayment(Payment paymnet);
       



        public List<Payment> ReadPaymentByNeptun(string neptunCode);
        public void UpdatePayment(Payment payment);
        


    }
}
