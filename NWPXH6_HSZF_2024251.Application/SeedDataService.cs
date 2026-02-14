using Newtonsoft.Json;
using NWPXH6_HSZF_2024251.Model;
using NWPXH6_HSZF_2024251.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Application
{
    public interface ISeedDataService 
    {
        public bool JsonReader(string jsonPath);
    }
    public class SeedDataService : ISeedDataService
    {
        private ISeedDataProvider _seedDataProvider;

        public SeedDataService(ISeedDataProvider dp)
        {
            this._seedDataProvider = dp;
        }
        public bool JsonReader(string jsonPath)
        {
            bool succeeded = false;

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException(jsonPath);
            }

            string jsonString = File.ReadAllText(jsonPath);
            Root rootObject = JsonConvert.DeserializeObject<Root>(jsonString);

            if (rootObject != null && rootObject.Persons != null)
            {
                foreach (var person in rootObject.Persons)
                {
                    if (person != null && person.Payment != null)
                    {
                        _seedDataProvider.LoadingPersonToTheDB(person);

                        foreach (var payment in person.Payment)
                        {
                            _seedDataProvider.LoadingPaymentToTheDB(payment);
                        }
                    }
                }

                _seedDataProvider.SaveAll();
                succeeded = true;
                return succeeded;
            }
            else
            {
                throw new JsonException("Failed to parse JSON data.");
            }
        }
    }
}
