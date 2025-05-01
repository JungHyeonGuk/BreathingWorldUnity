using UnityEngine;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public bool isGetWeed;
    public bool isGetTree;
    public bool isGetRabbit;
    public bool isGetWolf;

    [SerializeField] int testDistrictId;
    [SerializeField] WebSocketManager webSocketManager;
    [SerializeField] MapManager mapManager;



    [ContextMenu("TestSendMessage")]
    async void TestSendMessage() 
    {
        await webSocketManager.SendChatMessage($"Player{Random.Range(1, 1000)}", "Hello World!");
    }

    [ContextMenu("TestSetMapTile")]
    void TestSetMapTile() 
    {
        mapManager.SetMapTile(null, new Vector3(768, 280, 0));
    }

    [ContextMenu("TestInvokeGetOneByOneByDistrictId")]
    async void TestInvokeGetOneByOneByDistrictId() 
    {
        await webSocketManager.GetOneByOneByDistrictId(testDistrictId);
    }


    void Update()
    {
        LogClickPosInfo();
    }

    void LogClickPosInfo()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector2 worldIntPos = new Vector2(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
            List<string> districtIds = mapManager.GetDistrictIdByPoint(worldIntPos);

            Debug.Log($"World Position: {worldIntPos}, DistrictId: {districtIds[0]}");
        }
    }
}
