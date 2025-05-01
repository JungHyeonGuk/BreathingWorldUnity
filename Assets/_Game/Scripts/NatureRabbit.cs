using UnityEngine;

public class NatureRabbit : MonoBehaviour 
{
    [SerializeField] DB db;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float maxTime;
    [SerializeField] ulong id;
    [SerializeField] string positionString;


    public Rabbit rabbitInfo;

    Sprite[] sprites;
    float curTime;
    int spriteIndex;



    public void Renew(Rabbit rabbitInfo, Vector3 worldPos) 
    {
        this.rabbitInfo = rabbitInfo;
        transform.position = worldPos;

        RabbitData rabbitData = db.GetRabbitData(rabbitInfo.ActionStatus);
        sprites = rabbitData.sprites;

        id = rabbitInfo.Id;
        positionString = rabbitInfo.CurrentPositionString;
    }


    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime >= maxTime) 
        {
            curTime = 0;

            if (sprites == null || sprites.Length == 0) return;
            if (spriteIndex >= sprites.Length)  
            {
                spriteIndex = 0;
            }

            spriteRenderer.sprite = sprites[spriteIndex];
            spriteIndex++;
        }
    }

    void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }
}
