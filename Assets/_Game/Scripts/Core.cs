using UnityEngine;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

public class Core : MonoBehaviour 
{
    [SerializeField] SettingsInfo settingsInfo;
    [SerializeField] MapManager mapManager;
    [SerializeField] WebSocketManager webSocketManager;
    [SerializeField] DB db;

    const string API_URL = "https://api.breathingworld.com";



    async void Start()
    {
        db.Caching();
        mapManager.ClearTilemaps();

        await GetSettings();
        await GetMapImageUpdateId();
        await webSocketManager.StartFromCore();
    }

    public async Awaitable GetSettings()
    {
        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"{API_URL}/settings/base"))
            {
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Awaitable.NextFrameAsync();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception("Network response was not ok");
                }

                string jsonResponse = request.downloadHandler.text;

                settingsInfo = JsonConvert.DeserializeObject<SettingsInfo>(jsonResponse);
                Variables.Settings = settingsInfo;
                Variables.MapInfo.MapMinWidth = settingsInfo.mapMinWidth;
                Variables.MapInfo.MapMinHeight = settingsInfo.mapMinHeight;
            }
        }
        catch (Exception error)
        {
            Debug.LogError($"Error: {error.Message}");
        }
    }

    public void SetMapImageUpdateId(string newMapImageUpdateId) 
    {
        settingsInfo.mapImageUpdateId = newMapImageUpdateId;
        _ = GetMapImageUpdateId();
    }

    public async Awaitable GetMapImageUpdateId() 
    {
        string url = $"{API_URL}/maps/{settingsInfo.mapId}/live/{settingsInfo.mapImageUpdateId}";

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Awaitable.NextFrameAsync();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                return;
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            texture.filterMode = FilterMode.Point;

            mapManager.SetMapImage(texture);
            mapManager.ShowMapPixel(true);
        }
    }

    public async Awaitable InvokeAllJoinGroup(bool isActive, Vector2 leftBottom, Vector2 rightTop) 
    {
        if (isActive) 
        {
            List<string> joinDistrictIds = mapManager.GetDistrictIdsByBound(leftBottom, rightTop);
            await webSocketManager.JoinMapGroup(joinDistrictIds);
            foreach (string districtId in joinDistrictIds) 
            {
                if (!webSocketManager.IsConnected) break;

                int districtIdInt = int.Parse(districtId);
                await webSocketManager.GetOneByOneByDistrictId(districtIdInt);
            }
        }
        else 
        {
            await webSocketManager.UnjoinMapGroup();
        }
    }

}