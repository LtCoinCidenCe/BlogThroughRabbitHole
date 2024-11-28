using System.Text;

namespace UserServer.Utilities
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
            Encoding.UTF8.TryGetBytes(APPSECRET, APPSECRETBYTES, out int _);
        }
        public static List<string> EnvKeys = ["DATABASEURL", "APPSECRET"];
        public static Dictionary<string, string> envDic = new Dictionary<string, string>();
        public static string DATABASEURL => envDic["DATABASEURL"];
        public static string APPSECRET => envDic["APPSECRET"];
        public static byte[] APPSECRETBYTES { get; } = new byte[128];
    }
}
