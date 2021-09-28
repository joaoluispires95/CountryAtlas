namespace ProjetoPaíses.Services
{
    using Newtonsoft.Json;
    using Modelos;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ApiService
    {
        public async Task<Response> GetLists(string urlBase, string controller, string type)
        {
            try
            {
                var client = new HttpClient();

                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync(controller);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response()
                    {
                        IsSuccessful = false,
                        Message = result
                    };
                }

                dynamic obj;

                switch (type)
                {
                    case "ProjetoPaíses.Modelos.Pais":
                        obj = JsonConvert.DeserializeObject<List<Pais>>(result);
                        break;
                    case "ProjetoPaíses.Modelos.Information":
                        obj = JsonConvert.DeserializeObject<List<Information>>(result);
                        break;
                    default:
                        obj = "";
                        break;
                }

                return new Response
                {
                    IsSuccessful = true,
                    Result = obj
                };
            }
            catch (Exception e)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = e.Message
                };
            }
        }
    }
}
