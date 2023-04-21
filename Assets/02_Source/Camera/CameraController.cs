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
    public class CameraController : MonoBehaviour
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
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public bool interactable;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public bool inverted;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public float speedInEditor;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public float speedOnMobile;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public float zoomSpeedInEditor;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public float zoomSpeedOnMobile;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public float moveSpeedInEditor;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public float moveSpeedOnMobile;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public Camera associatedCam;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public CinemachineVirtualCameraBase virtualCam;
        //[TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        [InfoBox("These are the values st to AxisControl values of the FreeLook Camera")]
        public Vector2[] defaultViews;
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private int lastDefaultView = -1;

        protected Vector2 touch1Pos = Vector2.zero;
        protected Vector2 touch2Pos = Vector2.zero;
        
        protected Vector2 prevTouch1Pos = Vector2.zero;
        protected Vector2 prevTouch2Pos = Vector2.zero;

        protected Vector2 touchDelta = Vector2.zero;
        protected Vector2 touch2Delta = Vector2.zero;

        protected Vector2 touchCenterDelta = Vector2.zero;
        protected Vector2 touchCenterPos = Vector2.zero;
                
        private float zoomInput;
        private Vector2 oneFingerDragInput = Vector2.zero;
        private Vector2 twoFingerDragInput = Vector2.zero;

        private InteractionManager interactionManager;

        protected float currentZoom = 55f;

        private bool resetPastZoomTouches = true;
        private bool resetPastDragTouches = true;
        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        protected void Awake()
        {
            interactionManager = GameObject.FindObjectOfType<InteractionManager>();
        }

        protected void Update()
        {
            if (interactable)
            {
                Vector2 touch1PosSS = Vector2.zero; //In screen-space for UI raycasting later
                bool hoveringOverUI = false;

                bool clicked = false;
                bool moving = false;
                bool zooming = false;
                bool dragging = false;

                #if UNITY_EDITOR

                    //Read Inputs
                    touchDelta.x = Input.GetAxis("Mouse X") / Screen.width * (inverted ? -1f : 1f) * speedInEditor;
                    touchDelta.y = Input.GetAxis("Mouse Y") / Screen.height * (inverted ? -1f : 1f) * speedInEditor;
                    zoomInput = Input.GetAxis("Mouse ScrollWheel") * zoomSpeedInEditor;
                    if (Input.GetMouseButton(0) && touchDelta.magnitude > 0.05f)
                    {
                        //Mouse Button pressed    
                        oneFingerDragInput = touchDelta;
                        //Debug.Log($"TouchDelta One finger: {oneFingerDragInput}");
                    }
                    else
                    {
                        oneFingerDragInput = Vector2.zero;

                        if (Input.GetMouseButton(2) && touchDelta.magnitude > 0.05f)
                        {                        
                            //Middle Mouse Button pressed    
                            twoFingerDragInput = touchDelta * moveSpeedInEditor;
                            //Debug.Log($"TouchDelta Two fingers: {twoFingerDragInput}");
                            dragging = true;
                        }
                        else 
                        {
                            twoFingerDragInput = Vector2.zero;
                        }
                    }

                    //Use Input to set variables for later
                    //Cast against UI
                    GraphicRaycaster raycaster = GameObject.FindObjectOfType<GraphicRaycaster>();
                    PointerEventData eventData = new PointerEventData (EventSystem.current);
                    eventData.position = Input.mousePosition; 
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    raycaster.Raycast(eventData, raycastResults); 

                    hoveringOverUI = raycastResults.Count > 0;                

                    //set other variables
                    touch1PosSS = Input.mousePosition;
                    clicked = Input.GetMouseButtonUp(0);
                    moving = (touchDelta.magnitude > 0.1f) && Input.GetMouseButton(0);
                    zooming = Mathf.Abs(zoomInput) > 0.1f;      

                #elif (UNITY_IOS || UNITY_ANDROID)

                    //On single Touch Input
                    if(Input.touchCount > 0)
                    {
                        //Read Inputs
                        Vector2 touchPosition  = Input.GetTouch(0).position;
                        touch1PosSS = touchPosition;
                        touch1Pos = new Vector2(touchPosition.x / Screen.width, touchPosition.y / Screen.height);

                        Vector2 touchDeltaPosition  = Input.GetTouch(0).deltaPosition;
                        touchDelta = new Vector2(touchDeltaPosition.x / Screen.width, touchDeltaPosition.y / Screen.height);
                        touchDelta *= (inverted ? -1f : 1f) * speedOnMobile;

                        oneFingerDragInput = touchDelta;
                        Debug.Log($"TouchDelta: {touchDelta}");
                        dragging = false;
                        
                        //On two Touch Input
                        if(Input.touchCount > 1)
                        {
                            dragging = true;

                            //Read Inputs
                            Vector2 touch2Position  = Input.GetTouch(1).position;
                            touch2Pos = new Vector2(touch2Position.x / Screen.width, touch2Position.y / Screen.height);

                            Vector2 touch2DeltaPosition  = Input.GetTouch(1).deltaPosition;
                            touch2Delta = new Vector2(touch2DeltaPosition.x / Screen.width, touch2DeltaPosition.y / Screen.height);
                            touch2Delta *= (inverted ? -1f : 1f) * speedOnMobile;
                            
                            Vector2 oldCenterPos = touchCenterPos;
                            touchCenterPos = Vector2.Lerp(touch1Pos, touch2Pos, 0.5f);
                            touchCenterDelta = touchCenterPos - oldCenterPos;
                            touchCenterDelta *= (inverted ? -1f : 1f) * speedOnMobile;

                            if (!resetPastDragTouches)
                            {
                                twoFingerDragInput = touchCenterDelta * moveSpeedOnMobile;
                            }
                            else
                            {
                                twoFingerDragInput = Vector2.zero;
                            }

                            if (!resetPastZoomTouches)
                            {
                                //Read Zoom Input
                                // Find the magnitude of the vector (the distance) between the touches in each frame.
                                float prevTouchDeltaMag = (prevTouch1Pos - prevTouch2Pos).magnitude;
                                float touchDeltaMag = (touch1Pos - touch2Pos).magnitude;
                                // Find the difference in the distances between each frame.
                                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                                zoomInput = deltaMagnitudeDiff * zoomSpeedOnMobile;
                            }

                            prevTouch2Pos = touch2Pos;
                            resetPastZoomTouches = false;
                            resetPastDragTouches = false;
                        }
                        else
                        {
                            resetPastZoomTouches = true;
                            resetPastDragTouches = true;
                        }

                        prevTouch1Pos = touch1Pos;
                    }
                    else
                    {
                        resetPastZoomTouches = true;   
                        resetPastDragTouches = true;                 
                    }
                    
                    //Use Input to set variables for later

                    //On single Touch happening
                    if(Input.touchCount == 1)
                    {
                        //Cast against UI
                        GraphicRaycaster raycaster = GameObject.FindObjectOfType<GraphicRaycaster>();
                        PointerEventData eventData = new PointerEventData (EventSystem.current);
                        eventData.position = Input.GetTouch(0).position; 
                        List<RaycastResult> raycastResults = new List<RaycastResult>();
                        raycaster.Raycast(eventData, raycastResults);
                        //Debug.Log($"Raycast hit {raycastResults.Count}");
                        
                        hoveringOverUI = raycastResults.Count > 0;
                    
                        //set other variables
                        clicked = Input.GetTouch(0).phase == TouchPhase.Ended;
                        moving = Input.GetTouch(0).phase == TouchPhase.Moved;
                        zooming = false;
                        Debug.Log($"Finger 01 delta: {oneFingerDragInput}");
                    }

                    //On two Touch Input
                    if(Input.touchCount > 1)
                    {
                        //Check for touches on UI
                        hoveringOverUI = false;
                        GraphicRaycaster raycaster = GameObject.FindObjectOfType<GraphicRaycaster>();
                        PointerEventData eventData = new PointerEventData (EventSystem.current);
                        foreach (Touch touch in Input.touches)
                        {
                            eventData.position = touch.position; 
                            List<RaycastResult> raycastResults = new List<RaycastResult>();
                            raycaster.Raycast(eventData, raycastResults);
                            //Debug.Log($"Raycast hit {raycastResults.Count}");
                            if(raycastResults.Count > 0)
                            {
                                // ui touched
                                hoveringOverUI = true;
                                break;
                            }
                        }
                    
                        clicked = Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended;
                        moving = false;
                        dragging = twoFingerDragInput.magnitude > 0.05f;
                        zooming = Mathf.Abs(zoomInput) > 0.05f;                        

                        Debug.Log($"Pinch delta: {zoomInput}");
                        Debug.Log($"Pinch Movement: {twoFingerDragInput}");
                    }
            #endif

                if (hoveringOverUI)
                {
                    //Debug.Log("Hovering over UI");
                    MoveOneFinger(Vector2.zero);
                    return;
                }

                //Raycast if this Touch hasn't dragged but clicked
                if (clicked && !moving)
                {
                    interactionManager.RaycastForInteractable(touch1PosSS);
                }

                //If were not zooming, but moving one finger
                if (moving)
                {
                    MoveOneFinger(oneFingerDragInput);

                    if(GameManager.Instance.stateSelection == SelectionMode.Orbit)
                    {
                        if (!GameManager.Instance.menuManager.hudMenu.GetMovedAlready())
                        {
                            GameManager.Instance.menuManager.hudMenu.SetMovedAlready(true);
                            GameManager.Instance.menuManager.hudMenu.UpdateState();
                        }
                    }
                }
                else
                {                    
                    MoveOneFinger(Vector2.zero);
                }

                if (dragging)
                {                    
                    MoveTwoFinger(twoFingerDragInput);
                }
                else
                {
                    MoveTwoFinger(Vector2.zero);                    
                }

                if (zooming)
                {
                    //Move the Camera
                    //Vector3 translate = Quaternion.Euler(0, freelook.m_XAxis.Value, 0) * new Vector3(touchCenterDelta.x, touchCenterDelta.y, 0);
                    //translate = translate * -0.1f;
                    //if ((freelook.m_Follow.transform.localPosition + (translate)).magnitude < maxDist)
                    //{
                    //    freelook.m_Follow.transform.Translate(translate);
                    //}

                    Debug.Log($"Zoom Input: {zoomInput}");
                
                    //Zoom the camera
                    Zoom(zoomInput);
                }
                else
                {
                    //If we're neither zooming nor moving
                    Zoom(0f);               
                }

                //if (!moving)
                //{
                //    prevmag = -1f;
                //}
            }
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public virtual void MoveTwoFinger(Vector2 movement)
        {
            
        }
        public virtual void MoveOneFinger(Vector2 movement)
        {            
       
        }
        public virtual void Zoom(float amount)
        {

        }

        public void ToggleDefaultView()
        {
            int newIndex = (lastDefaultView + 1) % defaultViews.Length;
            ResetToDefault(newIndex);
        }

        public void ResetToDefault(int index = 0)
        {
            lastDefaultView = index;
            Vector2 defaultView = defaultViews[index];

            CinemachineFreeLook freeLook = (CinemachineFreeLook)virtualCam;
            freeLook.m_XAxis.Value = defaultView.x;
            freeLook.m_YAxis.Value = defaultView.y;
        }

        public void SetTarget(Transform newTarget)
        {
            Vector3 targetPos = newTarget.GetComponent<BoxCollider>().bounds.center;
            virtualCam.Follow.transform.parent.position = targetPos;
            virtualCam.Follow.transform.localPosition = Vector3.zero;
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
