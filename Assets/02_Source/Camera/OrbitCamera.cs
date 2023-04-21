using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace jn
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class OrbitCamera : CameraController
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
        //[TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        //[TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public float speed = 0.1f;
        public float moveSpeed = 1f;
        public float maxDist = 50f;
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private CinemachineFreeLook freelook;
        private Vector3 startPos;
        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        protected new void Awake()
        {
            base.Awake();

            freelook = (CinemachineFreeLook)virtualCam;

            freelook = GetComponent<CinemachineFreeLook>();
            freelook.m_XAxis.m_InputAxisName = "";
            freelook.m_YAxis.m_InputAxisName = "";
            currentZoom = freelook.m_Lens.FieldOfView;    

            startPos = freelook.m_Follow.transform.position;
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
            //    Vector3 moveInput = Vector2.zero;
            //    Vector2 screenPos = Vector2.zero;

            //    #if UNITY_EDITOR
            //    GraphicRaycaster raycaster = GameObject.FindObjectOfType<GraphicRaycaster>();
            //    PointerEventData eventData = new PointerEventData (EventSystem.current);
            //    eventData.position = Input.mousePosition ; 

            //    List<RaycastResult> raycastResults = new List<RaycastResult>();
            //    raycaster.Raycast(eventData, raycastResults);
            //    //Debug.Log($"Raycast hit {raycastResults.Count}");  
            //    clickedOnUI = raycastResults.Count > 0;
                
            //    clicked = Input.GetMouseButtonUp(0);
            //    screenPos = Input.mousePosition;
            //    controlling = (touchDelta.magnitude > 0.1f) && Input.GetMouseButton(0);
                

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
            //        Debug.Log($"Finger 01 delta: {touchDelta}");
            //    }
            //    else if(Input.touchCount > 1)
            //    {

            //        //Check for touches on UI
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
            //        if (prevmag != -1)
            //        {
            //            fingerdelta = prevmag - mag;
            //            if (Mathf.Abs(fingerdelta) > 0.01f)
            //            {
            //                zoom += fingerdelta * zoomSpeedOnMobile;
            //                //Debug.Log("zooming");
            //                zooming = true;
            //            }
            //        }
            //        prevmag = mag;

            //        Debug.Log($"Pinch delta: {fingerdelta}");
            //        Debug.Log($"Pinch Movement: {touchCenterDelta}");
            //    }
            //    #endif

            //    if (clickedOnUI)
            //    {
            //        //Debug.Log("Clicked on UI");
            //        freelook.m_XAxis.m_InputAxisValue = 0f;
            //        freelook.m_YAxis.m_InputAxisValue = 0f;
            //        return;
            //    }

            //    //Don't Raycast if this Touch has ever dragged
            //    if (controlling || dragged ||moving)
            //    {
            //        dragged = true;
            //    }

            //    if (controlling && !(moving|| zooming))
            //    {
            //        freelook.m_XAxis.m_InputAxisValue = touchDelta.x;
            //        freelook.m_YAxis.m_InputAxisValue = touchDelta.y;

            //        if(GameManager.Instance.stateSelection == SelectionMode.Orbit)
            //        {
            //            if (!GameManager.Instance.menuManager.hudMenu.GetMovedAlready())
            //            {
            //                GameManager.Instance.menuManager.hudMenu.SetMovedAlready(true);
            //                GameManager.Instance.menuManager.hudMenu.UpdateState();
            //            }
            //        }
            //    }
            //    else if (moving && !zooming)
            //    {
            //        Vector3 translate = Quaternion.Euler(0, freelook.m_XAxis.Value, 0) * new Vector3(touchCenterDelta.x, touchCenterDelta.y, 0);
            //        translate = translate * -0.1f;
            //        if ((freelook.m_Follow.transform.localPosition + (translate)).magnitude < maxDist)
            //        {
            //            freelook.m_Follow.transform.Translate(translate);
            //        }
            //    }
            //    else if (zooming)
            //    {
            //        if (zoom > 90)
            //        {
            //            zoom = 90f;
            //        }
            //        if (zoom < 30)
            //        {
            //            zoom = 30;
            //        }
            //        freelook.m_Lens.FieldOfView = zoom;
            //    }
            //    else
            //    {
            //        freelook.m_XAxis.m_InputAxisValue = 0f;
            //        freelook.m_YAxis.m_InputAxisValue = 0f;                    
            //    }

            //    if (!moving)
            //    {
            //        prevmag = -1f;
            //    }

            //    if (clicked && !dragged)
            //    {
            //        interactionManager.RaycastForInteractable(screenPos);
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
            //Move the Camera
            //Vector3 translate = Quaternion.Euler(0, freelook.m_XAxis.Value, 0) * new Vector3(movement.x, movement.y, 0);
            //translate = translate * -0.1f;
             
            movement *= (inverted ? -1f : 1f);

            Vector3 translate = new Vector3(movement.x, movement.y, 0);
            //Debug.Log($"Translating Camera Target by {translate}");

            //Get the location of the UI element you want the 3d onject to move towards
            //Vector3 screenPoint = ui_element_gameobject.transform.position + new Vector3(0,0,5);  //the "+ new Vector3(0,0,5)" ensures that the object is so close to the camera you dont see it
             
            //find out where this is in world space
            Camera currCam = GameManager.Instance.cameraManager.currCamController.associatedCam;


            Vector3 screenPos = currCam.WorldToScreenPoint( freelook.m_Follow.transform.position );
            
            //#if UNITY_EDITOR
            screenPos -= translate;
            //#elif (UNITY_IOS || UNITY_ANDROID)
            //screenPos -= translate;
            //#endif

            Vector3 newWorldPos = currCam.ScreenToWorldPoint( screenPos );

            float nextDistance = (newWorldPos - startPos).magnitude;
            //Debug.Log($"Next Distance is {nextDistance}");

            if (nextDistance < maxDist)
            {
                freelook.m_Follow.transform.position = newWorldPos;
            }
        }
        public override void MoveOneFinger(Vector2 movement)
        {            
            freelook.m_XAxis.m_InputAxisValue = movement.x;
            freelook.m_YAxis.m_InputAxisValue = movement.y;        
        }
        public override void Zoom(float amount)
        {
            currentZoom = Mathf.Clamp(currentZoom + amount, 30f, 90f);
            freelook.m_Lens.FieldOfView = currentZoom; 
        }

        public void SetInteractable(bool interactable)
        {
            this.interactable = interactable;
            if (!interactable)
            {
                freelook.enabled = false;
            }
            else
            {
                freelook.enabled = true;
            }
        }

        public void setToDefaultView(float rot)
        {
            freelook.m_XAxis.Value = rot;
            freelook.m_YAxis.Value = 0.5f;
        }

        public void setFreeLookSize(Vector3 objectSize)
        {
            float maxWidth = Mathf.Max(objectSize.x,objectSize.z);
            float height = objectSize.y;
            float aspect = Camera.main.aspect;

            float distanceH = ((height/2) / Mathf.Tan((currentZoom / 2) * Mathf.Deg2Rad))+maxWidth/2;
            float distanceW = (maxWidth / 2) / Mathf.Tan(((currentZoom*aspect) / 2) * Mathf.Deg2Rad);
            Debug.Log(distanceW + "  " + distanceH);
            float maxDist = Mathf.Max(distanceH, distanceW);
            freelook.m_Orbits = new CinemachineFreeLook.Orbit[] { new CinemachineFreeLook.Orbit(maxDist, 0.5f), new CinemachineFreeLook.Orbit(0, maxDist), new CinemachineFreeLook.Orbit(-maxDist, 0.5f) };
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
