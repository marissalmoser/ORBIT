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
            GameObject manager = GameObject.Find("SfxManager");
            SfxManager function_call = (SfxManager)manager.GetComponent(typeof(SfxManager));
            function_call.PlaySFX(1566);

            _PSMB = other.GetComponent<PlayerStateMachineBrain>();
            StartCoroutine(WaitCoroutine());
        }
    }
    private IEnumerator WaitCoroutine()
    {
        _PSMB.SetCardList(null);
        yield return new WaitForSeconds(1);
        GameManager.Instance.ChangeGameState(GameManager.STATE.End);
        print("WIN");
    }
}
