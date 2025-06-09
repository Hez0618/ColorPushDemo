using UnityEngine;

public class Splash : MonoBehaviour
{
    private Color currentColor;
    void Start()
    {
        currentColor = GetComponent<SpriteRenderer>().color;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.Play("Splash");
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.splashContactCount++;
                player.SetColorSmoothly(currentColor);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.splashContactCount--;
            }
        }
    }
}

