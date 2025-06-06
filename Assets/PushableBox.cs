using UnityEngine;
using System.Collections;

public class PushableBox : MonoBehaviour
{
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    public float moveSpeed = 5f;
    public LayerMask obstacleLayers; 

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isMoving = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.position = new Vector2(Mathf.Round(rb.position.x), Mathf.Round(rb.position.y));

        originalColor = spriteRenderer.color;
        UpdateColor();
    }

    public bool TryMove(Vector2 direction, Color playerColor)
    {
        if (isMoving) return false;

        if (playerColor != originalColor)
        {
            Debug.Log("Color Not Match!");
            return false;  
        }

        Vector2 size = boxCollider.size * 0.9f;

        Vector2 targetPos = rb.position + direction;
        Collider2D hit_test = Physics2D.OverlapBox(targetPos, size, 0f, obstacleLayers);
        if (hit_test != null)
        {
            print(hit_test.gameObject.name);
            return false;
        }


        MonoBehaviour runner = Object.FindAnyObjectByType<PlayerController>();
        if (runner != null)
        {
            runner.StartCoroutine(MoveCoroutine(direction));
            return true;
        }

        return false;
    }


    private IEnumerator MoveCoroutine(Vector2 direction)
    {
        isMoving = true;

        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + direction;
        float duration = 1f / moveSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPos);
        isMoving = false;
    }


    public void UpdateColor()
    {
        originalColor = spriteRenderer.color;
    }

    public void SetHighlight(bool highlight)
    {
        if (highlight)
        {
            spriteRenderer.color = Color.red; 
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }


}





