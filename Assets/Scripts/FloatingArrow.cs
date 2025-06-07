using UnityEngine;

/// <summary>
/// This script makes an arrow float up and down smoothly,
/// </summary>
public class FloatingArrow : MonoBehaviour
{
    public float floatSpeed = 3f;
    public float floatHeight = 0.1f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPos + new Vector3(0, offset, 0);
    }
}

