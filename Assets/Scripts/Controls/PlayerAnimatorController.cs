/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 10/14/24
*    Description: This script is used solely as a reference for the 
*    animations to use when making an animation event
*******************************************************************/
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [Tooltip("Enter as float value from 0 to 1")][SerializeField] private float BoredPercentChance;
    private Animator animator;
    private PlayerController playerController;
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
        if(GetComponentInParent<PlayerController>() != null)
        {
            playerController = GetComponentInParent<PlayerController>();
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
        playerController.PlayAnimation("Bored", -1);
    }
}
