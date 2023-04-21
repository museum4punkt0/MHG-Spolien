using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;

namespace jn
{
    public class CameraManager : MonoBehaviour
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
        public GuidReference mainOrbitCamRef;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public GuidReference spolienOrbitCamRef;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public GuidReference mapCamRef;
        //[TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   

        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        [System.NonSerialized]
        public CameraController currCamController;
        private CinemachineVirtualCameraBase currVirtCam;
        private CameraController mainOrbitCam
        {
            get
            {
                if(_mainOrbitCam == null)
                {
                    _mainOrbitCam = mainOrbitCamRef.gameObject.GetComponent<CameraController>();
                }
                return _mainOrbitCam;
            }
        }
        private CameraController _mainOrbitCam;
        private CameraController spolienOrbitCam
        {
            get
            {
                if(_spolienOrbitCam == null)
                {
                    _spolienOrbitCam = spolienOrbitCamRef.gameObject.GetComponent<CameraController>();
                }
                return _spolienOrbitCam;
            }
        }
        private CameraController _spolienOrbitCam;
        public CameraController mapDragCamera
        {
            get
            {
                if(_mapDragCamera == null)
                {
                    _mapDragCamera = mapCamRef.gameObject.GetComponent<CameraController>();
                }
                return _mapDragCamera;
            }
        }
        private CameraController _mapDragCamera;
        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods

        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public void TurnOffAllCameras()
        {
            mainOrbitCam.interactable = false;
            spolienOrbitCam.interactable = false;
            mapDragCamera.interactable = false;
        }

        public void SwitchToSpolienOrbit()
        {
            mainOrbitCam.interactable = false;
            spolienOrbitCam.interactable = true;
            mapDragCamera.interactable = false;
            SwitchToCamera(spolienOrbitCam.virtualCam, spolienOrbitCam);
            spolienOrbitCam.associatedCam.depth = 100;
            mainOrbitCam.associatedCam.depth = 0;
            mapDragCamera.associatedCam.depth = 0;
        }

        public void SwitchToMainOrbit()
        {
            mainOrbitCam.interactable = true;
            spolienOrbitCam.interactable = false;
            mapDragCamera.interactable = false;
            SwitchToCamera(mainOrbitCam.virtualCam, mainOrbitCam);
            spolienOrbitCam.associatedCam.depth = 0;
            mainOrbitCam.associatedCam.depth = 100;
            mapDragCamera.associatedCam.depth = 0;
        }

        public void SwitchToMapCamera()
        {
            mainOrbitCam.interactable = false;
            spolienOrbitCam.interactable = false;
            mapDragCamera.interactable = true;
            SwitchToCamera(mapDragCamera.virtualCam, mapDragCamera);
            spolienOrbitCam.associatedCam.depth = 0;
            mainOrbitCam.associatedCam.depth = 0;
            mapDragCamera.associatedCam.depth = 100;
        }

        public void SwitchToCamera(CinemachineVirtualCameraBase virtCam, CameraController newController)
        {
            if(currVirtCam != null)
            {
                currVirtCam.Priority = 0;    
            }
            
            virtCam.Priority = 100;
            currVirtCam = virtCam;
            currCamController = newController;
        }

        public void SwitchSpolienTarget(Spolie spolie)
        {
            spolienOrbitCam.SetTarget(spolie.transform);
            ((OrbitCamera)spolienOrbitCam).setToDefaultView(spolie.transform.rotation.eulerAngles.y+180);
            ((OrbitCamera)spolienOrbitCam).setFreeLookSize(spolie.renderer[0].bounds.size);
            SwitchToCamera(spolienOrbitCam.virtualCam, currCamController);
        }

        public void FoucsSpolieInOrbit(Spolie spolie)
        {
            GameManager.Instance.SetMuseumView(spolie.buildingStory);

            CinemachineVirtualCamera virtualCamera = spolie.virtualCamera;
            
            SwitchToCamera(virtualCamera, currCamController);
            
            spolie.SwitchRenderLayer("SpolienCamFocus");
        }

        public void FoucsBuildingInMap(MapBuilding mapBuilding)
        {
            CinemachineVirtualCamera virtualCamera = mapDragCamera.virtualCam as CinemachineVirtualCamera;
            virtualCamera.Follow.position = mapBuilding.transform.position;
            virtualCamera.Follow.Translate(0f, -1f, 0f);

            ((DraggableCamera)mapDragCamera).SetZoomExternally(3.6f);
        }

        public void UnfocusSpolie()
        {
            SwitchToCamera(mainOrbitCam.virtualCam, currCamController);

            GameManager.Instance.GetSpolieFromCurrentlyFocusedInteractable().SwitchRenderLayer("Interactable");
        }
       
        public void ResetView()
        {
            ((OrbitCamera) mainOrbitCam).ResetToDefault(0);
        }

        public void ToggleDefaultView()
        {
            ((OrbitCamera) mainOrbitCam).ToggleDefaultView();            
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
