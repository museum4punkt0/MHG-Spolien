using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace jn
{
    public class MainMenuBootstrapper : MonoBehaviour
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
        private void Awake()
        {
            SceneManager.LoadSceneAsync("MHG_Engine", LoadSceneMode.Additive);
            SceneManager.sceneLoaded += EngineLoaded;

            void EngineLoaded(Scene scene, LoadSceneMode mode)
            {            
                SceneManager.sceneLoaded -= EngineLoaded;
                StartCoroutine(EngineLoadedRoutine());
            }
        }

        private IEnumerator EngineLoadedRoutine()
        {
            yield return null;
            //GameObject.FindObjectOfType<GameManager>().LoadMainMenu();
            GameManager.Instance.LoadMainMenu();    

            //Unload Bootstrapper scene
            SceneManager.UnloadSceneAsync(0);
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
