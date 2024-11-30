using System.Text;

namespace BlogReceptionist.Utilities
{
    public static class Env
    {
        static Env()
        {
            foreach (var key in EnvKeys)
            {
                var local = Environment.GetEnvironmentVariable(key);
                if (local is null)
                {
                    Console.WriteLine($"Missing variable {key}");
                    Environment.Exit(5);
                }
                envDic.Add(key, local);
            }
        }
        public static List<string> EnvKeys = ["USERURL", "MQURL", "REDISURL"];
        public static Dictionary<string, string> envDic = new Dictionary<string, string>();
        public static string USERURL => envDic["USERURL"];
        public static string MQURL => envDic["MQURL"];
        public static string REDISURL => envDic["REDISURL"];
    }
}
