using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSFX_Box : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //sound effect caller
        SfxManager.Instance.PlaySFX(8834);
    }
}
