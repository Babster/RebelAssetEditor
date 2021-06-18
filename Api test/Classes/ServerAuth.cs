using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public static class ServerAuth
{

    public const string  ServerAddress = "https://localhost:44348";

    public static HttpClient CreateClient()
    {
        string accessToken = GetToken();
        var client = new HttpClient();
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }
        return client;
    }
    private static string securityTokenStr;
    public static string GetToken()
    {
        if (string.IsNullOrEmpty(securityTokenStr))
        {
            string userName = "BabsterId";
            string password = "[N>awg56Q>ipN#!";
            Dictionary<string, string> tokenInfo = GetTokenDictionary(userName, password);
            securityTokenStr = tokenInfo["access_token"];
        }
        return securityTokenStr;
    }
    public static Dictionary<string, string> GetTokenDictionary(string userName, string password)
    {
        var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "grant_type", "password" ),
                    new KeyValuePair<string, string>( "username", userName ),
                    new KeyValuePair<string, string> ( "Password", password )
                };
        var content = new FormUrlEncodedContent(pairs);

        using (var client = new HttpClient())
        {
            var response =
                client.PostAsync(ServerAuth.ServerAddress + "/Token", content).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            // Десериализация полученного JSON-объекта
            Dictionary<string, string> tokenDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            return tokenDictionary;
        }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}

