using System;
using System.Net.Http;

namespace Launcher.Services
{
    public class RestService
    {
        private HttpClient _client;
        private static RestService _restService;


        public RestService()
        {
            _client = new HttpClient();
        }

        public static RestService GetRestService()
        {
            return _restService ??= new RestService();
        }

        public HttpClient GetClient()
        {
            return _client;
        }
    }
}