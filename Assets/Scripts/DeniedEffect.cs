using UnityEngine;
using System.Collections;

public class DeniedEffect : MonoBehaviour
{
    public float floatDistance = 0.5f;
    public float showDuration = 0.5f;
    public float fadeDuration = 0.3f;

    private Vector3 startPos;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.localPosition;

        var color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;
    }

    public void Play()
    {
        StopAllCoroutines();
        transform.localPosition = startPos;

        var color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;

        StartCoroutine(PlayEffect());
    }

    private IEnumerator PlayEffect()
    {
        // 淡入 & 上浮
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float percent = t / fadeDuration;

            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(0f, 1f, percent);
            spriteRenderer.color = color;

            transform.localPosition = startPos + Vector3.up * (floatDistance * percent);
            yield return null;
        }

        yield return new WaitForSeconds(showDuration);

        // 淡出 & 继续浮动
        t = 0f;
        Vector3 currentPos = transform.localPosition;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float percent = t / fadeDuration;

            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(1f, 0f, percent);
            spriteRenderer.color = color;

            transform.localPosition = currentPos + Vector3.up * (floatDistance * percent);
            yield return null;
        }

        // 完全透明
        Color finalColor = spriteRenderer.color;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;
    }
}
