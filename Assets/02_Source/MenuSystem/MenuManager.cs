using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace jn
{
    public class MenuManager : MonoBehaviour
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
        public Canvas mainCanvas;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public SpolienMenu spolienMenu;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public MainMenu mainMenu;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDMenu hudMenu;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public UIMenu splashScreen;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public UIMenu galleryMenu;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public GameObject loadingScreen;
        //[TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private DataManager dataManager;
        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        private void Start()
        {
            dataManager = GameObject.FindObjectOfType<DataManager>();
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public void ShowHUDMenu()
        {
            hudMenu.InitList();    
            hudMenu.ShowMenu();
        }
       
        public void HideHUDMenu()
        {
            hudMenu.HideMenu();    
        }

        public void HideMainMenu()
        {
            mainMenu.HideMenu(false, true);
        }

        public void ShowMainMenu()
        {
            mainMenu.ShowMenu(false, true);
        }

        public void ShowLoadingScreen()
        {
            loadingScreen.SetActive(true);
        }

        public void HideLoadingScreen()
        {
            loadingScreen.SetActive(false);
        }

        public void CloseSplashScreen()
        {
            Debug.Log("Pressed button to disable splashscreen");
            splashScreen.HideMenu(false, true, () => ShowMainMenu());
            GameManager.Instance.splashScreenShown = true;
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
