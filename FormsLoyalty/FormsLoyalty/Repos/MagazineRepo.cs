using FormsLoyalty.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Repos
{
    public class MagazineRepo : IMagazineRepo
    {
        string MagazineConfigurationURL = $"{AppData.magazineURL}index.php/rest/V1/";


        public async Task<string> GetToken(object jObject, string methodname)
        {
            using (var client = new HttpClient())
            {
                string stringPayload = JsonConvert.SerializeObject(jObject);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                string content = string.Empty;
                await Task.Run(async () =>
                 {
                     HttpResponseMessage httpResponse = await client.PostAsync($"{MagazineConfigurationURL}{methodname}", httpContent);
                     content = await httpResponse.Content.ReadAsStringAsync();

                 });
                client.Dispose();
                return content;
            }
        }

        public async Task<string> GetMagazinesByToken(string token, string methodname)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                string content = string.Empty;
                await Task.Run(async () =>
                {
                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{MagazineConfigurationURL}{methodname}");
                    HttpResponseMessage response = await client.SendAsync(httpRequest);

                    content = await response.Content.ReadAsStringAsync();

                });
                client.Dispose();
                return content;
            }
        }

    }
}
