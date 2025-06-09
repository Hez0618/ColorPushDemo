using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScaleOnly : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Settings")]
    public Transform target; 
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public float scaleSpeed = 8f;

    private Vector3 originalScale;
    private bool isHovering = false;

    void Start()
    {
        if (target == null)
            target = this.transform;

        originalScale = target.localScale;
    }

    void Update()
    {
        Vector3 targetScale = isHovering ? hoverScale : originalScale;
        target.localScale = Vector3.Lerp(target.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.Play("Hover");
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}

