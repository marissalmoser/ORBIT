using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSFX_Box : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //sound effect caller
        GameObject manager = GameObject.Find("SfxManager");
        SfxManager function_call = (SfxManager)manager.GetComponent(typeof(SfxManager));
        function_call.PlaySFX(8834);
        function_call.FadeOutSFX(8834);
    }
}
