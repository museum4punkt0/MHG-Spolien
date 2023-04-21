using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace jn
{
    public abstract class SceneContext : MonoBehaviour
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
        public abstract IEnumerator ApplyStateRoutine(LoadCommand command);
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
