using System.Runtime.CompilerServices;
using UnityEngine;

public class AnimationController : MonoBehaviour
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

        }
    }
    private void PlayAnIdleAnim()
    {
        int val = Random.Range(0, 10);
        switch (val)
        {
            case 0:
            case 1:
            case 2:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:

                break;
        }
    }
}
