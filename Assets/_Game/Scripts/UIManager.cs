using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject plantAcceleratedIcon;
    [SerializeField] Text connectedUserText;



    public void SetPlantAccelerated(bool isAccelerated) 
    {
        plantAcceleratedIcon.SetActive(isAccelerated);
    }

    public void SetConnectedUserCount(int count) 
    {
        connectedUserText.text = $"{count}";
    }
}
