using UnityEngine;

public class Nature : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;



    public void Set(Sprite sprite) 
    {
        spriteRenderer.enabled = sprite != null;
        spriteRenderer.sprite = sprite;
    }


    void OnDisable()
    {
        try 
        {
            ObjectPooler.ReturnToPool(gameObject);
        }
        catch
        {
        }
    }
}
