using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Management.Instrumentation;
using System.Data.SqlClient;
using ApiTest.Classes;



namespace ApiTest
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("********************");
                Console.WriteLine("1 - get token");
                Console.WriteLine("2 - get next object");
                Console.WriteLine("3 - register account (method 2)");
                Console.WriteLine("4 - get scene");
                Console.WriteLine("5 - img cache test");
                Console.WriteLine("6 - get user stats");
                Console.WriteLine("7 - test stats creating");
                Console.WriteLine("other - exit application");

                string userResponse = Console.ReadLine();
                if (userResponse == "1")
                {
                    ShowToken();
                }
                else if (userResponse == "2")
                {
                    GetNextObject();
                }
                else if (userResponse == "3")
                {
                    CreateAccount2();
                }
                else if (userResponse == "4")
                {
                    GetSceneInner();
                }
                else if (userResponse == "5")
                {
                    TestImageCache();
                }
                else if (userResponse == "6")
                {
                    GetUserStats();
                }
                else if (userResponse == "7")
                {
                    RegisterUserChanges();
                }
                else
                {
                    Environment.Exit(0);
                }
            }

        }

        private static void RegisterUserChanges()
        {
            using (var client = ServerAuth.CreateClient())
            {

                AdmiralStats sampleStats = new AdmiralStats(true);

                var postTask = client.PostAsJsonAsync<AdmiralStats>(ServerAuth.ServerAddress + "/api/Admiral", sampleStats);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine("stat change registered");
                }
                else
                {
                    Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                }

            }
        }

        private static void GetUserStats()
        {
            using (var client = ServerAuth.CreateClient())
            {

                var response = client.GetAsync(ServerAuth.ServerAddress + "/api/Admiral/GetStats").Result;
                AdmiralStats tStats = JsonConvert.DeserializeObject<AdmiralStats>(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine("Stat points left: {0}", tStats.StatPointsLeft);
                foreach(PlayerStat stat in tStats.Stats)
                {
                    Console.WriteLine("{0} : {1}", stat.Name, stat.Value);
                }

            }
        }

        private static void ShowToken()
        {

            Console.WriteLine(ServerAuth.GetToken());

        }

        public class AdmiralStats
        {
            public List<PlayerStat> Stats { get; set; }
            public int StatPointsLeft { get; set; }
            public AdmiralStats() { }

            public AdmiralStats(bool createSampleFlag)
            {
                Stats = new List<PlayerStat>();
                StatPointsLeft = 0;

                Stats.Add(new PlayerStat(8, 3));
                Stats.Add(new PlayerStat(14, 1));
                Stats.Add(new PlayerStat(17, 2));
            }
        }

        public class PlayerStat
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Value { get; set; }
            public string DescriptionEnglish { get; set; }
            public string DescriptionRussian { get; set; }
            public int OrderIdx { get; set; }

            public PlayerStat() { }

            public PlayerStat(int Id, int Value)
            {
                this.Id = Id;
                this.Value = Value;
            }

            public PlayerStat Copy()
            {
                PlayerStat newElement = new PlayerStat();
                newElement.Id = this.Id;
                newElement.Name = this.Name;
                newElement.Value = this.Value;
                newElement.DescriptionEnglish = this.DescriptionEnglish;
                newElement.DescriptionRussian = this.DescriptionRussian;
                newElement.OrderIdx = this.OrderIdx;

                return newElement;

            }

        }

        private static void CreateAccount2()
        {

            //var newUser = new AccountData() { SteamAccountId = "Babster", aType = AccountData.ActionType.CreateUser };
            ServerAuth.RegisterBindingModel newUser = new ServerAuth.RegisterBindingModel();
            newUser.Email = "BabsterMail";
            newUser.Name = "BabsterId";
            newUser.Password = "samplepass123";
            newUser.ConfirmPassword = "samplepass123";

            using (var client = new HttpClient())
            {

                ServerAuth.RegisterBindingModel model = new ServerAuth.RegisterBindingModel();
                var postTask = client.PostAsJsonAsync<ServerAuth.RegisterBindingModel>(ServerAuth.ServerAddress + "/api/Account/Register", newUser);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var resString = result.Content.ReadAsStringAsync().Result;
                    AccountData insertedUser = JsonConvert.DeserializeObject<AccountData>(resString);
                    Console.WriteLine("Account {0} inserted with additional data: {1}", insertedUser.SteamAccountId, insertedUser.AdditionalData);
                }
                else
                {
                    Console.WriteLine(result.StatusCode);
                }
            }

            Console.WriteLine("*********************************");
            Console.WriteLine("");

        }

        //private static void Check

        

        private static void GetNextObject()
        {
            var newUser = new AccountData() { SteamAccountId = "Babster", aType = AccountData.ActionType.CheckPwd  };
            newUser.Name = "Babster";
            newUser.AdditionalData = "temp pwd";

            using (var client = ServerAuth.CreateClient())
            {

                client.BaseAddress = new Uri(ServerAuth.ServerAddress + "/api/Story/NextStep");

                string tStr = JsonConvert.SerializeObject(newUser);
                HttpContent tContent = new StringContent(tStr, null, "application/json");

                var response = client.GetAsync(client.BaseAddress);
                response.Wait();
                var result = response.Result;
                var resString = result.Content.ReadAsStringAsync().Result;
                StringAndInt  nextStep = JsonConvert.DeserializeObject<StringAndInt>(resString);
                Console.WriteLine("Next step type: {0}, id: {1}", nextStep.StrValue , nextStep.IntValue );

            }

        }

        private static void GetSceneInner()
        {
            using (var client = ServerAuth.CreateClient())
            {

                client.BaseAddress = new Uri(ServerAuth.ServerAddress + "/api/Story/");

                var postTask = client.GetAsync("scene?SceneId=1");
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var resString = result.Content.ReadAsStringAsync().Result;
                    RebelScene  sceneData = JsonConvert.DeserializeObject<RebelScene>(resString);
                    RebelSceneFull RF = new RebelSceneFull(ref sceneData);
                    Console.WriteLine("Loaded scene {0}", RF.ToString());
                }
                else
                {
                    Console.WriteLine(result.StatusCode);
                }
            }
        }

        private static void TestImageCache()
        {
            RebelImage img = ApiTest.Classes.ImageCache.GetImage(4);
            Console.WriteLine(img.ToString());
        }

        public class AccountData
        {
            public string SteamAccountId { get; set; }
            public string Name { get; set; }

            public int Id { get; set; }
            public string AdditionalData { get; set; }

            public ActionType aType { get; set; }

            public enum ActionType
            {
                CreateUser = 1,
                CheckPwd = 2,
                StepCompleted = 3
            }

            public ActionResult aResult { get; set; }
            public enum ActionResult
            {
                Success = 1,
                PasswordCorrect = 2,
                PasswordIncorrect = 3
            }

        }

        public struct StringAndInt
        {
            public string StrValue { get; set; }
            public int IntValue { get; set; }
        }

        


    }
}
