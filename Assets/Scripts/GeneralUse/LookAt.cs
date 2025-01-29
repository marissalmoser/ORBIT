using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    [SerializeField] private bool lookAtCam;

    // Start is called before the first frame update
    void Start()
    {
        if (lookAtCam == true)
        {
            target = Camera.main.transform;
        }
        transform.LookAt(target, Vector3.up);
    }
    void Update()
    {
        // Rotate the camera every frame so it keeps looking at the target
        //transform.LookAt(target);

        // Same as above, but setting the worldUp parameter to Vector3.up
        transform.LookAt(target, Vector3.up);
    }

}
