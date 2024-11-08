using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionAnimation : MonoBehaviour
{
    public void SetBool()
    {
        SceneTransitionManager.Instance.SetAnimHasPlayed();
    }
}
