using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace jn
{
    public class MuseumSceneContext : SceneContext
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
        public GameObject OuterModel;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public GameObject FirstFloorModel;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public GameObject BottomFloorModel;
        //[TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
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
        public void SetMuseumView(MuseumViewMode viewMode)
        {
            Debug.Log($"Setting Museum Building View to: {viewMode}");
            switch (viewMode)
            {
                case MuseumViewMode.Outer:
                    OuterModel.SetActive(true);
                    BottomFloorModel.SetActive(false);
                    FirstFloorModel.SetActive(false);
                    break;
                case MuseumViewMode.FirstFloor:
                    OuterModel.SetActive(false);
                    BottomFloorModel.SetActive(true);
                    FirstFloorModel.SetActive(true);
                    break;
                case MuseumViewMode.BottomFloor:
                    OuterModel.SetActive(false);
                    BottomFloorModel.SetActive(true);
                    FirstFloorModel.SetActive(false);
                    break;
            }
        }

        public override IEnumerator ApplyStateRoutine(LoadCommand command)
        {
            GameManager.Instance.menuManager.HideMainMenu();
            GameManager.Instance.menuManager.ShowHUDMenu();

            //Don't Show View Mode Buttons, as nor Spolie is focused
            GameManager.Instance.menuManager.hudMenu.ToggleVieModeButtons(false);

            if(command.stateSelection == SelectionMode.Orbit)
            {
                GameManager.Instance.cameraManager.SwitchToMainOrbit();
                GameManager.Instance.digitalTwinState = DigitalTwinState.Orbit;
                GameManager.Instance.stateSelection = SelectionMode.Orbit;
                SetMuseumView(MuseumViewMode.Outer);
                GameManager.Instance.menuManager.hudMenu.UpdateState();
                GameManager.Instance.menuManager.hudMenu.SetHomeButtonInteractable(true);
                GameManager.Instance.menuManager.hudMenu.ActivateViewDirectionButton();
            }

            if(command.stateSelection == SelectionMode.Map)
            {
                GameManager.Instance.cameraManager.SwitchToMapCamera();
                GameManager.Instance.digitalTwinState = DigitalTwinState.Map;
                GameManager.Instance.stateSelection = SelectionMode.Map;
                GameManager.Instance.menuManager.hudMenu.UpdateState();
                GameManager.Instance.menuManager.hudMenu.DeAssignBackButton();
                GameManager.Instance.menuManager.hudMenu.DeActivateViewDirectionButton();
            }

            yield return null;
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
