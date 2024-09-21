using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Sound_Caller : MonoBehaviour
{

    public void ActivateSFX()
    {
        SfxManager.Instance.PlaySFX(6893);
    }
}
