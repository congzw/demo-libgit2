using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace GitInit.ViewModel
{
    public class MyConfigHelper
    {
        private static string filePath = "MyConfig.json";
        public MyConfig TryLoad()
        {
            if (!File.Exists(filePath))
            {
                return new MyConfig();
            }

            var json = File.ReadAllText(filePath);
            var config = JsonConvert.DeserializeObject<MyConfig>(json);
            return config;
        }
        public void TrySave(MyConfig config)
        {
            if (config == null)
            {
                return;
            }
            var json = JsonConvert.SerializeObject(config);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        public static MyConfigHelper Instance = new MyConfigHelper();
    }
}