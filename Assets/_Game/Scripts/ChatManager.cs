using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [SerializeField] GameObject chatOpenBtn;
    [SerializeField] GameObject chatView;
    [SerializeField] ScrollRect chatScrollRect;
    [SerializeField] InputField nameInput;
    [SerializeField] InputField messageInput;
    [SerializeField] Text chatText;
    [SerializeField] RectTransform[] chatRects;
    [SerializeField] WebSocketManager webSocketManager;



    public async void SendToChat() 
    {
        if (string.IsNullOrEmpty(nameInput.text) || string.IsNullOrEmpty(messageInput.text)) return;

        await webSocketManager.SendChatMessage(nameInput.text, messageInput.text);
        messageInput.text = string.Empty;
    }

    public void ReceiveChatMessage(string user, string message) 
    {
        string sendMsg = $"{user}: {message}";
        if (chatText.text.Length > 0)
        {
            chatText.text += $"\n{sendMsg}";
        }
        else
        {
            chatText.text = sendMsg;
        }

        foreach (var rect in chatRects)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
        chatScrollRect.verticalNormalizedPosition = 0f;
    }

    public void OpenChat()
    {
        messageInput.text = string.Empty;
        chatOpenBtn.SetActive(false);
        chatView.SetActive(true);
    }

    public void CloseChat()
    {
        chatOpenBtn.SetActive(true);
        chatView.SetActive(false);
    }
}
