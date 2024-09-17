using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Caller : MonoBehaviour
{

    public void ActivateSFX()
    {
        FindObjectOfType<SfxManager>().PlaySFX(6893);
    }

}
