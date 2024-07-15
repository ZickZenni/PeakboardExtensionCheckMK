using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PeakboardExtensionCheckMK;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CheckMK
{
    public class CheckMKApi
    {
        private readonly HttpClient _httpClient;

        private readonly string _baseUrl;

        public CheckMKApi(string baseUrl, string username, string password) {
            _baseUrl = $"{baseUrl}/check_mk/api/1.0";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{username} {password}");
            _httpClient.DefaultRequestHeaders.Add("Accept",  "application/json");
        }

        public async void GetServicesStatus()
        {
            string url = $"{_baseUrl}/objects/host/checkmk/collections/services";
            var response = await _httpClient.GetAsync(url);
          
        }

        public async void GetMetrics()
        {
            string url = $"{_baseUrl}/domain-types/metric/actions/get/invoke";

            var data = new
            {
                time_range = new {
                    start = "2024-07-15 07:54:11",
                    end = "2024-07-15 8:23:11",
                },
                reduce = "max",
                service_description = "Check_MK",
                host_name = "checkmk",
                type = "predefined_graph",
                graph_id = "cmk_cpu_time_by_phase"
            };
            string json = JsonConvert.SerializeObject(data);
            var response = await _httpClient.PostAsync(url, new JsonContent(json));
            string content = await response.Content.ReadAsStringAsync();
            var jsonResult = JsonConvert.DeserializeObject<JToken>(content);
            Console.WriteLine(jsonResult["step"].ToString());
        }
    }
}
