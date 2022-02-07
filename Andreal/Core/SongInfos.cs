using System.Collections;

namespace AndrealClient.Core;

internal class SongInfos : IEnumerable<SongInfos.ISongInfo>
{
    private readonly ISongInfo[] _infos;
    private SongInfos(ISongInfo[] infos) { _infos = infos; }

    public IEnumerator<ISongInfo> GetEnumerator() => ((IEnumerable<ISongInfo>)_infos).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _infos.GetEnumerator();

    public static implicit operator SongInfos(ISongInfo[] infos) => new(infos);

    internal interface ISongInfo
    {
        internal string SongName { get; }
    }
}
