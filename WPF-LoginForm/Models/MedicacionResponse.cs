using Newtonsoft.Json;
using System.Collections.Generic;
using WPF_LoginForm.Models;

namespace WPF_LoginForm.Models
{
    public class MedicacionResponse
    {
        [JsonProperty("data")]
        public List<MedicacionModel> Data { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
