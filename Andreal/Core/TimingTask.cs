using System.Timers;
using AndrealClient.Data.Api;
using AndrealClient.Data.Json.Pjsk;
using AndrealClient.Model.Pjsk;
using Timer = System.Timers.Timer;

namespace AndrealClient.Core;

internal static class TimingTask
{
    private static ulong _timerCount;

    [NonSerialized] private static readonly Timer Timer;

     static TimingTask()
    {
        Timer = new(240000);
        Timer.Elapsed += PjskUpdate;
        Timer.Elapsed += Clean;
        Timer.Elapsed += (_, _) => ++_timerCount;
        Timer.AutoReset = true;
        Timer.Enabled = true;
    }

    internal static void Abort()
    {
        Timer.Stop();
        Timer.Dispose();
    }

    private static void Clean(object? source, ElapsedEventArgs e)
    {
        if (_timerCount % 15 != 0) return;

        var time = DateTime.Now.AddHours(-2);

        foreach (var j in new DirectoryInfo("./Andreal/TempImage/").GetFiles().Where(j => time > j.LastWriteTime)) j.Delete();
    }

    private static async void PjskUpdate(object? source, ElapsedEventArgs e)
    {
        try
        {
            if (_timerCount % 15 != 0) return;

            List<PjskMusics> musics = null!;
            List<PjskMusicMetas> musicMetas = null!;
            List<PjskMusicDifficulties> musicDifficulties = null!;

            await Task.WhenAll(Task.Run(() => musics = PjskApi.PjskMusics().Result),
                               Task.Run(() => musicMetas = PjskApi.PjskMusicMetas().Result),
                               Task.Run(() => musicDifficulties = PjskApi.PjskMusicDifficulties().Result));

            foreach (var item in musics)
                if (!SongInfo.CheckFull(item.Id))
                {
                    var metas = musicMetas.Where(i => i.MusicId == item.Id).ToList();
                    if (metas.Any())
                        SongInfo.Insert(item, metas);
                    else
                    {
                        var difficulties = musicDifficulties.Where(i => i.MusicId == item.Id).ToList();
                        if (difficulties.Any()) SongInfo.Insert(item, difficulties);
                    }
                }
        }
        catch (Exception ex)
        {
            Reporter.ExceptionReport(ex);
        }
    }
}
