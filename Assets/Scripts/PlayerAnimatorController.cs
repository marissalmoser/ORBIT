using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [Tooltip("Enter as float value from 0 to 1")][SerializeField] private float BoredPercentChance;
    private Animator animator;
    private void Start()
    {
        if (BoredPercentChance > 1)
        {
            Debug.LogWarning("The player will play bored idle animations 100% of the time every second. Did you set the probablity correctly?");
        }
        if (GetComponentInParent<Animator>() != null)
        {
            animator = GetComponentInParent<Animator>();
        }

    }
    public void SeeIfBored()
    {
        if (Random.Range(0, 100) <= BoredPercentChance * 100f)
        {
            PlayAnIdleAnim();
        }
    }
    private void PlayAnIdleAnim()
    {
        int val = Random.Range(1, 10);
        animator.SetInteger("Random", val);
        animator.SetTrigger("Bored");
    }
}