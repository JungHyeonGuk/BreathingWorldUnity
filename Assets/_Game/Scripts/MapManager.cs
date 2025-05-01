using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer mapBackground;
    [SerializeField] SpriteRenderer mapPixel;
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tilemap backTilemap;
    [SerializeField] DB db;
    [SerializeField] CamManager camManager;
    [SerializeField] GameObject mapGrid;
    [SerializeField] WebSocketManager webSocketManager;

    bool isLastZoomOut = true;



    public void SetMapImage(Texture2D mapTexture)
    {
        mapPixel.sprite = Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f), 1f);
    }

    public void SetMapTile(TileBase tileBase, Vector3 tilePos)
    {
        tilemap.SetTile(new Vector3Int((int)tilePos.x, (int)tilePos.y, 0), tileBase);
    }

    public void SetBackTile(TileBase tileBase, Vector3 tilePos)
    {
        backTilemap.SetTile(new Vector3Int((int)tilePos.x, (int)tilePos.y, 0), tileBase);
    }

    public List<string> GetDistrictIdByPoint(Vector2 point) 
    {
        return GetDistrictIdsByBound(point, point);
    }

    public List<string> GetDistrictIdsByBound(Vector2 leftBottom, Vector2 rightTop)
    {
        const int DISTRICT_WIDTH = 48;
        const int DISTRICT_HEIGHT = 27;
        const int TOTAL_DISTRICTS_X = 40;
        
        List<string> districtIds = new List<string>();
        
        int startX = Mathf.Max(0, Mathf.FloorToInt(leftBottom.x / DISTRICT_WIDTH));
        int endX = Mathf.Min(TOTAL_DISTRICTS_X - 1, Mathf.FloorToInt(rightTop.x / DISTRICT_WIDTH));

        int startY = Mathf.Max(0, Mathf.FloorToInt((1080 - rightTop.y) / DISTRICT_HEIGHT));
        int endY = Mathf.Min(TOTAL_DISTRICTS_X - 1, Mathf.FloorToInt((1080 - leftBottom.y) / DISTRICT_HEIGHT));
 
        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                int districtId = y * TOTAL_DISTRICTS_X + x;
                districtIds.Add(districtId.ToString());
            }
        }
        
        return districtIds;
    }

    public Vector3 GetTileToWorldPos(int tileX, int tileY) 
    {
        return new Vector3(tileX, 1079 - tileY, 0);
    }

    public Vector3 GetTileToWorldPosAnimal(string currentPositionString) 
    {
        string[] currentPositionSplits = currentPositionString.Split(':');
        int tileX = (int)(int.Parse(currentPositionSplits[0]) / 16f);
        int tileY = (int)(int.Parse(currentPositionSplits[1]) / 16f);
        return new Vector3(tileX, 1079 - tileY, -1);
    }

    [ContextMenu("ClearTilemaps")]
    public void ClearTilemaps() 
    {
        tilemap.ClearAllTiles();
        backTilemap.ClearAllTiles();
    }

    public void SetSeasonColor(string seasonId) 
    {
        SeasonData seasonData = db.GetSeasonData($"map{seasonId}");
        mapBackground.color = seasonData.color;
    }

    public void ShowMapPixel(bool isShow) 
    {
        mapPixel.gameObject.SetActive(isShow);
        Vector3 pos = new Vector3(959.5f, 539.5f, isShow ? -10 : 10);
        mapBackground.transform.position = pos;
        mapPixel.transform.position = pos;
        mapGrid.SetActive(!isShow);
    }


    void Start()
    {
        camManager.camChanged += OnCamChanged;
    }

    void OnDestroy()
    {
        camManager.camChanged -= OnCamChanged;
    }

    async void OnCamChanged(bool isZoomed, bool isPanning, int zoomLevel, Vector2 leftBottom, Vector2 rightTop)
    {
        bool isZoomOut = zoomLevel < 3;


        if (isZoomed) 
        {
            if (isZoomOut) 
            {
                ShowMapPixel(true);
            }
            else 
            {
                ShowMapPixel(false);
            }
        }

        if (isLastZoomOut != isZoomOut) 
        {
            isLastZoomOut = isZoomOut;

            if (isZoomOut) 
            {
                await webSocketManager.UnjoinMapGroup();
            }
        }

        if (!isZoomOut) 
        {
            await webSocketManager.UnjoinMapGroup();
            List<string> districtIds = GetDistrictIdsByBound(leftBottom, rightTop);
            List<int> districtIdsInt = districtIds.ConvertAll(x => int.Parse(x));
            await webSocketManager.JoinMapGroup(districtIds);

            foreach (int districtIdInt in districtIdsInt) 
            {
                if (!webSocketManager.IsConnected) break;

                await webSocketManager.GetOneByOneByDistrictId(districtIdInt);
            }
        }
    }
    
}
