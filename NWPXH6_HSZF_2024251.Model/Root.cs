using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Model
{
    public class Root
    {
        [JsonPropertyName("persons")]
        public List<Person> Persons { get; set; }
    }
}
