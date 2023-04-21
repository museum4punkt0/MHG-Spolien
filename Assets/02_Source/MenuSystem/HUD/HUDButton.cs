using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace jn
{
    public class HUDButton : MonoBehaviour
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
        public Button button;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public Sprite activeSprite;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public Sprite inActiveSprite;
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
        public void Show(bool state)
        {
            gameObject.SetActive(state);
        }

        public void SetInteractable(bool interactable)
        {
            button.interactable = interactable;
        }

        public void SetActive(bool active)
        {
            button.image.sprite = active? activeSprite : inActiveSprite;
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
