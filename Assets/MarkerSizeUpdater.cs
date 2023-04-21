using jn;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MarkerSizeUpdater : MonoBehaviour
{
    float size = 0.15f;
    private void Start()
    {
        if(TryGetComponent<MapBuilding>(out var mapBuilding))
        {
            size = mapBuilding.size;
        }
    }
    private void Update()
    {
        //Constant Size in Screen Space
        Camera mapCam = GameManager.Instance.cameraManager.mapDragCamera.associatedCam;

        float worldScreenHeight = mapCam.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector3(
        worldScreenWidth * size,
            worldScreenWidth * size,
            1);
    }
}
