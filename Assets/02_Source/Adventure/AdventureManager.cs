using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEngine.EventSystems;
#endif


namespace jn
{
    /// <summary>
    /// AdventureManager class Singleton; Toplevel Manager for adventure game
    /// </summary>
    public class AdventureManager : Singleton<AdventureManager>
    {
        public int currentGameStep { get; private set; } = 0;
        public List<AdventureScreen> adventureScreens;


        [SerializeField]
        private Button continueButton;

      
        private void Start()
        {

            //For development purpose, add EventSystem if none is in scene (if app is started directly in Adventure Scene)
#if UNITY_EDITOR
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if(eventSystem == null)
            {
                GameObject g = new GameObject("EventSystem");
                g.AddComponent<EventSystem>();
                g.AddComponent<StandaloneInputModule>();
            }
#endif


            LoadSaveGame();
            //Show welcome Screen with resume / start selection
            adventureScreens[0].ActivateScreen();

        }

        /// <summary>
        /// Load Savegame from player prefs if availible
        /// </summary>
        private void LoadSaveGame()
        {
            currentGameStep = PlayerPrefs.GetInt("SavedAdventureGameStep", 0);
            if (currentGameStep > 0)
            {
                // Savegame found -> Enable continie button
                continueButton.gameObject.SetActive(true);
            }
            else
            {
                //No Savegame or first screen not completed -> Disable continiue button
                continueButton.gameObject.SetActive(false);
            }
        }

        public void OnWelcomeScreenSelected(bool shouldContinue = false)
        {
            if (!shouldContinue) { currentGameStep = 0;}
            else {
                currentGameStep--; // Quick fix, current Step will be increased again in ShowNextScreen
                adventureScreens[0].DeactivateScreen();
            } 
            ShowNextScreen();
        }


        /// <summary>
        /// Called by AdventureScreen, if current step is completed
        /// </summary>
        public void CurrentStepCompleted()
        {
            ShowNextScreen();
        }

        private void ShowNextScreen()
        {
            adventureScreens[currentGameStep].DeactivateScreen();
            NextCurrentStep();
            if(adventureScreens.Count > currentGameStep)
            {
                adventureScreens[currentGameStep].ActivateScreen();
            }
        }

        /// <summary>
        /// Increase current step and save to PlayerPrefs
        /// </summary>
        private void NextCurrentStep()
        {
            currentGameStep++;
            PlayerPrefs.SetInt("SavedAdventureGameStep", currentGameStep);
        }
    }
}


