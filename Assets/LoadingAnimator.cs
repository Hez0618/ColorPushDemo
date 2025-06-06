using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimator : MonoBehaviour
{
    [Header("Dot Images")]
    public Image[] dots;

    [Header("Animation Settings")]
    public float speed = 2f;
    public Color baseColor = new Color(0.8f, 0.8f, 0.8f);
    public Color highlightColor = new Color(0.2f, 0.2f, 0.2f);

    private float angle = 0f;

    void Update()
    {
        if (dots == null || dots.Length == 0)
            return;

        angle += Time.deltaTime * speed;

        for (int i = 0; i < dots.Length; i++)
        {

            float phase = (i / (float)dots.Length) * Mathf.PI * 2f;


            float wave = Mathf.Cos(phase - angle);


            float t = Mathf.InverseLerp(-1f, 1f, wave);

            dots[i].color = Color.Lerp(baseColor, highlightColor, t);
        }
    }
}


