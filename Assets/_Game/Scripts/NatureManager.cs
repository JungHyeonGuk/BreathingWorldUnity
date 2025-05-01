using UnityEngine;
using System.Collections.Generic;
using MessagePack;
using System;

public class NatureManager : MonoBehaviour
{
    [SerializeField] CamManager camManager;
    [SerializeField] MapManager mapManager;
    [SerializeField] DB db;



    public void HandleReceiveOneWeedInfo(int districtId, string tileId, int weedProceedCode, bool rabbitFeceExists, bool wolfFeceExists) 
    {
        string[] tileSplits = tileId.Split(':');
        Vector3 worldPos = mapManager.GetTileToWorldPos(int.Parse(tileSplits[0]), int.Parse(tileSplits[1]));
        WeedData weedData = db.GetWeedData(weedProceedCode);

        if (weedData != null) 
        {
            mapManager.SetMapTile(weedData.tileBase, worldPos);
        }
        else 
        {
            mapManager.SetMapTile(null, worldPos);
        }

        if (rabbitFeceExists) 
        {
            PoopData poopData = GetPoopDataByKind("rabbit");
            mapManager.SetBackTile(poopData.tileBase, worldPos);
        }
        else if (wolfFeceExists) 
        {
            PoopData poopData = GetPoopDataByKind("wolf");
            mapManager.SetBackTile(poopData.tileBase, worldPos);
        }
        else 
        {
            mapManager.SetBackTile(null, worldPos);
        }
    }

    public void HandleReceiveWeedInfoByDistrictId(int districtId, byte[] weedsBytes)  
    {
        try 
        {
            var infoDecoded = MessagePackSerializer.Deserialize<Dictionary<string, ResponseWeedInfo>>(weedsBytes);
            foreach (var (tileId, info) in infoDecoded)
            {
                HandleReceiveOneWeedInfo(districtId, tileId, info.WeedValue, info.RabbitFecesExists, info.WolfFecesExists);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to handle receive weed info by district id: {ex.Message}");
        }
    }

    public void HandleReceiveTreeInfoByDistrictId(int districtId, byte[] treesBytes)  
    {
        try 
        {
            if (treesBytes.Length <= 2) return;
            ResponseTreeInfo treeInfo = MessagePackSerializer.Deserialize<ResponseTreeInfo>(treesBytes);

            string[] centerTileSplits = treeInfo.CenterTileString.Split(':');
            Vector3 worldPos = mapManager.GetTileToWorldPos(int.Parse(centerTileSplits[0]), int.Parse(centerTileSplits[1]));
            worldPos.z = -5;

            int treeDBid = -1;
            switch (treeInfo.ProceedCode) 
            {
                case TreeProceedCode.germination: treeDBid = 0; break;
                case TreeProceedCode.growth1: treeDBid = 1; break;
                case TreeProceedCode.growth2: treeDBid = 2; break;
                case TreeProceedCode.growth3: treeDBid = 3; break;
                case TreeProceedCode.active1: treeDBid = 4; break;
                case TreeProceedCode.active2: treeDBid = 5; break;
                case TreeProceedCode.active3: treeDBid = 6; break;
                case TreeProceedCode.active4: treeDBid = 7; break;
                case TreeProceedCode.active5: treeDBid = 7; break;
                case TreeProceedCode.wither1: treeDBid = 8; break;
                case TreeProceedCode.wither2: treeDBid = 9; break;
                case TreeProceedCode.dead: treeDBid = 10; break;
                case TreeProceedCode.none: treeDBid = -1; break;
            }

            TreeData treeData = db.GetTreeData(treeDBid);
            Nature existObject = ObjectPooler.GetAllPools<Nature>("Nature").Find(x => x.transform.position == worldPos);

            if (treeData != null) 
            {
                if (existObject == null) 
                {
                    existObject = ObjectPooler.SpawnFromPool<Nature>("Nature", worldPos);
                    existObject.transform.localScale = Vector3.one * 2.5f;
                }
                existObject.Set(treeData.sprite);
            }
            else if (existObject != null) 
            {
                existObject.gameObject.SetActive(false);
            }
        }
        catch 
        {
            //Debug.LogError($"Failed to handle receive tree info by district id: {ex.Message}");
        }
    }

    public void HandleReceiveAddedFecesByDistrict(int districtId, string tileId, string kind) 
    {
        string[] tileSplits = tileId.Split(':');
        Vector3 worldPos = mapManager.GetTileToWorldPos(int.Parse(tileSplits[0]), int.Parse(tileSplits[1]));

        PoopData poopData = GetPoopDataByKind(kind);
        if (poopData != null) 
        {
            mapManager.SetBackTile(poopData.tileBase, worldPos);
        }
        else 
        {
            mapManager.SetBackTile(null, worldPos);
        }
    }

    public void HandleReceiveRemovedFecesByDistrict(int districtId, string tileId, string kind) 
    {
        string[] tileSplits = tileId.Split(':');
        Vector3 worldPos = mapManager.GetTileToWorldPos(int.Parse(tileSplits[0]), int.Parse(tileSplits[1]));
        mapManager.SetBackTile(null, worldPos);
    }

    public void HandleReceiveOneRabbitInfoByDistrict(ulong rabbitId, byte[] rabbitBytes) 
    {
        try 
        {
            if (rabbitBytes.Length <= 2) return;
            Debug.Log($"rabbitId: {rabbitId}");
            var rabbitInfo = MessagePackSerializer.Deserialize<Rabbit>(rabbitBytes);

            NatureRabbit existNatureRabbit = ObjectPooler.GetAllPools<NatureRabbit>("NatureRabbit")
                .Find(x => x.rabbitInfo != null && x.rabbitInfo.Id == rabbitId);
            Vector3 worldPos = mapManager.GetTileToWorldPosAnimal(rabbitInfo.CurrentPositionString);

            if (existNatureRabbit == null) 
            {
                existNatureRabbit = ObjectPooler.SpawnFromPool<NatureRabbit>("NatureRabbit", worldPos);
            }

            existNatureRabbit.Renew(rabbitInfo, worldPos);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to handle receive one rabbit info by district: {ex.Message}");
        }
    }

    public void HandleReceiveOneWolfInfoByDistrict(ulong wolfId, byte[] wolfBytes) 
    {
        try 
        {
            if (wolfBytes.Length <= 2) return;
            var wolfInfo = MessagePackSerializer.Deserialize<Wolf>(wolfBytes);

        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to handle receive one wolf info by district: {ex.Message}");
        }
    }

    PoopData GetPoopDataByKind(string kind) 
    {
        switch (kind) 
        {
            case "rabbit": return db.GetPoopData(0);
            case "wolf": return db.GetPoopData(1);
            default: return null;
        }
    }

    void Start()
    {
        camManager.camChanged += OnCamChanged;
    }

    void OnDestroy()
    {
        camManager.camChanged -= OnCamChanged;
    }

    void OnCamChanged(bool isZoomed, bool isPanning, int zoomLevel, Vector2 leftBottom, Vector2 rightTop)
    {

    }

}