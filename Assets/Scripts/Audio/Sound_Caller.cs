using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Sound_Caller : MonoBehaviour
{
    public int SFX_ID;

    public void ActivateSFX()
    {
        GameObject manager = GameObject.Find("SfxManager");
        SfxManager function_call = (SfxManager)manager.GetComponent(typeof(SfxManager));

        switch (SFX_ID)
        {
            case 6893:
                //play ButtonClick
                function_call.PlaySFX(6893);
                break;
            case 3541:
                //play CardPickedUp
                function_call.PlaySFX(3541);
                break;
            case 1092:
                //play CardPutDown
                function_call.PlaySFX(1092);
                break;
            case 6706:
                //play LoadIn
                function_call.PlaySFX(6706);
                break;
            case 6789:
                //play LoadOut
                function_call.PlaySFX(6789);
                break;
            case 1191:
                //play MovingWallMove
                function_call.PlaySFX(1191);
                break;
            case 8834:
                //play PlayerFallOffMap
                function_call.PlaySFX(8834);
                break;
            case 9754:
                //play PlayerMove
                function_call.PlaySFX(9754);
                break;
            case 1987:
                //play SpikesDown
                function_call.PlaySFX(1987);
                break;
            case 4136:
                //play SpikesUp
                function_call.PlaySFX(4136);
                break;
            case 3740:
                //play PlayerJump
                function_call.PlaySFX(3740);
                break;
            case 1917:
                //play PlayerJumpHigh
                function_call.PlaySFX(1917);
                break;
            case 2469:
                //play Woosh1
                function_call.PlaySFX(2469);
                break;
            case 4295:
                //play Woosh2
                function_call.PlaySFX(4295);
                break;
            case 8894:
                //play Woosh3
                function_call.PlaySFX(8894);
                break;

        }
    }
}
