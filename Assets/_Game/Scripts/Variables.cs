using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using UnityEngine;

public static class Variables
{
    public static string ApiUrl = "";
    public static string SocketUrl = "";
    public static string ChatUrl = "";
    public static object Settings = null;
    public static int PlantsTurnId = 0;
    public static bool UserDragged = false;

    public static class TimeoutInfo
    {
        public static object ZoomMap = null;
        public static object DistrictInOut = null;
        public static object UpdateMapImageUpdateId = null;
    }

    public static class ScrollInfo
    {
        public static bool IsScrolling = false;
        public static float UpAmount = 0f;
        public static float DownAmount = 0f;
    }

    public static class MapInfo
    {
        public static bool FirstDraw = true;
        public static Texture2D MapImage = new Texture2D(1, 1);
        public static float MapMinWidth = 0f;
        public static float MapMinHeight = 0f;
        public static float MapMaxWidth = 0f;
        public static float MapMaxHeight = 0f;
        public static int[] ViewDistrictIds = new int[] { };
    }

    public static class MapScaleInfo
    {
        public static float Previous = 1f;
        public static float Current = 1f;
        public static readonly float[] List = new float[] { 1f, 2f, 4f, 8f, 16f, 32f, 64f, 128f };
        public static readonly float MaxScale = 128f;
        public static float ZoomPosX = 0f;
        public static float ZoomPosY = 0f;
        public static float MobileTouchStartCenterPosX = 0f;
        public static float MobileTouchStartCenterPosY = 0f;
        public static float MobileTouchStartDistance = 0f;
        public static bool MobileTouchScaleIsChanged = false;
    }

    public static class MapMoveInfo
    {
        public static float CurrentLeft = 0f;
        public static float CurrentTop = 0f;
        public static float CurrentPosX = 0f;
        public static float CurrentPosY = 0f;
        public static float MovedPosX = 0f;
        public static float MovedPosY = 0f;
        public static float FinalLeft = 0f;
        public static float FinalTop = 0f;
    }

    public static class MapCanvasInfo
    {
        public static int DrawMapCase = 0;
        public static float XStartPos = 0f;
        public static float YStartPos = 0f;
        public static float XEndPosLimit = 0f;
        public static float YEndPosLimit = 0f;
        public static float BringMapWidth = 0f;
        public static float BringMapHeight = 0f;
        public static float XPosStartOfCanvas = 0f;
        public static float YPosStartOfCanvas = 0f;
        public static float WidthOfCanvas = 0f;
        public static float HeightOfCanvas = 0f;
    }
}

[System.Serializable]
public class SettingsInfo
{
    public int mapId;
    public int averageRabbitProceedIntervalSeconds;
    public int averageWolfProceedIntervalSeconds;
    public int mapMinWidth;
    public int mapMinHeight;
    public int districtWidth;
    public int districtHeight;
    public int animalCoordinateScale;
    public int animalMaxGrowthForScale;
    public string[] environmentCode;
    public string[] weedProceedCode;
    public string[] animalGender;
    public string[] animalLifeStatus;
    public string[] rabbitActionStatus;
    public string[] wolfActionStatus;
    public string mapImageUpdateId;
    public bool plantProceedAccelerated;
    public int seasonId;
}

[MessagePackObject]
public class ResponseWeedInfo 
{
    [Key(0)]
    public int WeedValue { get; set; }
    [Key(1)]
    public bool RabbitFecesExists { get; set; }
    [Key(2)]
    public bool WolfFecesExists { get; set; }
}

[MessagePackObject]
public class ResponseTreeInfo {
    [Key(0)]
    public ulong Id { get; set; }
    [Key(1)]
    public string CenterTileString { get; set; } = "";
    [Key(2)]
    public ResponseTile SizeInfo { get; set; } = new ResponseTile();
    [Key(3)]
    public int Growth { get; set; } = 1;
    [Key(4)]
    public TreeProceedCode ProceedCode = TreeProceedCode.germination;
    [Key(5)]
    public HashSet<ulong> FruitIds { get; set; }
}

[MessagePackObject]
public class ResponseTile
{
    [Key(0)]
    public int X { get; set; }
    [Key(1)]
    public int Y { get; set; }
}

public enum TreeProceedCode 
{
    germination,
    growth1,
    growth2,
    growth3,
    active1,
    active2,
    active3,
    active4,
    active5,
    wither1,
    wither2,
    dead,
    none,
}

[MessagePackObject]
public class Animal 
{
    [Key(18)]
    public int MapTileSightRange { get; set; } = 0;
    [Key(19)]
    public int AnimalTileMovableRange { get; set; } = 0;
    [Key(20)]
    public HashSet<int> ConcernedDistrictIds { get; set; }
    [Key(21)]
    public int DeadCount { get; set; } = 0;
    [Key(22)]
    public int DeadCountMax { get; set; } = 0;
    [Key(23)]
    public long NextActionDateTime { get; set; } = 0;
    [Key(24)]
    public bool CanInterfere { get; set; } = true;
    [Key(25)]
    public bool DoingInteraction { get; set; } = false;

    [Key(27)]
    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;

    [Key(26)]
    public long UpdateTimeUnix 
    {
        get => new DateTimeOffset(UpdateTime).ToUnixTimeSeconds();
        set => UpdateTime = DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime;
    }
}

public enum AnimalGender 
{
    male,
    female,
    neutral,
    none
}

public enum AnimalLifeStatus 
{
    baby,
    child,
    young,
    adult,
    old,
    death,
    none,
}

[MessagePackObject]
public class AnimalMovableTileInfo 
{
    [Key(0)]
    public HashSet<ResponseTile> all;
    [Key(1)]
    public HashSet<ResponseTile> border;
}

[MessagePackObject]
public class Rabbit : Animal {
    [Key(0)]
    public ulong Id { get; set; }
    [Key(1)]
    public AnimalGender Gender { get; set; }
    [Key(2)]
    public List<string> MovedTilesStrings { get; set; } = new List<string>();
    public string GetCurrentPositionAsString() { return CurrentPositionString; }
    [Key(3)]
    public List<string> ReservedTilesStrings { get; set; } = new List<string>();
    public List<string> GetMovedTilesAsString() { return MovedTilesStrings.Select(tile => tile.ToString()).ToList(); }
    [Key(4)]
    public string CurrentPositionString { get; set; } = string.Empty;
    [Key(5)]
    public RabbitActionStatus ActionStatus { get; set; } = RabbitActionStatus.idle;
    [Key(6)]
    public AnimalLifeStatus LifeStatus { get; set; } = AnimalLifeStatus.baby;
    [Key(7)]
    public int Energy { get; set; } = 3;
    [Key(8)]
    public int Hunger { get; set; } = 0;
    [Key(9)]
    public int Growth { get; set; } = 0;
    [Key(10)]
    public int MatingCount { get; set; } = 0;
    [Key(11)]
    public int MatingMaxCount { get; set; } = Settings.FemaleMaxMatingCount;
    [Key(12)]
    public int PregnantCount { get; set; } = 0;
    [Key(13)]
    public int PregnantStepMaxCount { get; set; } = Settings.RabbitPregnantMaxCount;
    [Key(14)]
    public ulong FemaleTargetId { get; set; } = 0;
    [Key(15)]
    public ulong MatingTargetId { get; set; } = 0;
    [Key(16)]
    public bool Moved { get; set; } = false;
    [Key(17)]
    public NodeKind NodeKind { get; set; } = NodeKind.rabbit1;
}

[MessagePackObject]
public class Wolf : Animal 
{
    [Key(0)]
    public ulong Id { get; set; }
    [Key(1)]
    public ulong MotherId { get; set; }
    [Key(2)]
    public AnimalGender Gender { get; set; }
    [Key(3)]
    public List<string> MovedTilesStrings { get; set; }
    [Key(4)]
    public List<string> ReservedTilesStrings { get; set; } = new List<string>();
    [Key(5)]
    public string CurrentPositionString { get; set; } = "";
    [Key(6)]
    public WolfActionStatus ActionStatus { get; set; } = WolfActionStatus.idle;
    [Key(7)]
    public AnimalLifeStatus LifeStatus { get; set; } = AnimalLifeStatus.baby;
    [Key(8)]
    public int Energy { get; set; } = 3;
    [Key(9)]
    public int Hunger { get; set; } = 0;
    [Key(10)]
    public int Growth { get; set; } = 0;
    [Key(11)]
    public int MatingCount { get; set; } = 0;
    [Key(12)]
    public int MatingMaxCount { get; set; } = Settings.FemaleMaxMatingCount;
    [Key(13)]
    public int PregnantCount { get; set; } = 0;
    [Key(14)]
    public int PregnantStepMaxCount { get; set; } = Settings.WolfPregnantMaxCount;
    [Key(15)]
    public ulong FemaleTargetId { get; set; } = 0;
    [Key(16)]
    public ulong MatingTargetId { get; set; } = 0;
    [Key(17)]
    public bool Moved { get; set; } = false;
}

public enum RabbitActionStatus 
{
    idle,
    movingToWeed,
    eating,
    jump,
    sleep,
    movingToMate,
    mating,
    pregnant,
    breeding,
    dead,
}

public enum WolfActionStatus 
{
    idle,
    movingToRabbit,
    eating,
    jump,
    sleep,
    movingToMate,
    mating,
    pregnant,
    breeding,
    dead,
}

public enum NodeKind
{
    plantWeed,
    rabbit1,
    rabbit2,
    rabbit3,
    datahub,
    wolf1,
    relay,
    outpost_ap,
    outpost_eu,
    outpost_us,
}

public static class Settings 
{
    public static int FemaleMaxMatingCount = 3;
    public static int RabbitPregnantMaxCount = 3;
    public static int WolfPregnantMaxCount = 3;
}