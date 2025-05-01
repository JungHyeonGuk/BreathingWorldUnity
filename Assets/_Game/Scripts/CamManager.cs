using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamManager : MonoBehaviour
{
    public delegate void CamChangeEvent(bool isZoomed, bool isPanning, int zoomLevel, Vector2 leftBottom, Vector2 rightTop);
    public event CamChangeEvent camChanged;

    [SerializeField] Camera cam;
    [SerializeField] SpriteRenderer mapBound;
    [SerializeField] float zoomTimeThreshold;
 
    Vector3 lastMousePosition;
    Vector3 lastPosition;
    bool isDragging;
    float[] zoomLevels = new float[] { 540f, 270f, 135f, 67.5f, 33.75f, 16.875f, 8.4375f, 4.21875f };
    int currentZoomIndex;
    float lastZoomTime;
    bool needsZoomLogging;



    void Update()
    {
        if (IsMouseOverUI()) return;

        bool isZoomed = HandleZoom();
        bool isPanning = HandlePanning();

        ClampCamera();
        
        if (isPanning)
        {
            NotifyCameraChanged(isZoomed, isPanning);
        }
    }

    bool HandleZoom()
    {
        float scrollInput = Input.mouseScrollDelta.y;
        
        if (scrollInput != 0)
        {
            Vector3 mouseWorldPositionBefore = cam.ScreenToWorldPoint(Input.mousePosition);

            if (scrollInput > 0 && currentZoomIndex < zoomLevels.Length - 1)
            {
                currentZoomIndex++;
                cam.orthographicSize = zoomLevels[currentZoomIndex];
                lastZoomTime = Time.time;
                needsZoomLogging = true;
            }
            else if (scrollInput < 0 && currentZoomIndex > 0)
            {
                currentZoomIndex--;
                cam.orthographicSize = zoomLevels[currentZoomIndex];
                lastZoomTime = Time.time;
                needsZoomLogging = true;
            }
            else
            {
                return false;
            }

            Vector3 mouseWorldPositionAfter = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 adjustment = mouseWorldPositionBefore - mouseWorldPositionAfter;
            transform.position += adjustment;

            return true;
        }

        if (needsZoomLogging && Time.time - lastZoomTime >= zoomTimeThreshold)
        {
            NotifyCameraChanged(true, false);
            needsZoomLogging = false;
        }

        return false;
    }

    bool HandlePanning()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            lastPosition = transform.position;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            if (lastPosition != transform.position) 
            {
                return true;
            }
        }

        if (isDragging)
        {
            Vector3 currentMousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = lastMousePosition - currentMousePosition;
            transform.position += difference;
            lastMousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        return false;
    }

    void ClampCamera()
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        Vector3 pos = transform.position;
        
        float minX = mapBound.bounds.min.x + camWidth;
        float maxX = mapBound.bounds.max.x - camWidth;
        float minY = mapBound.bounds.min.y + camHeight;
        float maxY = mapBound.bounds.max.y - camHeight;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        
        transform.position = pos;
    }

    void NotifyCameraChanged(bool isZoomed, bool isPanning)
    {
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
        Vector2 bottomLeft2 = new Vector2(Mathf.Floor(bottomLeft.x), Mathf.Floor(bottomLeft.y));
        Vector2 topRight2 = new Vector2(Mathf.Ceil(topRight.x), Mathf.Ceil(topRight.y));
        
        camChanged?.Invoke(isZoomed, isPanning, currentZoomIndex, bottomLeft2, topRight2);
    }

    bool IsMouseOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        else
        {
            var eventData = new PointerEventData(EventSystem.current) {position = Input.mousePosition };
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, hits);
            return hits.Count > 0;
        }
    }
}
