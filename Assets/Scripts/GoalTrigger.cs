using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private bool isMatched = false;
    private Color goalColor;

    public bool IsMatched => isMatched;

    //Parameters for animation
    private float scaleAmplitude = 0.1f;  
    private float scaleSpeed = 2f;          
    private float rotateSpeed = 40f;

    private Vector3 initialScale;

    private void Start()
    {
        goalColor = GetComponent<SpriteRenderer>().color;
        initialScale = transform.localScale;
    }

    void Update()
    {
        AnimateGoal();
    }


    private void AnimateGoal()
    {
        float scaleOffset = Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;
        transform.localScale = initialScale + Vector3.one * scaleOffset;

        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }


    public void OnBoxArrived(PushableBox box)
    {
        if (isMatched) return;

        Color boxColor = box.GetComponent<SpriteRenderer>().color;
        if (ColorUtils.IsSimilar(boxColor, goalColor))
        {
            isMatched = true;
            AudioManager.Instance.Play("Suck");
            GameManager.Instance.OnBoxAbsorbed();
            box.AbsorbToGoal(transform.position);
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            isMatched = false;
        }
    }

}

