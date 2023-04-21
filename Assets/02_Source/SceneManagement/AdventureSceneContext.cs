using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jn
{
    public class AdventureSceneContext : SceneContext
    {
        public override IEnumerator ApplyStateRoutine(LoadCommand command)
        {
            GameManager.Instance.menuManager.HideMainMenu();
            GameManager.Instance.menuManager.ShowHUDMenu();

            //Don't Show View Mode Buttons, as nor Spolie is focused
            GameManager.Instance.menuManager.hudMenu.ToggleVieModeButtons(false);
/*
            if (command.stateSelection == SelectionMode.Orbit)
            {
                GameManager.Instance.cameraManager.SwitchToMainOrbit();
                GameManager.Instance.digitalTwinState = DigitalTwinState.Orbit;
                GameManager.Instance.menuManager.hudMenu.UpdateState();
                GameManager.Instance.menuManager.hudMenu.SetHomeButtonInteractable(true);
                GameManager.Instance.menuManager.hudMenu.ActivateViewDirectionButton();
            }

            if (command.stateSelection == SelectionMode.Map)
            {
                GameManager.Instance.cameraManager.SwitchToMapCamera();
                GameManager.Instance.digitalTwinState = DigitalTwinState.Map;
                GameManager.Instance.menuManager.hudMenu.UpdateState();
                GameManager.Instance.menuManager.hudMenu.DeAssignBackButton();
                GameManager.Instance.menuManager.hudMenu.DeActivateViewDirectionButton();
            }
            
*/
            yield return null;
        }
    }
}

