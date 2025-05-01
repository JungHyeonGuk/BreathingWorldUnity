using UnityEngine;

public class NatureWolf : MonoBehaviour 
{
    public Wolf wolfInfo;



    void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }
}