using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace jn
{
    public class InteractionManager : MonoBehaviour
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
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]
        public LayerMask interactableMask;
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
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
        public void RaycastForInteractable(Vector2 screenTouchPos)
        {
            if(GameManager.Instance.digitalTwinState == DigitalTwinState.SpolienView3D
                ||GameManager.Instance.digitalTwinState == DigitalTwinState.SpolienView2D)
            {
                return;    
            }

            Ray ray = GameManager.Instance.cameraManager.currCamController.associatedCam.ScreenPointToRay(new Vector3(screenTouchPos.x, screenTouchPos.y, 0f));
            Debug.DrawRay(ray.origin, ray.direction * 10000f, Color.red, 1f, true);
            
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 10000f, interactableMask))
            {
                Debug.Log("Hit Interactable");
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if(interactable != null)
                {
                    interactable.Interact();    
                }
                else
                {
                    Debug.LogWarning("Hit Collider on Interactable Layer, but no Interactable Script present!\nABORTING...");                    
                }
            }
            else
            {
                Debug.Log("Cast didnt Hit in Interactable Layer");    
            }
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
