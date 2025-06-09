using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{    

    public float moveSpeed = 5f;
    public LayerMask obstacleLayers;
    public LayerMask boxLayer;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isMoving = false;

    public GameObject arrowPrefab;
    private GameObject arrowInstance;

    private List<PushableBox> nearbyBoxes = new List<PushableBox>();
    private int selectedBoxIndex = -1;
    
    private SpriteRenderer spriteRenderer;
    private Color currentColor = Color.gray;
    private Coroutine colorLerpCoroutine;
    private float colorLerpDuration = 0.5f;

    [HideInInspector]
    public int splashContactCount = 0;
    public bool isOnSplashTile => splashContactCount > 0;
    public DeniedEffect deniedEffect;



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Vector2 startPos = new Vector2(Mathf.Round(rb.position.x), Mathf.Round(rb.position.y));
        rb.position = startPos;
        transform.position = startPos;
        UpdatePlayerColorVisual();
        spriteRenderer.color = Color.gray;
        CreateOutline();
    }




    private void Update()
    {
        

        //move
        if (isMoving) return;

        Vector2 inputDirection = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow)) inputDirection = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) inputDirection = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) inputDirection = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) inputDirection = Vector2.right;

        if (inputDirection != Vector2.zero)
        {
            StartCoroutine(MoveOneGrid(inputDirection));
        }

        if (isOnSplashTile && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.E)))
        {
            AudioManager.Instance.Play("Error");
            PlayShakeEffect();
            if (deniedEffect != null)
                deniedEffect.Play();
        }


        if (!isOnSplashTile)
        {
            //TAB to select nearby boxes
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                FindNearbyBoxes();
                SelectNextBox();
                ShowSelectedBoxHighlight();
            }

            if (selectedBoxIndex >= 0)
            {

                FindNearbyBoxes();


                if (selectedBoxIndex >= nearbyBoxes.Count || !nearbyBoxes.Contains(nearbyBoxes[selectedBoxIndex]))
                {
                    
                    selectedBoxIndex = -1;
                    RemoveArrow();
                }
            }

            // press E to change Color
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (selectedBoxIndex >= 0 && selectedBoxIndex < nearbyBoxes.Count)
                {
                    Color targetColor = nearbyBoxes[selectedBoxIndex].GetComponent<SpriteRenderer>().color;
                    AudioManager.Instance.Play("Dye");
                    SetColorSmoothly(targetColor);
                    //currentColor = nearbyBoxes[selectedBoxIndex].GetComponent<SpriteRenderer>().color;
                    //UpdatePlayerColorVisual();
                    Debug.Log($"玩家颜色已改变为：{currentColor}");
                    RemoveArrow();
                }
                else
                {
                    Debug.Log("没有选中任何箱子");
                }
            }
        }
        

    }

    public void SetColorSmoothly(Color targetColor)
    {
        if (colorLerpCoroutine != null)
            StopCoroutine(colorLerpCoroutine);

        colorLerpCoroutine = StartCoroutine(SmoothChangeColor(targetColor));
    }

    private IEnumerator SmoothChangeColor(Color targetColor)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color startColor = sr.color;
        float elapsed = 0f;

        while (elapsed < colorLerpDuration)
        {
            elapsed += Time.deltaTime;
            sr.color = Color.Lerp(startColor, targetColor, elapsed / colorLerpDuration);
            yield return null;
        }

        sr.color = targetColor;
        currentColor = targetColor;
    }

    //when the player is on splash and try to change color 
    public void PlayShakeEffect(float duration = 0.1f, float magnitude = 0.03f)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            transform.position = originalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }


    private IEnumerator MoveOneGrid(Vector2 direction)
    {
        isMoving = true;

        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + direction;

        Vector2 size = boxCollider.size * 0.9f;


        Collider2D hit = Physics2D.OverlapBox(targetPos, size, 0f, obstacleLayers);

        if (hit != null)
        {

            if (((1 << hit.gameObject.layer) & boxLayer) != 0)
            {
                PushableBox box = hit.GetComponent<PushableBox>();
                if (box != null)
                {
                    // Try to push
                    if (box.TryMove(direction, currentColor))
                    {
                        Debug.Log("succesfully pushed");
                    }
                    else
                    {
                        AudioManager.Instance.Play("Hit");
                        Debug.Log("Obstacle!");
                        isMoving = false;
                        yield break;
                    }
                }
                else
                {
                    Debug.LogWarning("No script!");
                    isMoving = false;
                    yield break;
                }
            }
            else
            {
                PlayShakeEffect();
                AudioManager.Instance.Play("Hit");
                Debug.Log("Hit Wall");
                print(hit.gameObject.name);
                isMoving = false;
                yield break;
            }
        }

        AudioManager.Instance.Play("Move");

        float elapsed = 0f;
        float duration = 1f / moveSpeed;

        while (elapsed < duration)
        {
            Vector2 newPos = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            rb.MovePosition(newPos);
            elapsed += Time.deltaTime;
            yield return null;
        }


        rb.MovePosition(targetPos);
        Vector2 roundedPos = new Vector2(Mathf.Round(rb.position.x), Mathf.Round(rb.position.y));


        if (Vector2.Distance(rb.position, roundedPos) > 0.01f)
        {
            rb.MovePosition(roundedPos);
        }

        isMoving = false;
    }

    private void FindNearbyBoxes()
    {
        nearbyBoxes.Clear();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f, boxLayer);

        foreach (var hit in hits)
        {
            Vector2 dir = hit.transform.position - transform.position;
            dir = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y));


            if (dir == Vector2.up || dir == Vector2.down || dir == Vector2.left || dir == Vector2.right)
            {
                PushableBox box = hit.GetComponent<PushableBox>();
                if (box != null)
                    nearbyBoxes.Add(box);
            }
        }
    }

    private void SelectNextBox()
    {
        if (nearbyBoxes.Count == 0)
        {
            selectedBoxIndex = -1;
            Debug.Log("No Box Nearby");
            return;
        }

        // 取消之前选中箱子的高亮
        if (selectedBoxIndex >= 0 && selectedBoxIndex < nearbyBoxes.Count)
        {
            nearbyBoxes[selectedBoxIndex].SetHighlight(false);
        }

        selectedBoxIndex++;
        if (selectedBoxIndex >= nearbyBoxes.Count)
            selectedBoxIndex = 0;

        Debug.Log($"Box selected：{nearbyBoxes[selectedBoxIndex].name}");
    }


    private void ShowSelectedBoxHighlight()
    {
        RemoveArrow();

        if (selectedBoxIndex >= 0 && selectedBoxIndex < nearbyBoxes.Count)
        {

            Transform targetBox = nearbyBoxes[selectedBoxIndex].transform;

            AudioManager.Instance.Play("Note");
            arrowInstance = Instantiate(arrowPrefab);
            arrowInstance.transform.SetParent(targetBox);
            arrowInstance.transform.localPosition = new Vector3(0, 0.5f, 0); 

 
        }


        for (int i = 0; i < nearbyBoxes.Count; i++)
        {
            if (i == selectedBoxIndex)
                Debug.Log($"[highlighted] {nearbyBoxes[i].name}");
            else
                Debug.Log($"{nearbyBoxes[i].name}");
        }
    }

    private void UpdatePlayerColorVisual()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = currentColor;
    }


    private void CreateOutline()
    {
        string[] directions = { "Down" };
        Vector2[] offsets = { new Vector2(0, -0.05f) };

        SpriteRenderer baseSprite = GetComponent<SpriteRenderer>();

        GameObject outlineGroup = new GameObject("OutlineGroup");
        outlineGroup.transform.SetParent(transform);
        outlineGroup.transform.localPosition = Vector3.zero;

        for (int i = 0; i < directions.Length; i++)
        {
            GameObject part = new GameObject("Outline_" + directions[i]);
            part.transform.SetParent(outlineGroup.transform);
            part.transform.localPosition = offsets[i];

            SpriteRenderer sr = part.AddComponent<SpriteRenderer>();
            sr.sprite = baseSprite.sprite;
            sr.color = new Color(0, 0, 0, 0.5f);  
            sr.sortingLayerID = baseSprite.sortingLayerID;
            sr.sortingOrder = baseSprite.sortingOrder - 1;
        }
    }

    private void RemoveArrow()
    {
        if (arrowInstance != null)
        {
            Destroy(arrowInstance);
            arrowInstance = null;
        }
    }



}









