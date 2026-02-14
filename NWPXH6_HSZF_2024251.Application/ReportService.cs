using NWPXH6_HSZF_2024251.Model;
using NWPXH6_HSZF_2024251.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Application
{
    public interface IReportService
    {
        void CreatingDirectory();
        void AnnualStatisticsReport();
        //void GenerateParticipationStatisticsReport();
        void GenerateOrderableMugsReport();
    }
    public class ReportService : IReportService
    {
        private IPersonService _personService;

        public ReportService( IPersonService personService)
        {
            _personService = personService;
        }
        public void GenerateOrderableMugsReport()
        {
            var data = _personService.GetAllPersonsIncludePayments();
            string baseDirectory = Path.Combine(Environment.CurrentDirectory, "aaaReports", DateTime.Now.Year.ToString());
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }

            string reportFileName = $"OrderableMugs_{DateTime.Now:yyyyMMdd}.txt";
            string reportFilePath = Path.Combine(baseDirectory, reportFileName);

            using (StreamWriter writer = new StreamWriter(reportFilePath))
            {
                writer.WriteLine("Orderable Mugs Report");
                writer.WriteLine("==============================================================================================");
                writer.WriteLine($"{"Name",-26} {"Neptun Code",-24} {"Status",-25} {"Order Status",-25}");
                writer.WriteLine("----------------------------------------------------------------------------------------------");

                foreach (var person in data)
                {
                    var latestPayment = _personService.OrderPaymentsDescTopNull(person);
                    
                    if ((latestPayment != null && latestPayment.Is_paid) || !person.Is_student)
                    {
                        if (person.Mug_order_status != "elkészült")
                        {
                            if (person.Mug_order_status != "megrendelve")
                            {
                                _personService.UpdateOrderStatus(person.Neptun_code, "megrendelve");
                            }

                            string name = person.Name.PadRight(28);
                            string neptunCode = person.Neptun_code.PadRight(22);
                            string status = (person.Is_student ? "Student" : "Teacher").PadRight(26);
                            string orderStatus = person.Mug_order_status.PadRight(26);

                            writer.WriteLine($"{name} {neptunCode} {status} {orderStatus}");
                        }
                    }
                }

                writer.WriteLine();
                writer.WriteLine("==============================================================================================");
            }
        }

        public void AnnualStatisticsReport()
        {
            var data = _personService.GetAllPersonsIncludePayments();
            string baseDirectory = Path.Combine(Environment.CurrentDirectory, "aaaReports");

            var groupedByYears = data.SelectMany(person => person.Payment
                                               .Where(payment => payment.Date.HasValue)
                                               .Select(payment => new { Year = payment.Date.Value.Year, Person = person, PaymentDate = payment.Date }))
                            .GroupBy(x => x.Year);

            foreach (var yearGroup in groupedByYears)
            {
                int year = yearGroup.Key;
                string yearDirectory = Path.Combine(baseDirectory, year.ToString());

                if (!Directory.Exists(yearDirectory))
                {
                    Directory.CreateDirectory(yearDirectory);
                }

                string reportFilePath = Path.Combine(yearDirectory, $"ParticipationStatistics_{year}.txt");

                using (StreamWriter writer = new StreamWriter(reportFilePath))
                {
                    writer.WriteLine($"Participation Statistics for {year}");
                    writer.WriteLine("==============================================================================================");
                    writer.WriteLine($"{"Name",-26} {"Neptun Code",-24} {"Status",-25} {"Payment Date",-25}");
                    writer.WriteLine("----------------------------------------------------------------------------------------------");

                    foreach (var record in yearGroup)
                    {
                        string name = record.Person.Name.PadRight(28);
                        string neptunCode = record.Person.Neptun_code.PadRight(22);
                        string status = (record.Person.Is_student ? "Student" : "Teacher").PadRight(26);
                        string paymentDate = record.PaymentDate?.ToString("yyyy-MM-dd") ?? "N/A";


                        writer.WriteLine($"{name} {neptunCode} {status} {paymentDate}");
                    }

                    writer.WriteLine();
                    writer.WriteLine("==============================================================================================");
                    writer.WriteLine();
                    writer.WriteLine();
                    writer.WriteLine();

                    int studentCount = yearGroup.Count(x => x.Person.Is_student);
                    int teacherCount = yearGroup.Count(x => !x.Person.Is_student);

                    writer.WriteLine($"Student, Teacher Statistics for {year}");
                    writer.WriteLine("===============================");
                    writer.WriteLine($"Student(s): {studentCount}");
                    writer.WriteLine($"Teacher(s): {teacherCount}");

                }
            }
        }

        public void CreatingDirectory()
        {
            var data = _personService.GetAllPersonsIncludePayments();
            string baseDirectory = Path.Combine(Environment.CurrentDirectory, "aaaReports");

            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }

            foreach (var person in data)
            {
                var latestPaymentDate = person.Payment.OrderByDescending(p => p.Date).FirstOrDefault()?.Date;
                if (latestPaymentDate.HasValue)
                {
                    int year = latestPaymentDate.Value.Year;
                    string yearDirectory = Path.Combine(baseDirectory, year.ToString());

                    if (!Directory.Exists(yearDirectory))
                    {
                        Directory.CreateDirectory(yearDirectory);
                    }
                }
            }
        }

        /*
        public void GenerateParticipationStatisticsReport()
        {
            var data = _personData.GetAllPerson();
            string baseDirectory = Path.Combine(Environment.CurrentDirectory, "aaaReports");

            var groupedByYears = data.SelectMany(person => person.Payment
                                               .Where(payment => payment.Date.HasValue)
                                               .Select(payment => new { Year = payment.Date.Value.Year, Person = person }))
                                    .GroupBy(x => x.Year);

            foreach (var yearGroup in groupedByYears)
            {
                int year = yearGroup.Key;
                string yearDirectory = Path.Combine(baseDirectory, year.ToString());

                if (!Directory.Exists(yearDirectory))
                {
                    Directory.CreateDirectory(yearDirectory);
                }

                string reportFilePath = Path.Combine(yearDirectory, $"StudentTeacherStatistics_{year}.txt");

                int studentCount = yearGroup.Count(x => x.Person.Is_student);
                int teacherCount = yearGroup.Count(x => !x.Person.Is_student);

                using (StreamWriter writer = new StreamWriter(reportFilePath))
                {
                    writer.WriteLine($"Student, Teacher Statistics for {year}");
                    writer.WriteLine("===============================");
                    writer.WriteLine($"Student(s): {studentCount}");
                    writer.WriteLine($"Teacher(s): {teacherCount}");
                }
            }
        }
        */

    }

}
