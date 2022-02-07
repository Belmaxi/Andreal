using AndrealClient.Core;
using Sora;
using Sora.Net.Config;
using YukariToolBox.LightLog;
using Path = AndrealClient.Core.Path;

Log.LogConfiguration.EnableConsoleOutput().SetLogLevel(LogLevel.Info);

ushort port;
if (Path.Portconfig.FileExists)
    port = ushort.Parse(File.ReadAllText(Path.Portconfig ));
else
{
    Console.Write("请输入WebSocket端口(1 - 65535,不建议使用常用端口如80,443等):    ");
    port = ushort.Parse(Console.ReadLine()!);
    File.WriteAllText(Path.Portconfig, port.ToString());
}

var service = SoraServiceFactory.CreateService(new ServerConfig { Port = port });

service.Event.OnGroupMessage += async (_, eventArgs) => External.Process(eventArgs.SoraApi,1, eventArgs.SourceGroup, eventArgs.Sender,
                                                                         eventArgs.Message.GetText(),eventArgs.Message.MessageId);

service.Event.OnPrivateMessage += async (_, eventArgs) => External.Process(eventArgs.SoraApi,eventArgs.IsTemporaryMessage
                                                                               ? 2
                                                                               : 0, 0, eventArgs.Sender,
                                                                           eventArgs.Message.GetText(),eventArgs.Message.MessageId);

//连接事件
service.ConnManager.OnOpenConnectionAsync += (connectionId, eventArgs) =>
                                             {
                                                 Log.Debug("Andreal|OnOpenConnectionAsync",
                                                           $"connectionId = {connectionId} type = {eventArgs.Role}");
                                                 return ValueTask.CompletedTask;
                                             };
//连接关闭事件
service.ConnManager.OnCloseConnectionAsync += (connectionId, eventArgs) =>
                                              {
                                                  Log.Debug("Andreal|OnCloseConnectionAsync",
                                                            $"uid = {eventArgs.SelfId} connectionId = {connectionId} type = {eventArgs.Role}");
                                                  return ValueTask.CompletedTask;
                                              };
//连接成功元事件
service.Event.OnClientConnect += (_, _) =>
                                 {
                                     Log.Info("Andreal|Message", "交流群 191234485");
                                     External.Initialize();
                                     return ValueTask.CompletedTask;
                                 };


await service.StartService();

await Task.Delay(-1);
