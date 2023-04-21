using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

namespace jn
{
    public class ModeButton : MonoBehaviour
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
        public Image thumbImage;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public TextMeshProUGUI modeLabel;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public TextMeshProUGUI modeDescription;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public Button button;
        //[TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private ObjectMode mode;
        private Interactable interactable;
        private int index;

        private InteractableType type;
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
        public void InitializeModeButton(Thumb thumb, ObjectMode mode, Interactable interactable, int index)
        {
            modeDescription.text = thumb.thumbInfo;
            modeLabel.text = thumb.thumbTitle;
            
            thumbImage.sprite = thumb.thumb;
            this.mode = mode;
            this.interactable = interactable;
            this.index = index;

            if (interactable.GetType().Equals(typeof(Spolie)))
            {
                type = InteractableType.Spolie;
            }
            else
            {
                type = InteractableType.Building;                
            }

            button.onClick.AddListener(LoadMode);
        }

        public void LoadMode()
        {
            if(GameManager.Instance.digitalTwinState == DigitalTwinState.SpolienFocus)
            {
                //GameManager.Instance.LoadSpolie(ObjectMode.twoD, (Spolie)GameManager.Instance.focusedInteractable);  
                if(type == InteractableType.Spolie)
                {
                    GameManager.Instance.LoadSpolie(mode, (Spolie)interactable, index);
                }
            }

            if(GameManager.Instance.digitalTwinState == DigitalTwinState.BuildingFocus)
            {
                //GameManager.Instance.LoadSpolie(ObjectMode.twoD, (Spolie)GameManager.Instance.focusedInteractable);  
                if(type == InteractableType.Building)
                {
                    GameManager.Instance.LoadBuilding(mode, (MapBuilding)interactable, index);
                }
            }

            
            if(GameManager.Instance.digitalTwinState == DigitalTwinState.SpolienView2D
               || GameManager.Instance.digitalTwinState == DigitalTwinState.SpolienView3D)
            {
                if(mode == ObjectMode.twoD)
                {
                    GameManager.Instance.Show2DGallery(index);
                    Debug.Log(index);
                }
                else
                {
                    GameManager.Instance.Show3DView();                    
                }
            } 

            if(GameManager.Instance.digitalTwinState == DigitalTwinState.BuildingView2D)
            {
                if(mode == ObjectMode.twoD)
                {
                    GameManager.Instance.Show2DGallery(index);
                    Debug.Log($"2D View requested for Building {((MapBuilding)interactable).buildingData.headline}");
                }
                else
                {
                    Debug.LogError($"A 3D View was requested for Building {((MapBuilding)interactable).buildingData.headline}, this is not supported!");                
                }
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
