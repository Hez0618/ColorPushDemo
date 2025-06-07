using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class CameraFitToTilemap : MonoBehaviour
{
    public Tilemap tilemap;

    private Camera cam;

    [Range(0.5f, 1f)]
    public float zoomFactor = 0.5f;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (tilemap == null)
        {
            return;
        }

        Bounds bounds = tilemap.localBounds;
        Vector3 center = bounds.center;
        float mapWidth = bounds.size.x;
        float mapHeight = bounds.size.y;

        cam.transform.position = new Vector3(center.x, center.y, cam.transform.position.z);

        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = mapWidth / mapHeight;

        if (screenRatio >= targetRatio)
        {
            cam.orthographicSize = (mapHeight / 2f) * zoomFactor;
        }
        else
        {
            cam.orthographicSize = ((mapWidth / screenRatio) / 2f) * zoomFactor;
        }
    }
}



