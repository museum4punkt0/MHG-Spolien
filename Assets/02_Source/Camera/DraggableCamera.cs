using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace jn
{
    public class DraggableCamera : CameraController
    {
        // =================================================================
        #region Odin Menu Names
        static readonly string TAB_GROUP = "TabGroup";
        static readonly string REFERENCES_TAB = "References";
        static readonly string SETTINGS_TAB = "Settings";
        #endregion

        #region Const Strings
        #endregion
        // =================================================================

        // =================================================================
        #region Public Fields
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public Transform camTarget;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB"), MinMaxSlider(1, 30)]   
        public Vector2 minMaxZoom;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]
        public float maxDist = 50f;
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        protected new void Awake()
        {        
            base.Awake();    
            currentZoom = ((Cinemachine.CinemachineVirtualCamera)virtualCam).m_Lens.OrthographicSize;
        }

        protected new void Update()
        {
            base.Update();

            //if (interactable)
            //{
            //    bool clickedOnUI = false;
            //    bool controlling = false;
            //    bool clicked = false;
            //    bool moving = false;
            //    bool zooming = false;
            //    Vector2 screenPos = Vector2.zero;
            //    Vector2 zoomDelta = Vector2.zero;

            //    #if UNITY_EDITOR
            //    GraphicRaycaster raycaster = GameObject.FindObjectOfType<GraphicRaycaster>();
            //    PointerEventData eventData = new PointerEventData (EventSystem.current);
            //    eventData.position = Input.mousePosition ; 

            //    List<RaycastResult> raycastResults = new List<RaycastResult>();
            //    raycaster.Raycast(eventData, raycastResults);
            //    //Debug.Log($"Raycast hit {raycastResults.Count}");  
            //    clickedOnUI = raycastResults.Count > 0;

            //    screenPos = Input.mousePosition;
            //    zoomDelta = Input.mouseScrollDelta;

            //    controlling = (touchDelta.magnitude > 0.025f) && Input.GetMouseButton(0);
            //    moving = controlling;
            //    clicked = Input.GetMouseButtonUp(0);
            //    zooming = (Mathf.Abs(Input.mouseScrollDelta.y) > 0.1f);
                
            //    //#elif (UNITY_IOS || UNITY_ANDROID)

            //    if(Input.touchCount > 0 && Input.touchCount < 2)
            //    {
            //        GraphicRaycaster raycaster = GameObject.FindObjectOfType<GraphicRaycaster>();
            //        PointerEventData eventData = new PointerEventData (EventSystem.current);
            //        eventData.position = Input.GetTouch(0).position; 

            //        List<RaycastResult> raycastResults = new List<RaycastResult>();
            //        raycaster.Raycast(eventData, raycastResults);
            //        //Debug.Log($"Raycast hit {raycastResults.Count}");  
            //        clickedOnUI = raycastResults.Count > 0;
                    
            //        controlling = Input.GetTouch(0).phase == TouchPhase.Moved;
            //        clicked = Input.GetTouch(0).phase == TouchPhase.Ended;
            //        screenPos = Input.GetTouch(0).position;
            //    }
            //    else if(Input.touchCount > 1)
            //    {
            //        //Check for UI
            //        clickedOnUI = false;
            //        foreach (Touch touch in Input.touches)
            //        {
            //            int id = touch.fingerId;
            //            //Debug.Log($"Testing Touch {id} if its over UI at {touch.position}");
            //            if (EventSystem.current.IsPointerOverGameObject(id))
            //            {
            //                // ui touched
            //                clickedOnUI = true;
            //            }
            //        }
                    
            //        float fingerdelta = 0;
            //        moving = Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved;
            //        clicked = Input.GetTouch(0).phase == TouchPhase.Ended;
                    
            //        float mag = (touch1Pos - touch2Pos).magnitude;
            //        //if (prevmag != -1)
            //        //{
            //            fingerdelta = prevmag - mag;
            //            //if (Mathf.Abs(fingerdelta) > 0.01f)
            //            //{
            //                zoom += fingerdelta * zoomSpeedOnMobile;
            //            //    zooming = true;
            //            //}
            //        //}
            //        prevmag = mag;

            //        Debug.Log($"Pinch zoom: {zoom}");
            //        Debug.Log($"Pinch Movement: {touchCenterDelta}");
            //    }
            //    #endif

            //    if (clickedOnUI)
            //    {
            //        //Debug.Log("Clicked on UI");
            //        return;
            //    }

            //    //Don't Raycast if this Touch has ever dragged
            //    if (controlling || dragged || moving)
            //    {
            //        dragged = true;
            //    }

            //    //if (controlling && !(moving || zooming))
            //    //{
            //    //    camTarget.Translate(new Vector3(touchDelta01.x, touchDelta01.y, 0f));
            //    //    //freelook.m_XAxis.m_InputAxisValue = inputDelta.x;
            //    //    //freelook.m_YAxis.m_InputAxisValue = inputDelta.y;
                    
            //    //    camTarget.localPosition = new Vector3(Mathf.Clamp(camTarget.localPosition.x, -16f,16f),
            //    //                                     Mathf.Clamp(camTarget.localPosition.y, -16f,16f), 0f);
            //    //}
                
            //    if (zooming)// || moving)
            //    {
            //        float currentZoom = ((Cinemachine.CinemachineVirtualCamera)virtualCam).m_Lens.OrthographicSize;
            //        float newZoom =  Mathf.Clamp(currentZoom + zoom, minMaxZoom.x, minMaxZoom.y);

            //        ((Cinemachine.CinemachineVirtualCamera)virtualCam).m_Lens.OrthographicSize = newZoom;

            //        //camTarget.Translate(new Vector3(touchCenterDelta01.x, touchCenterDelta01.y, 0f));
            //        //camTarget.localPosition = new Vector3(Mathf.Clamp(camTarget.localPosition.x, -16f,16f),
            //        //                                 Mathf.Clamp(camTarget.localPosition.y, -16f,16f), 0f);
            //    }

            //    if (!moving)
            //    {
            //        prevmag = -1f;
            //    }

            //    if (clicked && !dragged)
            //    {
            //        GameManager.Instance.interactionManager.RaycastForInteractable(screenPos);
            //    }

            //    if (clicked)
            //    {
            //        dragged = false;    
            //    }
            //}
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public override void MoveTwoFinger(Vector2 movement)
        {
            
        }

        public override void MoveOneFinger(Vector2 movement)
        {            
            camTarget.Translate(new Vector3(movement.x, movement.y, 0f));

            camTarget.localPosition = new Vector3(Mathf.Clamp(camTarget.localPosition.x, -16f, 16f),
                                             Mathf.Clamp(camTarget.localPosition.y, -16f, 16f), 0f);
        }

        public override void Zoom(float amount)
        {
            currentZoom =  Mathf.Clamp(currentZoom + amount, minMaxZoom.x, minMaxZoom.y);

            ((Cinemachine.CinemachineVirtualCamera)virtualCam).m_Lens.OrthographicSize = currentZoom;
        }

        public void SetZoomExternally(float zoomLevel)
        {
            ((Cinemachine.CinemachineVirtualCamera)virtualCam).m_Lens.OrthographicSize = Mathf.Clamp(zoomLevel, minMaxZoom.x, minMaxZoom.y);            
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
