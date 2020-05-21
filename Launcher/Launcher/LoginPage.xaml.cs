using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AuthServer.Models;
using Launcher.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Launcher
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private const string Url = "https://sshcity.vinetos.fr:5001/";
        private static readonly Color DefaultColor = Color.White;
        private static readonly Color SuccessColor = Color.Green;
        private static readonly Color ErrorColor = Color.Red;
        private readonly HttpClient _client;

        public LoginPage()
        {
            InitializeComponent();

            //specify to use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _client = new HttpClient(clientHandler) {BaseAddress = new Uri(Url)};
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async void OnClicked(object sender, EventArgs e)
        {
            DisableEntries();
            var content = new StringContent(
                JsonConvert.SerializeObject(AuthModel.Of(UsernameEntry.Text, PasswordEntry.Text)),
                Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("users/authenticate", content);

            if (!response.IsSuccessStatusCode)
            {
                // Handle failure
                // Wrong username or password
                MarkAsErrored();
                EnableEntries();
                return;
            }

            var user = await response.Content.ReadAsStringAsync();
            var authenticatedModel = JsonConvert.DeserializeObject<AuthenticatedModel>(user);
            MarkAsSucceeded();
            LoginButton.Text = "Chargement";
            // Add Header
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authenticatedModel.Token);
            response = await _client.GetAsync($"games/{authenticatedModel.GameId}");
            var gameData = "";
            if (response.IsSuccessStatusCode)
                // Save found
                gameData = await response.Content.ReadAsStringAsync();


            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory(appdata); // Create if not exists
            var exec = Path.Combine(appdata, "SSHCity.exe");
            Process.Start(exec, $"{user} # {gameData}");
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (LoginButton.IsEnabled && !AreInputsFilled())
                LoginButton.IsEnabled = false;
            else if (!LoginButton.IsEnabled && AreInputsFilled())
                LoginButton.IsEnabled = true;

            // Reset color
            if (UsernameEntry.TextColor != ErrorColor)
                return;
            UsernameEntry.TextColor = DefaultColor;
            UsernameEntry.PlaceholderColor = DefaultColor;
            PasswordEntry.TextColor = DefaultColor;
            PasswordEntry.PlaceholderColor = DefaultColor;
        }

        private void DisableEntries()
        {
            UsernameEntry.IsEnabled = false;
            PasswordEntry.IsEnabled = false;
            LoginButton.IsEnabled = false;
        }

        private void EnableEntries()
        {
            UsernameEntry.IsEnabled = true;
            PasswordEntry.IsEnabled = true;
            LoginButton.IsEnabled = true;
        }

        private void MarkAsErrored()
        {
            UsernameEntry.TextColor = ErrorColor;
            UsernameEntry.PlaceholderColor = ErrorColor;
            PasswordEntry.TextColor = ErrorColor;
            PasswordEntry.PlaceholderColor = ErrorColor;
        }

        private void MarkAsSucceeded()
        {
            UsernameEntry.TextColor = SuccessColor;
            UsernameEntry.PlaceholderColor = SuccessColor;
            PasswordEntry.TextColor = SuccessColor;
            PasswordEntry.PlaceholderColor = SuccessColor;
        }

        private bool AreInputsFilled()
        {
            return !string.IsNullOrEmpty(UsernameEntry.Text) && !string.IsNullOrEmpty(PasswordEntry.Text);
        }
    }
}