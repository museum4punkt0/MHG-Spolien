using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureOrbitCamera : MonoBehaviour
{
    public Vector3 rotation = new Vector3(0, 0.1f, 0);
   
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation,Space.Self);
    }
}
