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

    [Tooltip("Array of particle systems, index of system is same as particleID")][SerializeField] private ParticleSystem[] particles;
    //Particle Dictionary--
    //0 - All Purpose Dust
    //1 - Sun Landing

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

    public void TurnPlayerLeft()
    {
        playerController.UpdateFacingDirection(true);
    }
    public void TurnPlayerRight()
    {
        playerController.UpdateFacingDirection(false);
    }

    public void ShakeCamera()
    {
        ShakeManager.ShakeCamera(2, 1, 0.3f);
    }

    public void SpawnParticle(int particleID = 0)
    {
        //temp fix VVV
        if (gameObject.name == "MainModel")
        {
            particles[particleID].Play();

            switch (particleID)
            {
                //Additional particle logic gets called here:

                //all purpose dust
                case 0:
                    break;
                //landing sun burst
                case 1:
                    break;
                //default sun aura
                case 2:
                    break;
                //sunglasses --- UNUSED
                case 3:
                    break;
                //sun movement
                case 4:
                    break;
                //wall bump dust
                case 5:
                    break;
                //landing sun burst but smaller
                case 6:
                    break;
                //celebration particles --- used for winning
                case 7:
                    break;
                case 8:
                    //moon collection particle --- used for collectable
                    break;
                default:
                    Debug.LogError("Error! particleID outside of knowable range!");
                    break;
            }
        }
    }
    public void StopParticle(int particleID = 0)
    {
        //temp fix VVV
        if (gameObject.name == "MainModel")
        {
            particles[particleID].Stop();

            switch (particleID)
            {
                //Additional particle logic gets called here:

                //all purpose dust
                case 0:
                    break;
                //landing sun burst
                case 1:
                    break;
                //default sun aura
                case 2:
                    break;
                //sunglasses --- UNUSED
                case 3:
                    break;
                //sun movement
                case 4:
                    break;
                //wall bump dust
                case 5:
                    break;
                //landing sun burst but smaller
                case 6:
                    break;
                //celebration particles --- used for winning
                case 7:
                    break;
                case 8:
                    //moon collection particle --- used for collectable
                    break;
                default:
                    Debug.LogError("Error! particleID outside of knowable range!");
                    break;
            }
        }
    }
}
