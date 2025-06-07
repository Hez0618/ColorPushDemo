using UnityEngine;
using UnityEngine.EventSystems;

public class HoverRotateAndScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Icon Rotation Settings")]
    public Transform iconTransform;
    public float rotationSpeed = 180f;
    public float rotationReturnDuration = 1f;

    [Header("Button Scale Settings")]
    public Transform scaleTarget;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public float scaleSpeed = 8f;

    private Vector3 originalScale;
    private bool isHovering = false;

    private bool isReturning = false;
    private Quaternion startRotation;
    private float returnTimer = 0f;

    void Start()
    {
        if (scaleTarget == null)
            scaleTarget = this.transform;

        originalScale = scaleTarget.localScale;
    }

    void Update()
    {

        Vector3 targetScale = isHovering ? hoverScale : originalScale;
        scaleTarget.localScale = Vector3.Lerp(scaleTarget.localScale, targetScale, Time.deltaTime * scaleSpeed);


        if (isHovering && iconTransform != null)
        {
            iconTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
        else if (isReturning && iconTransform != null)
        {
            returnTimer += Time.deltaTime;
            float t = Mathf.Clamp01(returnTimer / rotationReturnDuration);
            iconTransform.rotation = Quaternion.Lerp(startRotation, Quaternion.identity, t);

            if (t >= 1f)
                isReturning = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        isReturning = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (iconTransform != null)
        {
            startRotation = iconTransform.rotation;
            returnTimer = 0f;
            isReturning = true;
        }
    }
}


