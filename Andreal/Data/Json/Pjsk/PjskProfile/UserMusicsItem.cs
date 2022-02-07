﻿using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk.PjskProfile;

public class UserMusicsItem
{
    [JsonProperty("musicId")] public int MusicId { get; set; }

    [JsonProperty("userMusicDifficultyStatuses")]
    public List<UserMusicDifficultyStatusesItem> UserMusicDifficultyStatuses { get; set; }
}
