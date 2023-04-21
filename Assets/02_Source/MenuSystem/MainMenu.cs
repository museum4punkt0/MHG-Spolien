using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace jn
{
    public class MainMenu : UIMenu
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
        public void ResetMainMenuButtons()
        {
            foreach(Button button in GetComponentsInChildren<Button>())
            {
                button.interactable = true;
            }
        }

        public void PlayDigitalTwinMode()
        {
            LoadCommand command = new LoadCommand();
            command.state = GameMode.Museum;
            command.stateSelection = SelectionMode.Orbit;
            GameManager.Instance.LoadScene(command);
        }

        public void PlayMapMode()
        {
            LoadCommand command = new LoadCommand();
            command.state = GameMode.Museum;
            command.stateSelection = SelectionMode.Map;
            GameManager.Instance.LoadScene(command);
        }

        public void PlayAdventureMode()
        {
            LoadCommand command = new LoadCommand();
            command.state = GameMode.Adventure;
            command.stateSelection = SelectionMode.Orbit;
            GameManager.Instance.LoadScene(command);
        }

        public void LoadJSON()
        {
            
        }

        public void Exit()
        {
            Application.Quit();
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
