using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace jn
{
    public static class CameraExtensions
     {
         public static Bounds OrthograpicBounds2D(this Camera camera)
         {
             float screenAspect = (float)Screen.width / (float)Screen.height;
             float cameraHeight = camera.orthographicSize * 2;
             Bounds bounds = new Bounds(
                 camera.transform.position,
                 new Vector3(cameraHeight * screenAspect, cameraHeight, Mathf.Infinity));
             return bounds;
         }
     }
}
