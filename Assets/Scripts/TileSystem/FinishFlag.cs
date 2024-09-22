using System.Collections;
using UnityEngine;

public class FinishFlag : Collectable
{
    private PlayerStateMachineBrain _PSMB;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //sound effect caller
            SfxManager.Instance.PlaySFX(1566);

            _PSMB = other.GetComponent<PlayerStateMachineBrain>();
            StartCoroutine(WaitCoroutine());
        }
        else if (other.CompareTag("PlayerGhost"))
        {
            PlayerStateMachineBrain psmb = other.gameObject.GetComponentInParent<PlayerStateMachineBrain>(true);
            while (psmb.GetNextAction() != null) //removes all remaining actions
            {

            }
            PlayerController.ReachedDestination?.Invoke();
        }
    }
    private IEnumerator WaitCoroutine()
    {
        //_PSMB.SetCardList(null);
        _PSMB.StartCardActions(null);
        yield return new WaitForSeconds(.75f);
        GameManager.Instance.ChangeGameState(GameManager.STATE.End);
        print("WIN");
    }
}
