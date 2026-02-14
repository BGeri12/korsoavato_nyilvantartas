using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Abstractions;
using Newtonsoft.Json;
using NWPXH6_HSZF_2024251.Application;
using NWPXH6_HSZF_2024251.Model;
using NWPXH6_HSZF_2024251.Persistence.MsSql;
using System;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NWPXH6_HSZF_2024251
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // példányosítás IOC konténerrel
            // oda resigsztáljuk fel az interface osztály párosokat
            // azaz milyen interface-val milyen osztályt szeretnénk pédányitani

            // string jsonFilePath = "json.json";

            // dependency injection-höz service feliratkoztatás
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) => 
                {
                    // AddScope - adatbázishoz
                    // AddSingleton egy db interface-hez példányosít egy db osztály
                    // program futása során ezek az egy db-ok fognak létezni

                    //sorrend fontos !!!
                    services.AddScoped<AppDbContext>();

                    services.AddSingleton<ISeedDataProvider, SeedDataProvider>();
                    services.AddSingleton<IPersonDataProvider, PersonDataProvider>();
                    services.AddSingleton<IPaymentDataProvider, PaymentDataProvider>();
                    

                    services.AddSingleton<ISeedDataService, SeedDataService>();
                    services.AddSingleton<IPersonService, PersonService>();
                    services.AddSingleton<IPaymentService, PaymentService>();
                    services.AddSingleton<IReportService, ReportService>();
                    services.AddSingleton<IOrderService, OrderService>();
                    services.AddSingleton<IFilterService, FilterService>();

                    services.AddSingleton<IEventPaymentService>(provider => provider.GetRequiredService<IPaymentService>() as PaymentService);
                })
                .Build();
            host.Start();
            
            using IServiceScope serviceScope = host.Services.CreateScope();

            IPersonDataProvider personData = host.Services.GetRequiredService<IPersonDataProvider>();
            IPaymentDataProvider paymentData = host.Services.GetRequiredService<IPaymentDataProvider>();
            IEventPaymentService paymentEnvents = host.Services.GetRequiredService<IEventPaymentService>();

            paymentEnvents.NowPaid += IsPaidWriteOut;
            paymentEnvents.ThisPersonNotExist += ThisPersonNotExistWriteOut;
            paymentEnvents.ThisPaymentNotExist += ThisPaymentNotExistWriteOut;
            paymentEnvents.AlreadyPaid += AlreadyPaidWriteOut;


            ISeedDataService seedData = host.Services.GetRequiredService<ISeedDataService>();
            IPersonService personService = host.Services.GetRequiredService<IPersonService>();
            IPaymentService paymentService = host.Services.GetRequiredService<IPaymentService>();
            IReportService reportService = host.Services.GetRequiredService<IReportService>();
            IOrderService orderService = host.Services.GetRequiredService<IOrderService>();
            IFilterService filterService = host.Services.GetRequiredService<IFilterService>();


            try
            {
                Menu(seedData, personService, paymentService, reportService, orderService, filterService);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A critical error occurred: {ex.Message}");
            }

            //ManualTests(seedData, personService, paymentService, reportService, orderService, filterService);
        }
        public static void Menu(ISeedDataService seedData, IPersonService personService, IPaymentService paymentService, IReportService reportService, IOrderService orderService, IFilterService filterService)
        {
            bool next = false;
            do
            {
                try
                {
                    Console.WriteLine("Please enter the json file path! (Json.json)");
                    string jsonPath = Console.ReadLine();
                    //string jsonPath = "Json.json";
                    next = seedData.JsonReader(jsonPath);
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"File not found: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Invalid JSON format: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }

            } while (!next);


            bool exit = false;
            do
            {
                Console.Clear();
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("---------- Korsó Avató Nyilvántartás ----------");
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine();
                Console.WriteLine("1  ->  List All Data!");
                Console.WriteLine("2  ->  Create Person!");
                Console.WriteLine("3  ->  Delate Person!");
                Console.WriteLine("4  ->  Create Payment!");
                Console.WriteLine("5  ->  Delate Paymnet!");
                Console.WriteLine("6  ->  Update Payment!");
                Console.WriteLine("7  ->  Order By Date!");
                Console.WriteLine("8  ->  Order By Name!");
                Console.WriteLine("9  ->  Filter Already Orderd Mug(s)!");
                Console.WriteLine("10 ->  Filter Already Paid Mug(s)!");
                Console.WriteLine("11 ->  Filter Not Yet Paid Mug(s)!");
                Console.WriteLine("12 ->  Filter Finished Mug(s)!");
                Console.WriteLine("13 ->  Generate Of Reports!"); 
                Console.WriteLine("14 ->  Exit!");
                Console.Write("Choose an option (1-14): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        List<Person> allData = personService.GetAllPersonsIncludePayments();
                        ConsoleWriteOut(allData);
                        break;

                    case "2":
                        Console.WriteLine("Please enter the following information.");
                        
                        Console.WriteLine("Enter the name:");
                        string name = Console.ReadLine();
                        Console.WriteLine("Enter the neptun code (must be 6 characters long):");
                        string neptun = Console.ReadLine();
                        Console.WriteLine("Enter whether the person is a student or a teacher.");
                        Console.WriteLine("student - true");
                        Console.WriteLine("student - false");
                        bool studentOrteacher = bool.Parse(Console.ReadLine());
                        Console.WriteLine("Enter the mug special request (like: színes,üveg,műanyag) or null if don't have any.");
                        string specalMugRequest = Console.ReadLine();
                        Console.WriteLine("Enter the mug order status (like: befizetésre vár, befizetve, megrendelve, elkészült).");
                        string status = Console.ReadLine();

                        Person thePerson = new Person(name, neptun, studentOrteacher, specalMugRequest, status);
                        personService.CreatePerson(thePerson);
                        break;

                    case "3":
                        Console.WriteLine("Please enter the neptun code of the person you want to delete.");
                        string neptunDel = Console.ReadLine();

                        try
                        {
                            var personToFindDel = personService.GetPersonByNeptun(neptunDel);
                            personService.DeletePerson(personToFindDel.Id);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }      
                        break;

                    case "4":
                        Console.WriteLine("Please enter the following information.");
                        Console.WriteLine("Enter whether the payment has been made - true or not yet - false.");
                        bool isPaidOrNot = bool.Parse(Console.ReadLine());
                        Console.WriteLine("Enter the amount (be a real integer).");
                        decimal amount = int.Parse(Console.ReadLine());

                        if (isPaidOrNot)
                        {
                            Console.WriteLine("Enter the date of payment in YYYY-MM-DD format.");
                            DateTime date = DateTime.Parse(Console.ReadLine());
                            Payment thePayment = new Payment(isPaidOrNot, amount, date, null);
                            paymentService.CreatePayment(thePayment);
                        }
                        else
                        {
                            Payment thePayment = new Payment(isPaidOrNot, amount, null, null);
                            paymentService.CreatePayment(thePayment);
                        }
                        break;

                    case "5":
                        Console.WriteLine("Please enter the payment ID of the payment you want to delet.");
                        string paymentId = Console.ReadLine();
                        try
                        {
                            paymentService.DeletePayment(paymentId);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;

                    case "6":
                        Console.WriteLine("Please enter the neptun code of the person whose payment status you want to change.");
                        try
                        {
                            string neptunUpdatePayment = Console.ReadLine();
                            paymentService.UpdatePayment(neptunUpdatePayment);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;

                    case "7":
                        var orderByDate = orderService.OrderByDate();
                        ConsoleWriteOut(orderByDate);
                        break;

                    case "8":
                        var orderByName = orderService.OrderByName();
                        ConsoleWriteOut(orderByName);
                        break;

                    case "9":
                        var filter9 = filterService.FilterAlreadyOrderd();
                        ConsoleWriteOut(filter9);
                        break;

                    case "10":
                        var filter10 = filterService.FilterAlreadyPaid();
                        ConsoleWriteOut(filter10);
                        break;

                    case "11":
                        var filter11 = filterService.FilterNotPaidYet();
                        ConsoleWriteOut(filter11);
                        break;

                    case "12":
                        var filter12 = filterService.FilterFinishedMug();
                        ConsoleWriteOut(filter12);
                        break;

                    case "13":
                        Reports(reportService);
                        break;

                    case "14":
                        exit = true;
                        Console.WriteLine("Exit...");
                        break;

                    default:
                        Console.WriteLine("Invalid selection, please try again.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("Press a button to continue...");
                    Console.ReadKey();
                }

            } while (!exit);
        }
        private static void ManualTests(ISeedDataService seedData, IPersonService personService, IPaymentService paymentService, IReportService reportService, IOrderService orderService, IFilterService filterService)
        {
            string jsonPath = "Json.json";
            bool next = seedData.JsonReader(jsonPath);

            
            Person tesztPerson = new Person("AAAteszt Jani", "000000", true, null, "befizetésre vár");
            personService.CreatePerson(tesztPerson);
            
            var personFind = personService.GetPersonByNeptun(tesztPerson.Neptun_code);
            var personFind2 = personService.GetPersonByID(tesztPerson.Id);

            DateTime date1 = new DateTime(2023, 01, 01);
            DateTime date2 = new DateTime(2022, 01, 01);
            Payment tesztpay = new Payment(false, 2990, null, null);
            Payment tesztpay2 = new Payment(true, 2990, date1, null);
            Payment tesztpay3 = new Payment(true, 2990, date2, null);

            personService.AddPaymentToPerson(tesztPerson.Id, tesztpay);
            personService.AddPaymentToPerson(tesztPerson.Id, tesztpay2);
            personService.AddPaymentToPerson(tesztPerson.Id, tesztpay3);

            var order1 = personService.OrderPaymentsDesc(tesztPerson);
            var order2 = personService.OrderPaymentsDescTopNull(tesztPerson);

            paymentService.UpdatePayment(tesztPerson.Neptun_code);

            personService.UpdateSpecialRequests(tesztPerson.Neptun_code, "színes");

            var persons = personService.GetAllPerson();

            paymentService.DeletePayment(tesztpay3.Id);

            var payments = paymentService.ReadAllPayment();

            personService.DeletePerson(tesztPerson.Id);

            var orderByDate = orderService.OrderByDate();
            var orderByName = orderService.OrderByName();
            var filter9 = filterService.FilterAlreadyOrderd();
            var filter10 = filterService.FilterAlreadyPaid();
            var filter11 = filterService.FilterNotPaidYet();
            var filter12 = filterService.FilterFinishedMug();

            Reports(reportService);
        }
        private static void Reports(IReportService reportService)
        {
            reportService.CreatingDirectory();
            reportService.AnnualStatisticsReport();
            try
            {
                reportService.GenerateOrderableMugsReport();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            //reportService.GenerateParticipationStatisticsReport();
        }
        public static void IsPaidWriteOut(string name, string neprunCode)
        {
            Console.WriteLine();
            Console.WriteLine($"The payment has been made by {name} - {neprunCode}");
            Console.WriteLine();
        }
        public static void ThisPersonNotExistWriteOut(string neptunCode)
        {
            Console.WriteLine();
            Console.WriteLine($"The person with the specified neptun code, {neptunCode} cannot be found.");
            Console.WriteLine();
        }
        public static void ThisPaymentNotExistWriteOut(string neptunCode)
        {
            Console.WriteLine();
            Console.WriteLine($"There is no payment information associated with this neptun code, {neptunCode}.");
            Console.WriteLine();
        }
        public static void AlreadyPaidWriteOut(string name, string neprunCode)
        {
            Console.WriteLine();
            Console.WriteLine($"The person, {name} belonging to this neptun code, {neprunCode} has already been paid.");
            Console.WriteLine();
        }
        public static void ConsoleWriteOut(List<Person> data)
        {

            if (data == null || data.Count == 0)
            {
                Console.WriteLine("No data available to display.");
                return;
            }

            int currentPage = 0;
            int pageSize = 10;
            int totalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
            ConsoleKey keyPressed;

            do
            {
                Console.Clear();
                Console.WriteLine($"Page {currentPage + 1}/{totalPages}");
                Console.WriteLine();

                var currentPageData = data.Skip(currentPage * pageSize).Take(pageSize).ToList();

                foreach (Person person in currentPageData)
                {
                    Console.WriteLine($"{"Person ID",-42} {"Name",-20} {"Neptun Code",-15} {"Role",-10} {"Request",-25} {"Order Status",-15}");
                    string studentOrTeacher = person.Is_student ? "Student" : "Teacher";
                    Console.WriteLine($"{person.Id,-42} {person.Name,-20} {person.Neptun_code,-15} {studentOrTeacher,-10} {person.Special_requests ?? "None",-25} {person.Mug_order_status,-15}");

                    if (person.Payment.Any())
                    {
                        Console.WriteLine();
                        Console.WriteLine("  Payment Details:");

                        Console.WriteLine($"  {"Payment ID",-40} {"Is Paid",-12} {"Amount",-15} {"Date",-15}");
                        Console.WriteLine("  --------------------------------------------------------------------------------------------");
                        foreach (var payment in person.Payment)
                        {
                            string isPaid = payment.Is_paid ? "Yes" : "No";
                            string amountFormatted = string.Format("{0:C}", payment.Amount); // Currency format for amount
                            string dateFormatted = payment.Date.HasValue ? payment.Date.Value.ToString("yyyy-MM-dd") : "N/A";

                            Console.WriteLine($"  {payment.Id,-40} {isPaid,-12} {amountFormatted,-15} {dateFormatted,-15}");
                        }
                        Console.WriteLine("  --------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("  Nincs fizetési adat.");
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                }

                Console.WriteLine("Use the arrow keys to navigate pages. Press Escape twice to exit.");

                keyPressed = Console.ReadKey(true).Key;

                if (keyPressed == ConsoleKey.RightArrow && currentPage < totalPages - 1)
                {
                    currentPage++; 
                }
                else if (keyPressed == ConsoleKey.LeftArrow && currentPage > 0)
                {
                    currentPage--;
                }

            } while (keyPressed != ConsoleKey.Escape);

               
        }
    }
}
