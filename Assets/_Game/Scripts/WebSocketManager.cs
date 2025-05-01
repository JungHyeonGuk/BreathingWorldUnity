using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;

public class WebSocketManager : MonoBehaviour
{
    [SerializeField] Core core;
    [SerializeField] Test test;
    [SerializeField] NatureManager natureManager;
    [SerializeField] ChatManager chatManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] MapManager mapManager;
  
    SynchronizationContext context;
    HubConnection mainConnection;
    HubConnection chatConnection;
    string socketUrl = "https://api.breathingworld.com/nodeHub";
    string chatUrl = "https://chat.breathingworld.com/chatHub";
    byte[] ulongBytes;


    public bool IsConnected 
    { 
        get
        {
            return mainConnection != null && mainConnection.State == HubConnectionState.Connected;
        }
    }


    public async Awaitable StartFromCore()
    {
        await OnDestroy();
        await InitializeConnections();
        SetupEventHandlers();
    }

    public async Awaitable InitializeConnections()
    {
        context = SynchronizationContext.Current;
        ulongBytes = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        mainConnection = new HubConnectionBuilder()
            .WithUrl(socketUrl)
            // .ConfigureLogging(logging => 
            // {
            //     logging.SetMinimumLevel(LogLevel.Debug);
            //     logging.AddProvider(new UnityDebugLoggerProvider());
            // })
            .Build();
        chatConnection = new HubConnectionBuilder()
            .WithUrl(chatUrl)
            .Build();
            
        try 
        {
            await mainConnection.StartAsync();
            await chatConnection.StartAsync();

            await mainConnection.InvokeAsync("SendMessage", "Welcome", "to Breathing World!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to connect: {ex.Message}");
        }
    }

    public async Awaitable SendChatMessage(string user, string message)
    {
        try
        {
            await chatConnection.InvokeAsync("SendMessage", user, message);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to send message: {ex.Message}");
        }
    }

    public async Awaitable JoinMapGroup(List<string> mapIds)
    {
        try
        {
            await mainConnection.InvokeAsync("JoinMapGroup", mapIds);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to join map group: {ex.Message}");
        }
    }

    public async Awaitable UnjoinMapGroup()
    {
        try
        {
            await mainConnection.InvokeAsync("UnjoinMapGroup");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to unjoin map group: {ex.Message}");
        }
    }

    public async Awaitable GetOneByOneByDistrictId(int districtId) 
    {
        try 
        {
            if (test.isGetWeed)
            {
                await mainConnection.InvokeAsync("GetWeedInfoByDistrictId", districtId);
            }
            if (test.isGetTree)
            {
                await mainConnection.InvokeAsync("GetTreeInfoByDistrictId", districtId);
            }
            if (test.isGetRabbit)
            {
                await mainConnection.InvokeAsync("GetRabbitInfoByDistrictId", districtId);
            }
            if (test.isGetWolf)
            {
                await mainConnection.InvokeAsync("GetWolfInfoByDistrictId", districtId);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to get one by one by district id: {ex.Message}");
        }
    }


    void SetupEventHandlers()
    {
        if (!Application.isPlaying) 
        {
            return;
        }

        chatConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            context.Post(o => 
            {
                chatManager.ReceiveChatMessage(user, message);
            }, null);
        });

        chatConnection.On<int>("ReceiveConnectedUserCount", (count) =>
        {
            context.Post(o => 
            {
                uiManager.SetConnectedUserCount(count);
            }, null);
        });



        mainConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            Debug.Log($"Main ReceiveMessage {user}: {message}");
        });

        mainConnection.On("WebsocketDisconnected", () => 
        {
            Debug.Log("Websocket Disconnected");
        });

        mainConnection.On<string>("ReceiveMapImageUpdateId", (mapImageUpdateId) =>
        {
            context.Post(o => 
            {
                core.SetMapImageUpdateId(mapImageUpdateId);
            }, null);
        });

        mainConnection.On<string>("ReceiveSeasonIdUpdated", (seasonId) =>
        {
            context.Post(o => 
            {
                Debug.Log($"Season Image ID: {seasonId}");
                mapManager.SetSeasonColor(seasonId);
            }, null);
        });

        mainConnection.On<bool>("ReceivePlantProceedAccelerated", (plantProceedAccelerated) =>
        {
            context.Post(o => 
            {
                uiManager.SetPlantAccelerated(plantProceedAccelerated);
            }, null);
        });

        mainConnection.On<int, string, int, bool, bool>("ReceiveOneWeedInfo", (districtId, tileId, weedProceedCode, rabbitFeceExists, wolfFeceExists) =>
        {
            context.Post(o => 
            {
                natureManager.HandleReceiveOneWeedInfo(districtId, tileId, weedProceedCode, rabbitFeceExists, wolfFeceExists);
            }, null);
        });

        mainConnection.On<int, byte[]>("ReceiveWeedInfoByDistrictId", (districtId, weedsBytes) => 
        {
            context.Post(o => 
            {
                natureManager.HandleReceiveWeedInfoByDistrictId(districtId, weedsBytes);
            }, null);
        });

        mainConnection.On<ulong, byte[]>("ReceiveOneRabbitInfoByDistrict", (rabbitId, rabbitBytes) => 
        {
            context.Post(o => 
            {
                natureManager.HandleReceiveOneRabbitInfoByDistrict(rabbitId, rabbitBytes);
            }, null);
        });

        mainConnection.On<int, object>("ReceiveRabbitInfoByDistrictId", (districtId, args) => 
        {
            // try 
            // {
            //     string jsonArray = args.ToString();
            //     string[] base64Strings = JsonConvert.DeserializeObject<string[]>(jsonArray);
                
            //     if (base64Strings != null && base64Strings.Length > 0)
            //     {
            //         foreach (string base64String in base64Strings)
            //         {
            //             byte[] currentBytes = Convert.FromBase64String(base64String);
            //             if (currentBytes.Length > 0)
            //             {
            //                 context.Post(o => 
            //                 {
            //                     ulong rabbitId = BitConverter.ToUInt64(ulongBytes, 0);
            //                     natureManager.HandleReceiveOneRabbitInfoByDistrict(rabbitId, currentBytes);
            //                 }, null);
            //             }
            //         }
            //     }
            // }
            // catch (Exception ex)
            // {
            //     Debug.LogError($"Error processing rabbit data: {ex.Message}\nStack trace: {ex.StackTrace}");
            // }
        });

        mainConnection.On<ulong, byte[]>("ReceiveOneWolfInfoByDistrict", (wolfId, wolfBytes) => 
        {
            context.Post(o => 
            {
                natureManager.HandleReceiveOneWolfInfoByDistrict(wolfId, wolfBytes);
            }, null);
        });

        mainConnection.On<int, object>("ReceiveWolfInfoByDistrictId", (districtId, args) => 
        {
            try 
            {
                string jsonArray = args.ToString();
                string[] base64Strings = JsonConvert.DeserializeObject<string[]>(jsonArray);
                if (base64Strings != null && base64Strings.Length > 0)  
                {
                    foreach (string base64String in base64Strings)
                    {
                        byte[] currentBytes = Convert.FromBase64String(base64String);
                        if (currentBytes.Length > 0)
                        {
                            context.Post(o => 
                            {
                                ulong wolfId = BitConverter.ToUInt64(ulongBytes, 0);
                                natureManager.HandleReceiveOneWolfInfoByDistrict(wolfId, currentBytes);
                            }, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error processing wolf data: {ex.Message}\nStack trace: {ex.StackTrace}");
            }
        });

        mainConnection.On<int, object>("ReceiveTreeInfoByDistrictId", (districtId, args) =>
        {
            try 
            {
                string jsonArray = args.ToString();
                string[] base64Strings = JsonConvert.DeserializeObject<string[]>(jsonArray);
                if (base64Strings != null && base64Strings.Length > 0)
                {
                    foreach (string base64String in base64Strings)
                    {
                        byte[] currentBytes = Convert.FromBase64String(base64String);
                        if (currentBytes.Length > 0)
                        {
                            context.Post(o => 
                            {
                                natureManager.HandleReceiveTreeInfoByDistrictId(districtId, currentBytes);
                            }, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error processing tree data: {ex.Message}\nStack trace: {ex.StackTrace}");
            }
        });

        mainConnection.On<ulong, byte[]>("ReceiveOneTreeInfoByDistrict", (treeId, treesBytes) => 
        {
            context.Post(o => 
            {
                natureManager.HandleReceiveTreeInfoByDistrictId(0, treesBytes);
            }, null);
        });

        mainConnection.On<int, string, string>("ReceiveAddedFecesByDistrict", (districtId, tileId, kind) => 
        {
            context.Post(o => 
            {
                natureManager.HandleReceiveAddedFecesByDistrict(districtId, tileId, kind);
            }, null);
        });

        mainConnection.On<int, string, string>("ReceiveRemovedFecesByDistrict", (districtId, tileId, kind) => 
        {
            context.Post(o => 
            {
                natureManager.HandleReceiveRemovedFecesByDistrict(districtId, tileId, kind);
            }, null);
        });
    }


    async Awaitable OnDestroy()
    {
        if (mainConnection != null)
        {
            await mainConnection.StopAsync();
            await mainConnection.DisposeAsync();
        }
        
        if (chatConnection != null)
        {
            await chatConnection.StopAsync();
            await chatConnection.DisposeAsync();
        }
    }
}