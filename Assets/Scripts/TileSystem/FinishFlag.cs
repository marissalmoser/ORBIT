/******************************************************************
*    Author: Elijah Vroman
*    Contributors: 
*    Date Created: 11/04/24
*    Description: The reason for the unfortunate state of this script
*    is the result of some interesting unintended game mechanic 
*    interactions. 
*       1. The player needs to stop as soon as it hits the flag, so 
*       that it doesn't overshoot. However, this means it stops at 
*       a weird point on the board
*       2. Coroutines are finicky and can't be depended upon to get
*       solid delays/stoppings for everyones computer.
*       3. So, the player will now hit the flag, stop and empty its
*       actions, and then lerp its transform to the flag. Until Finish
*       script is looked at and updated, this is the best we have for
*       now.
*******************************************************************/
using System.Collections;
using UnityEngine;

public class FinishFlag : Collectable
{
    private PlayerStateMachineBrain _PSMB;
    private PlayerController _pc;
    private bool _hitWinAsPlayer = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!_hitWinAsPlayer&& other.gameObject.CompareTag("Player"))
        {           
            _hitWinAsPlayer = true;
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
            _pc = other.GetComponent<PlayerController>();

            while (_PSMB.GetNextAction() != null) //removes all remaining actions
            {

            }
            StartCoroutine(GhostWaitCoroutine());
        }
    }
    private IEnumerator WaitCoroutine()
    {
        _pc.StopCoroutine(_pc.GetCurrentMovementCoroutine());
        _pc.SetCurrentTile(_pc.GetTileWithPlayerRaycast());

        _pc.StartSimpleMoveCoroutine(_pc.transform.position, transform.position, .5f);

        yield return new WaitForSeconds(.75f);
        GameManager.Instance.ChangeGameState(GameManager.STATE.End);
        //print("WIN");
    }
    private IEnumerator GhostWaitCoroutine()
    {
        _pc.StopCoroutine(_pc.GetCurrentMovementCoroutine());

        _pc.StartSimpleMoveCoroutine(_pc.transform.position, transform.position, .5f);

        yield return new WaitForSeconds(.75f);
        //PlayerController.ReachedDestination?.Invoke();
    }
}
