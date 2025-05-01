using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Linq;

[Serializable]
public class WeedData 
{
    public int id;
    public TileBase tileBase;
}

[Serializable]
public class TreeData 
{
    public int id;
    public Sprite sprite;
}

[Serializable]
public class RabbitData 
{
    public RabbitActionStatus actionStatus;
    public Sprite[] sprites;
}

[Serializable]
public class WolfData 
{
    public WolfActionStatus actionStatus;
    public Sprite[] sprites;
}

[Serializable]
public class PoopData 
{
    public int id;
    public TileBase tileBase;
}

[Serializable]
public class SeasonData 
{
    public string id;
    public Color color;
}

[CreateAssetMenu(fileName = "DB", menuName = "Scriptable Objects/DB")]
public class DB : ScriptableObject
{
    [SerializeField] List<WeedData> weedDatas;
    [SerializeField] List<TreeData> treeDatas;
    [SerializeField] List<RabbitData> rabbitDatas;
    [SerializeField] List<WolfData> wolfDatas;
    [SerializeField] List<PoopData> poopDatas;
    [SerializeField] List<SeasonData> seasonDatas;

    Dictionary<int, WeedData> weedDataDict;
    Dictionary<int, TreeData> treeDataDict;
    Dictionary<RabbitActionStatus, RabbitData> rabbitDataDict;
    Dictionary<WolfActionStatus, WolfData> wolfDataDict;
    Dictionary<int, PoopData> poopDataDict;
    Dictionary<string, SeasonData> seasonDataDict;



    public void Caching()
    {
        weedDataDict = weedDatas.ToDictionary(data => data.id);
        treeDataDict = treeDatas.ToDictionary(data => data.id);
        rabbitDataDict = rabbitDatas.ToDictionary(data => data.actionStatus);
        wolfDataDict = wolfDatas.ToDictionary(data => data.actionStatus);
        poopDataDict = poopDatas.ToDictionary(data => data.id);
        seasonDataDict = seasonDatas.ToDictionary(data => data.id);
    }

    public WeedData GetWeedData(int id)
    {
        return weedDataDict.TryGetValue(id, out var data) ? data : null;
    }

    public TreeData GetTreeData(int id)
    {
        return treeDataDict.TryGetValue(id, out var data) ? data : null;
    }

    public RabbitData GetRabbitData(RabbitActionStatus actionStatus)
    {
        return rabbitDataDict.TryGetValue(actionStatus, out var data) ? data : null;
    }

    public WolfData GetWolfData(WolfActionStatus actionStatus)
    {
        return wolfDataDict.TryGetValue(actionStatus, out var data) ? data : null;
    }

    public PoopData GetPoopData(int id)
    {
        return poopDataDict.TryGetValue(id, out var data) ? data : null;
    }

    public SeasonData GetSeasonData(string id)
    {
        return seasonDataDict.TryGetValue(id, out var data) ? data : null;
    }
}
