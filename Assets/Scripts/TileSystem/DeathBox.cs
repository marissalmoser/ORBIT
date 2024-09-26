using UnityEngine;

public class DeathBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.Instance.ChangeGameState(GameManager.STATE.Death);
        }
        else if(other.CompareTag("PlayerGhost"))
        {
            PlayerStateMachineBrain psmb = other.gameObject.GetComponentInParent<PlayerStateMachineBrain>(true);
            while(psmb.GetNextAction() != null) //removes all remaining actions
            {

            }
            PlayerController.ReachedDestination?.Invoke();
        }
    }
}