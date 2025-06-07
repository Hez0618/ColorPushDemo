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

        // When Color is not matched, play shake animation
        if (!ColorUtils.IsSimilar(playerColor, originalColor))
        {
            print(playerColor);
            print(originalColor);
            Debug.Log("Color Not Match!");
            StartCoroutine(ShakeBox(direction));
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
            runner.StartCoroutine(MoveCoroutine(direction, () =>
            {
                CheckGoalAfterMove();
            }));
            return true;
        }

        return false;
    }

    private void CheckGoalAfterMove()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (var hit in hits)
        {
            GoalTrigger goal = hit.GetComponent<GoalTrigger>();
            if (goal != null)
            {
                goal.OnBoxArrived(this);
                break;
            }
        }
    }


    IEnumerator ShakeBox(Vector2 direction)
    {
        Vector3 originalPos = transform.localPosition;
        float duration = 0.2f;
        float strength = 0.05f;
        float elapsed = 0f;

        Vector3 shakeDir = new Vector3(direction.x, direction.y, 0f).normalized;

        while (elapsed < duration)
        {
            float offset = Mathf.Sin(elapsed * 40f) * strength;
            transform.localPosition = originalPos + shakeDir * offset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }


    private IEnumerator MoveCoroutine(Vector2 direction, System.Action onComplete = null)
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

        onComplete?.Invoke();
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

    public void AbsorbToGoal(Vector3 goalPosition)
    {
        StartCoroutine(AbsorbAnimation(goalPosition));
    }

    private IEnumerator AbsorbAnimation(Vector3 goalPosition)
    {
        isMoving = true;
        enabled = false;

        float duration = 0.8f;
        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(startPos, goalPosition, t);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            transform.Rotate(0, 0, 360f * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
        transform.position = goalPosition;
        //Destroy(gameObject);

    }



}





