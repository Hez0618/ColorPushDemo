using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private bool hasWon = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasWon) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AbsorbToGoal(transform.position);
            }
            hasWon = true;
            Debug.Log("Goal!");
            GameManager.Instance.OnGameWin();
        }
    }
}

