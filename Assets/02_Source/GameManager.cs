using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace jn
{
    /// <summary>
    /// Game Manager class Singleton, that holds all the necessary References,
    /// that could globally be needed.
    /// </summary>
    public class GameManager : Singleton<GameManager>
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
        public MenuManager menuManager {
            get {
                if(_menuManager == null)
                    {
                       _menuManager = gameObject.GetComponent<MenuManager>();
                    }
                return _menuManager;
            }
        }

        public DataManager dataManager {
            get {
                if(_dataManager == null)
                    {
                       _dataManager = gameObject.GetComponent<DataManager>();                        
                    }
                return _dataManager;
            }
        }

        public InteractionManager interactionManager {
            get {
                if(_interactionManager == null)
                    {
                       _interactionManager = gameObject.GetComponent<InteractionManager>();                        
                    }
                return _interactionManager;
            }
        }

        public CameraManager cameraManager {
            get {
                if(_cameraManager == null)
                    {
                       _cameraManager = gameObject.GetComponent<CameraManager>();                        
                    }
                return _cameraManager;
            }
        }

        public MapManager mapManager{
            get {
                if(_mapManager == null)
                    {
                       _mapManager = FindObjectOfType<MapManager>();                        
                    }
                return _mapManager;
            }
        }
        
        public GameMode state;

        public LanguageCode currentLanguage {
            get {
                    return _currentLanguage;
                }
            private set{
                    _currentLanguage = value;
                    if(onLanguageChange != null) onLanguageChange.Invoke(_currentLanguage);

                switch (_currentLanguage)
                {
                    case LanguageCode.de:
                        germanButton.SetActive(true);
                        englishButton.SetActive(false);
                        break;
                    case LanguageCode.en:
                        germanButton.SetActive(false);
                        englishButton.SetActive(true);
                        break;
                }
            }
        }

        public GameObject englishButton;
        public GameObject germanButton;

        private LanguageCode _currentLanguage;
        public delegate void OnLanguageChange(LanguageCode lang);
        public OnLanguageChange onLanguageChange;

        public SelectionMode stateSelection;
        public ObjectMode stateObject;

        public DigitalTwinState digitalTwinState;
        
        [System.NonSerialized]
        public Interactable focusedInteractable;
        [System.NonSerialized]
        public ObjectMode spolieMode;
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private MenuManager _menuManager;
        private DataManager _dataManager;
        private InteractionManager _interactionManager;
        private CameraManager _cameraManager;
        private MapManager _mapManager;

        private SceneContext currentSceneContext;

        [System.NonSerialized]
        public bool splashScreenShown = false;
        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        private void Awake()
        {
            Debug.Log($"System Language detected as: {Application.systemLanguage}");
            switch (Application.systemLanguage)
            {
                case SystemLanguage.German:
                    currentLanguage = LanguageCode.de;
                    break;
                case SystemLanguage.English:
                    currentLanguage = LanguageCode.en;
                    break;
                default:
                    break;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                currentLanguage = LanguageCode.de;
                Debug.Log("Switching to German");
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                currentLanguage = LanguageCode.en;
                Debug.Log("Switching to English");
            }
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        private void UnloadMainMenu()
        {
            try 
            {
                Debug.Log("unloading main Menu");
                //Unload Main Menu Scene
                SceneManager.UnloadSceneAsync(1);                 
            }
            catch
            {}
        }

        private void UnloadMuseum()
        {
            try 
            {
                //Unload Main Menu Scene
                SceneManager.UnloadSceneAsync(2);                 
            }
            catch
            {}
        }

        private void UnloadAdventure()
        {
            try 
            {
                //Unload Main Menu Scene
                SceneManager.UnloadSceneAsync(3);                 
            }
            catch
            {}
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public void LoadScene(LoadCommand command)
        {
            if(state != command.state)
            {
                //Load new Scene, wait for async
                if(command.state == GameMode.Museum)
                {
                    Debug.Log("switching now to Museum Now");
                    //Unload Main Menu Scene
                    UnloadMainMenu();
                    //Unload Adventure Scene
                    UnloadAdventure();
                    menuManager.ShowLoadingScreen();
                    
                    //Load Museum Scene
                    SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
                    SceneManager.sceneLoaded += AfterLoadMuseum;

                    void AfterLoadMuseum(Scene scene, LoadSceneMode mode)
                    {
                        Debug.Log("loading finished");
                        menuManager.HideLoadingScreen();
                        SceneManager.sceneLoaded -= AfterLoadMuseum;
                        
                        //Finish GameManager Changes
                        SceneManager.SetActiveScene(scene);
                        state = GameMode.Museum;
                        digitalTwinState = DigitalTwinState.Orbit;

                        InitSceneContext();
                    }
                }

                if(command.state == GameMode.MainMenu)
                { 
                    menuManager.hudMenu.Hide2DGallery();
                    menuManager.hudMenu.ClearList();
                    dataManager.UnRegisterAllSpolien();

                    //Unload Museum Scene
                    UnloadMuseum();
                    //Unload Adventure Scene
                    UnloadAdventure();
                    
                    //Load Main Menu Scene
                    SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive); 
                    SceneManager.sceneLoaded += AfterLoadMainMenu; 

                    void AfterLoadMainMenu(Scene scene, LoadSceneMode mode)
                    {
                        SceneManager.sceneLoaded -= AfterLoadMainMenu;
                        
                        //Finish GameManager Changes
                        SceneManager.SetActiveScene(scene);
                        state = GameMode.MainMenu;

                        InitSceneContext();
                    }
                }

                if(command.state == GameMode.Adventure)
                {
                    Debug.Log("switching now to Adventure Now");
                    //Unload Main Menu Scene
                    UnloadMainMenu();
                    //Unload Adventure Scene
                    UnloadMuseum();
                    menuManager.ShowLoadingScreen();

                    //Load Museum Scene
                    SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
                    SceneManager.sceneLoaded += AfterLoadAdventure;

                    void AfterLoadAdventure(Scene scene, LoadSceneMode mode)
                    {
                        Debug.Log("loading finished");
                        menuManager.HideLoadingScreen();
                        SceneManager.sceneLoaded -= AfterLoadAdventure;

                        //Finish GameManager Changes
                        SceneManager.SetActiveScene(scene);
                        state = GameMode.Adventure;
                        digitalTwinState = DigitalTwinState.Orbit;

                        InitSceneContext();
                    }
                }

                void InitSceneContext()
                {
                    //Init Language
                    currentLanguage = currentLanguage; //Triggers callsbacks

                    //Load With Scene Context
                    currentSceneContext = GameObject.FindObjectOfType<SceneContext>();

                    if(currentSceneContext != null) 
                    {
                        StartCoroutine(currentSceneContext.ApplyStateRoutine(command));
                    }
                    else
                    {
                        Debug.LogError("Couldn't find SceneContext in this Scene! Skipping Scene Innintialization...");    
                    }
                }
            }
        }
        
        public void FocusInteractable(Interactable interactable)
        {
            Spolie spolie = interactable as Spolie;
            MapBuilding building = interactable as MapBuilding;

            //Are we interacting with a spolie?
            if(spolie != null)
            {
                if(interactable == focusedInteractable)
                {
                    if(interactable is Spolie)
                    {
                        LoadSpolie(ObjectMode.threeD, (Spolie)interactable);
                    }
                    return;
                }

                if(focusedInteractable != null)
                {
                    UnFocusInteractable(focusedInteractable);    
                }

                //Debug.Log($"Focusing on {interactable.name}");
                focusedInteractable = interactable;
                interactable.Focus();

                Debug.Log($"Focusing on Spolie {spolie.data.name}");
                digitalTwinState = DigitalTwinState.SpolienFocus;

                //Camera fixed to Spolie
                cameraManager.FoucsSpolieInOrbit(spolie);
                cameraManager.FoucsBuildingInMap(GetMapBuildingFromSpolie(spolie));

                //Dynamic List Item updates to Mode Selection
                menuManager.hudMenu.FocusSpolie(spolie);

                //Back Button leaves Spolien Focus
                menuManager.hudMenu.AssignBackButton(() => UnFocusInteractable(interactable));    
            }

            //Are we interacting with a building?   
            if(building != null)
            {
                //if(interactable == focusedInteractable)
                //{
                //    if(interactable is MapBuilding)
                //    {
                //        LoadBuilding(ObjectMode.twoD, (MapBuilding)interactable);
                //    }
                //    return;
                //}

                if(focusedInteractable != null)
                {
                    UnFocusInteractable(focusedInteractable);    
                }

                Debug.Log($"Focusing on Building {building.buildingData.headline}");
                
                focusedInteractable = interactable;
                interactable.Focus();

                digitalTwinState = DigitalTwinState.BuildingFocus;

                //Camera fixed to Spolie
                cameraManager.FoucsBuildingInMap(building);
                    
                //GameManager.Instance.GetSpolieFromCurrentlyFocusedInteractable().SwitchRenderLayer("SpolienCamFocus");

                menuManager.hudMenu.FocusBuilding(building);

                //Back Button leaves Spolien Focus
                menuManager.hudMenu.AssignBackButton(() => UnFocusInteractable(interactable));    

            }

            if(stateSelection == SelectionMode.Map)
            {
                menuManager.hudMenu.draggableList.ChangeListState(ListPositionState.Half);
            }            

        }

        public void UnFocusInteractable(Interactable interactable)
        {
            Debug.Log($"Un-focusing Interactable");

            interactable.UnFocus();
            if(interactable.type == InteractableType.Spolie)
            {
                //Camera fixed to Spolie
                cameraManager.UnfocusSpolie();
            }

            //Going Back from Focus, we either go to Map or Orbit, depening what was before
            if(stateSelection == SelectionMode.Map)
            {
                digitalTwinState = DigitalTwinState.Map;
                
                //Dynamic List Item deletes extended banner content
                menuManager.hudMenu.UnfocusSelectedItem(); //Also hides Back Button
                
                //Deactivate View Mode Buttons
                menuManager.hudMenu.ToggleVieModeButtons(false);

                focusedInteractable = null;
                menuManager.hudMenu.OnLanguageChange(currentLanguage);
            }
            else
            {
                Debug.Log("BACK");
                digitalTwinState = DigitalTwinState.Orbit;   
                

                //Dynamic List Item deletes extended banner content
                menuManager.hudMenu.UnfocusSelectedItem();

                //Deactivate View Mode Buttons
                menuManager.hudMenu.ToggleVieModeButtons(false);

                //menuManager.hudMenu.AssignBackButton(() => ResetView());
                menuManager.hudMenu.ShowViewGizmo();

                focusedInteractable = null;
                menuManager.hudMenu.OnLanguageChange(currentLanguage);
            }

            menuManager.hudMenu.draggableList.ChangeListState(ListPositionState.Low);
        }

        public void LoadSpolie(ObjectMode mode, Spolie spolie, int index = 0)
        {
            if(digitalTwinState == DigitalTwinState.SpolienView2D
                || digitalTwinState == DigitalTwinState.SpolienView3D)
            {
                return;
            }

            spolieMode = mode;

            //Start Spolien Cam & Orbit
            GameManager.Instance.cameraManager.SwitchSpolienTarget(spolie);
            GameManager.Instance.cameraManager.SwitchToSpolienOrbit();

            //Only Show this Spolie, with grey BG & Vignette            
            //Camera fixed to Spolie
            cameraManager.FoucsSpolieInOrbit(spolie);

            //Activate View Mode Toggle
            menuManager.hudMenu.ToggleVieModeButtons(true);
            
            //Assign BackButton to Unload Spolie
            menuManager.hudMenu.AssignBackButton(() => UnLoadSpole());
            //menuManager.hudMenu.AssignBackButton(() => UnFocusInteractable(spolie));

            //Load new Dynamic List with Details
            menuManager.hudMenu.LoadSpolieDetails(spolie);
            
            //Set GameState
            if(mode == ObjectMode.threeD)
            {
                Debug.Log($"Init 3D view for Spolie {spolie.data.name}");
                digitalTwinState = DigitalTwinState.SpolienView3D;

                Show3DView();
            }
            else
            {
                Debug.Log($"Init 2D view for Spolie {spolie.data.name}");
                digitalTwinState = DigitalTwinState.SpolienView2D;

                Show2DGallery();
            }
        }

        public void LoadBuilding(ObjectMode mode, MapBuilding building, int index = 0)
        {
            if(digitalTwinState == DigitalTwinState.SpolienView2D
                || digitalTwinState == DigitalTwinState.SpolienView3D)
            {
                return;
            }

            spolieMode = mode;

            //Start Spolien Cam & Orbit
            //GameManager.Instance.cameraManager.SwitchSpolienTarget(building);
            //GameManager.Instance.cameraManager.SwitchToSpolienOrbit();

            //Only Show this Spolie, with grey BG & Vignette            
            //Camera fixed to Spolie
            //cameraManager.FoucsSpolieInOrbit(building);

            //Activate View Mode Toggle
            menuManager.hudMenu.ToggleVieModeButtons(true);
            
            //Assign BackButton to Unload Spolie
            menuManager.hudMenu.AssignBackButton(() => UnLoadBuilding());
            
            //Set GameState
            Debug.Log($"Init 2D view for Spolie {building.buildingData.headline}");
            digitalTwinState = DigitalTwinState.BuildingView2D;

            Show2DGallery();
        }

        public void UnLoadSpole()
        {
            Debug.Log($"Return to Spolien Focus {focusedInteractable.Name()}!");

            //Start Main Cam & Orbit
            if(stateSelection == SelectionMode.Orbit)
            {
                if(focusedInteractable is Spolie) 
                {
                    Spolie spolie = (Spolie) focusedInteractable;
                    //Go back to Orbit Camera, as this show the Spolie if only in focus
                    GameManager.Instance.cameraManager.SwitchToMainOrbit();
                    //Camera fixed to Spolie
                    cameraManager.FoucsSpolieInOrbit(spolie);

                    digitalTwinState = DigitalTwinState.SpolienFocus;

                    menuManager.hudMenu.AssignBackButton(() => UnFocusInteractable(focusedInteractable)); 
                    menuManager.hudMenu.InitList();
                    menuManager.hudMenu.FocusSpolie(spolie);
                }
            }
            else if(stateSelection == SelectionMode.Map)
            {                
                //Go back to Map Camera

                Spolie spolie = (Spolie) focusedInteractable;
                //Go back to Map Camera
                GameManager.Instance.cameraManager.SwitchToMapCamera();

                digitalTwinState = DigitalTwinState.SpolienFocus;

                menuManager.hudMenu.AssignBackButton(() => UnFocusInteractable(focusedInteractable)); 
                menuManager.hudMenu.InitList();
                menuManager.hudMenu.FocusSpolie(spolie);
            }

            //Reset DynamicList
            //menuManager.hudMenu.InitList();
            menuManager.hudMenu.Hide2DGallery();
            menuManager.hudMenu.UpdateState();
        }

        public void UnLoadBuilding()
        {
            Debug.Log($"Unload Building View!");

            //UnFocusInteractable(focusedInteractable);

            //Start Main Cam & Orbit
            if(stateSelection == SelectionMode.Orbit)
            {
                //Go back to Orbit Camera
                GameManager.Instance.cameraManager.SwitchToMainOrbit();

                menuManager.hudMenu.AssignBackButton(() => UnFocusInteractable(focusedInteractable));
                //Reset DynamicList 
                menuManager.hudMenu.InitList();
                menuManager.hudMenu.FocusBuilding((MapBuilding)focusedInteractable);

                digitalTwinState = DigitalTwinState.BuildingFocus;
            }
            else if(stateSelection == SelectionMode.Map)
            {                
                //Go back to Map Camera
                GameManager.Instance.cameraManager.SwitchToMapCamera();
                GameManager.Instance.cameraManager.FoucsBuildingInMap((MapBuilding) focusedInteractable);

                menuManager.hudMenu.AssignBackButton(() => UnFocusInteractable(focusedInteractable)); 
                //Reset DynamicList
                menuManager.hudMenu.InitList();
                menuManager.hudMenu.FocusBuilding((MapBuilding)focusedInteractable);
                
                digitalTwinState = DigitalTwinState.BuildingFocus;
            }

            menuManager.hudMenu.Hide2DGallery();
            menuManager.hudMenu.UpdateState();
        }
        
        public void ToggleSelectionMode()
        {
            Debug.Log("Toggling Selection Mode");
            if(stateSelection == SelectionMode.Orbit)
            {
                ShowMap();
            }
            else
            {
                Show3DView();
            }
        }

        public void ToggleViewMode()
        {
            Debug.Log("Toggling ViewMode");
            if(digitalTwinState == DigitalTwinState.SpolienView2D)
            {
                Show3DView();
            }
            else
            {
                Show2DGallery();
            }
        }

        public void Show3DView()
        {
            digitalTwinState = DigitalTwinState.SpolienView3D;
            menuManager.hudMenu.Hide2DGallery();
            menuManager.hudMenu.UpdateState();
            menuManager.hudMenu.draggableList.ChangeListState(ListPositionState.Low);
        }

        public void Show2DGallery(int page = 0)
        {
            //Load 2D Gallery View
            if(focusedInteractable.type == InteractableType.Spolie)
            {
                digitalTwinState = DigitalTwinState.SpolienView2D;
                //menuManager.hudMenu.draggableList.ChangeListState(ListPositionState.Low);
            }

            if(focusedInteractable.type == InteractableType.Building)
            {
                digitalTwinState = DigitalTwinState.BuildingView2D;
            }

            menuManager.hudMenu.Init2DGallery(focusedInteractable, page);
            menuManager.hudMenu.Show2DGallery();
            menuManager.hudMenu.UpdateState();
        }

        public void ShowMap()
        {
            //if(focusedInteractable != null)
            //{
            //    UnFocusInteractable(focusedInteractable);
            //}

            if(focusedInteractable != null)
            {
                if(focusedInteractable is Spolie)
                {
                    menuManager.hudMenu.InitList();
                    menuManager.hudMenu.FocusSpolie((Spolie) focusedInteractable);

                    if(digitalTwinState == DigitalTwinState.SpolienView2D
                        || digitalTwinState == DigitalTwinState.SpolienView3D)
                    {
                        UnLoadSpole();
                    }
                    
                    digitalTwinState = DigitalTwinState.SpolienFocus;
                    stateSelection = SelectionMode.Map;
                }

                if(focusedInteractable is MapBuilding)
                {
                    if(digitalTwinState == DigitalTwinState.BuildingView2D)
                    {
                        UnLoadBuilding();
                    }

                    digitalTwinState = DigitalTwinState.BuildingFocus;
                    stateSelection = SelectionMode.Map;                    
                }
            }
            else
            {            
                digitalTwinState = DigitalTwinState.Map; 
                stateSelection = SelectionMode.Map;     
            }
            
            cameraManager.SwitchToMapCamera();
            menuManager.hudMenu.UpdateState();
        }

        public void ShowOrbit()
        {
            if (focusedInteractable != null)
            {
                if(focusedInteractable is Spolie)
                {
                    if(digitalTwinState == DigitalTwinState.SpolienView2D
                        || digitalTwinState == DigitalTwinState.SpolienView3D)
                    {
                        UnLoadSpole();
                    }

                    digitalTwinState = DigitalTwinState.SpolienFocus;  
                    cameraManager.SwitchToMainOrbit();   
                    cameraManager.FoucsSpolieInOrbit((Spolie)focusedInteractable); 
                }

                if(focusedInteractable is MapBuilding)
                {
                    if(digitalTwinState == DigitalTwinState.BuildingView2D)
                    {
                        UnLoadBuilding();
                    }

                    digitalTwinState = DigitalTwinState.BuildingFocus;  
                    cameraManager.SwitchToMainOrbit();  
                    SetMuseumView(MuseumViewMode.Outer);
                }
                
                //UnFocusInteractable(focusedInteractable);
            }
            else
            {
                digitalTwinState = DigitalTwinState.Orbit;  
                cameraManager.SwitchToMainOrbit();
            }

            //digitalTwinState = DigitalTwinState.Orbit;
            //menuManager.hudMenu.InitList();
            stateSelection = SelectionMode.Orbit;
            menuManager.hudMenu.UpdateState();
            //ResetView();
        }

        public void LoadMainMenu()
        {
            LoadCommand command = new LoadCommand();
            command.state = GameMode.MainMenu;
            GameManager.Instance.LoadScene(command);
        }
        
        public void ResetView()
        {
            menuManager.hudMenu.SetMovedAlready(false);
            cameraManager.ResetView();
            menuManager.hudMenu.UpdateState();
        }
        
        public void SetMuseumView(MuseumViewMode viewMode)
        {
            MuseumSceneContext museumSceneContext = currentSceneContext as MuseumSceneContext;

            if(museumSceneContext != null)
            {
                museumSceneContext.SetMuseumView(viewMode);
            }
        }
        
        
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        public MapBuilding GetMapBuildingFromSpolie(Spolie spolie)
        {
            return mapManager.sceneBuildings.Find(m => m.buildingData.id.Equals(dataManager.GetBuildingFromSpolie(spolie.data).id));
        }

        public Spolie GetSpolieFromCurrentlyFocusedInteractable()
        {
            Spolie spolie = focusedInteractable as Spolie;

            //if(spolie == null) 
            //{
            //    spolie = GetFirstSpolieFromBuilding((MapBuilding) focusedInteractable);    
            //}

            return spolie;
        }

        public Spolie GetFirstSpolieFromBuilding(MapBuilding building)
        {
            SpolieData spolieData = _dataManager.GetSpolienFromBuilding(building.buildingData)[0];
            Spolie sceneSpolie = _dataManager.GetSceneSpolieFromData(spolieData);
            
            return sceneSpolie;
        }

        public List<Spolie> GetSpolienFromBuilding(MapBuilding building)
        {
            Debug.Log($"Trying to find Spolien of Building {building.buildingData.headline}");
            List<Spolie> result = dataManager.sceneSpolien.FindAll(s => s.data.getBuildingID() == building.buildingData.id);

            return result;
        }
        
        public void ShowARMode()
        {
            Debug.Log("Starting AR Mode");
            menuManager.hudMenu.HideMenu(true, true);
            menuManager.galleryMenu.HideMenu(true, true);
            GetComponent<openAR>().loadObjectToAR(GetSpolieFromCurrentlyFocusedInteractable().gameObject);
        }
        
        public void SetLanguageGerman()
        {
            currentLanguage = LanguageCode.de;              
        }

        public void SetLanguageEnglish()
        {
            currentLanguage = LanguageCode.en;              
        }

        public void ToggleLanguage()
        {
            if(currentLanguage == LanguageCode.de)
            {
                currentLanguage = LanguageCode.en;    
                return;
            }

            if(currentLanguage == LanguageCode.en)
            {
                currentLanguage = LanguageCode.de;    
                return;
            }
        }
        #endregion
        // =================================================================
    }


    public enum GameMode
    {
        Empty,
        MainMenu,
        Museum,
        Adventure,
    }

    public enum LanguageCode
    {
        de = 0,
        en = 1
    }

    public enum SelectionMode
    {
        Orbit,
        Map
    }

    public enum ObjectMode
    {
        threeD,
        twoD
    }

    public enum DigitalTwinState
    {
        Orbit,
        Map,
        SpolienFocus,
        BuildingFocus,
        BuildingView2D,
        SpolienView2D,
        SpolienView3D,
        DetailFocus
    }

    public enum MuseumViewMode
    {
        Outer,
        FirstFloor,
        BottomFloor
    }

    public enum InteractableType
    {
        Spolie,
        Building
    }

    public struct LoadCommand 
    {
        public GameMode state;
        public SelectionMode stateSelection;
        public ObjectMode stateObject;
    }
}