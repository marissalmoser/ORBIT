using System.Collections;
using UnityEngine;

public class FinishFlag : Collectable
{
    private PlayerStateMachineBrain _PSMB;
    private PlayerController _pc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {           
            //sound effect caller
            SfxManager.Instance.PlaySFX(1566);

            _PSMB = other.gameObject.GetComponent<PlayerStateMachineBrain>();
            _pc = other.GetComponent<PlayerController>();

            while (_PSMB.GetNextAction() != null) //removes all remaining actions
            {

            }
            StartCoroutine(WaitCoroutine());

        }
        else if (other.CompareTag("PlayerGhost"))
        {
            _PSMB = other.gameObject.GetComponentInParent<PlayerStateMachineBrain>(true);

            while (_PSMB.GetNextAction() != null) //removes all remaining actions
            {

            }
            StartCoroutine(GhostWaitCoroutine());
        }
    }
    private IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(.1f);

        _pc.StopCoroutine(_pc.GetCurrentMovementCoroutine());
        _pc.SetCurrentTile(_pc.GetTileWithPlayerRaycast());

        yield return new WaitForSeconds(.25f);
        GameManager.Instance.ChangeGameState(GameManager.STATE.End);
        print("WIN");
    }
    private IEnumerator GhostWaitCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerController.ReachedDestination?.Invoke();
    }
}
