namespace AndrealClient.Model.Pjsk;

[Serializable]
internal class PjskMusicMetadata
{
    public int SongId { get; set; }
    public string Songname { get; set; }
    public string Categories { get; set; }
    public string Lyricist { get; set; }
    public string Composer { get; set; }
    public double MusicTime { get; set; }
    public int EventRate { get; set; }
    public string Level { get; set; }
    public string Note { get; set; }
    public string BaseScore { get; set; }
    public string FeverScore { get; set; }
    public string AssetbundleName { get; set; }
    public long PublishedAt { get; set; }
}
