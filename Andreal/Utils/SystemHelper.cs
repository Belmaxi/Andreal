using System.Net;
using AndrealClient.Data.Api;
using Newtonsoft.Json;
using ThesareaClient.Data.Json;
using Path = AndrealClient.Core.Path;

namespace AndrealClient.Utils;

internal static class SystemHelper
{
    private static ThesareaConfig _config = null!;
    
    public static void Init()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        ServicePointManager.ServerCertificateValidationCallback = (_, _, _, _) => true;
        ServicePointManager.DefaultConnectionLimit = 512;
        ServicePointManager.Expect100Continue = false;
        ServicePointManager.UseNagleAlgorithm = false;
        ServicePointManager.ReusePort = true;
        ServicePointManager.CheckCertificateRevocationList = true;
        WebRequest.DefaultWebProxy = null;

        if (!File.Exists(Path.ApiConfig))
        {
            File.WriteAllText(Path.ApiConfig,
                              "{\"unlimitedapiurl\": \"https://exampleapi.example.com/api/v0\",\"unlimitedtoken\": \"your token here\",\"limitedapiurl\": \"\",\"limitedtoken\": \"\"}");
            Console.WriteLine("ThesareaConfig默认配置文件已生成，请修改 ThesareaConfig.json 后重新启动!");
            Console.WriteLine("按任意键结束...");
            Console.ReadKey();
            Environment.Exit(-1);
        }

        _config = JsonConvert.DeserializeObject<ThesareaConfig>(File.ReadAllText(Path.ApiConfig))!;
       
        ArcaeaLimitedApi.Init(_config);
        ArcaeaUnlimitedApi.Init(_config);
        //SqliteHelper.TryCreateTable();
    }
}
