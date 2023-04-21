using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using UI.Pagination;

namespace jn
{
    public enum ListPositionState
    {
            Full,
            Half,
            Low
    }

    public class HUDMenu : UIMenu
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
        public GameObject listItemPrefab;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public GameObject dynamicList;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public RectTransform buildingInfo;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public TextMeshProUGUI listTitle;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDButton backButton;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDButton viewDirectionButton; 
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDButton homeButton;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDButton threeDButton;        
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDButton twoDButton;        
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDButton mapButton;     
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDButton orbitButton;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public GameObject buildingStorySelection;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDButton outerViewButton;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDButton firstFloorButton;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDButton bottomFloorButton;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public HUDButton arButton;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public PagedRect gallery;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public UIMenu galleryMenu;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public Animator buildingStoryAnim;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public DraggableList draggableList;
        //[TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private List<DynamicListItem> currentItems = new List<DynamicListItem>();
        private List<DynamicListItem> selectedItems = new List<DynamicListItem>();
        //private List<int> selectedItemsOGIndex;
        private LayoutGroup listLayout;

        private Action backButtonFunction;
        private bool showingBuildingInfo;
        private Tween buildingInfoTween;

        private bool movedAlready;
        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        private void OnEnable()
        {
            GameManager.Instance.onLanguageChange += OnLanguageChange;
        }

        private void OnDisable()
        {
            GameManager.Instance.onLanguageChange -= OnLanguageChange;
        }

        private void Awake()
        {
            listLayout = dynamicList.GetComponent<LayoutGroup>();
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        private DynamicListItem GetListItemFromSpolie(Spolie spolie)
        {
            foreach(DynamicListItem dynamicListItem in currentItems)
            {
                if (dynamicListItem.IsSpolienListItem())
                {
                    if(string.Equals(spolie.spolieID, dynamicListItem.associatedSpolienData.id, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        return dynamicListItem;    
                    }
                }
            }

            return null;
        }

        private DynamicListItem GetListItemFromBuidling(MapBuilding building)
        {
            foreach(DynamicListItem dynamicListItem in currentItems)
            {
                if (dynamicListItem.IsBuildingListItem())
                {
                    if(string.Equals(building.buildingData.id, dynamicListItem.associatedBuildingData.id, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        return dynamicListItem;    
                    }
                }
            }

            return null;
        }

        private void SelectItem(DynamicListItem listItem, bool bringToTop = true)
        {
            selectedItems.Clear();

            if (bringToTop)
            {
                if(listItem.type == InteractableType.Building)
                {
                    SpolieData[] buildingSpolien = GameManager.Instance.dataManager.GetSpolienFromBuilding(listItem.associatedBuildingData);
                    
                    selectedItems.Add(listItem);

                    for(int i = 0; i < buildingSpolien.Length; i++)
                    {
                        Spolie currSpolie = GameManager.Instance.dataManager.GetSceneSpolieFromData(buildingSpolien[i]);
                        DynamicListItem spolieListItem = GetListItemFromSpolie(currSpolie);
                        selectedItems.Add(spolieListItem);

                        //First bring spolien to top
                        spolieListItem.transform.SetAsFirstSibling();
                    }
                    
                    //Then bring building to top
                    listItem.transform.SetAsFirstSibling();
                }
                
                if(listItem.type == InteractableType.Spolie)
                {
                    //Re-order Spolie Item to be the top item!
                    selectedItems.Add(listItem);
                    listItem.transform.SetAsFirstSibling();
                }

                UpdateListLayout();
            }
        }

        private void DeSelectItem(DynamicListItem listItem, bool moveBackIntoList = true)
        {
            if (moveBackIntoList)
            {
                //if(listItem.type == InteractableType.Building)
                //{
                //    foreach(DynamicListItem item in selectedItems)
                //    {
                //        item.transform.SetSiblingIndex(item.ogSiblingIndex);
                //    }
                //}


                //if(listItem.type == InteractableType.Spolie)
                //{
                //    //Re-order it to be the top item!
                //    listItem.transform.SetSiblingIndex(listItem.ogSiblingIndex);
                //}
                        

                UpdateListLayout(true);
            }

            selectedItems.Clear();
        }
        
        public void OnLanguageChange(LanguageCode lang)
        {
            if(GameManager.Instance.focusedInteractable == null)
            {
                switch (lang)
                {
                    case LanguageCode.de:
                        SwitchListLabel("Übersicht der Spolien");
                        break;
                    case LanguageCode.en:
                        SwitchListLabel("Overview of spolias");
                        break;
                }
            }
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods

        // ===== Dynamic List Things ====
        public void InitList()
        {
            if(currentItems.Count > 0)
            {
                foreach(DynamicListItem item in currentItems)
                {
                    Destroy(item.gameObject);    
                }   
            }
            currentItems.Clear();

            
            Debug.Log($"Init Dynamic List with {DataManager.dataInstance.building.Length} Buildings");
            foreach(buildingData building in DataManager.dataInstance.building)
            {
                GameObject buildingListItem = Instantiate(listItemPrefab, dynamicList.transform);
                DynamicListItem dynamicListItem = buildingListItem.GetComponent<DynamicListItem>();

                if(dynamicListItem != null)
                {
                    //Init Building
                    MapBuilding associatedBuildung = dynamicListItem.InitWithBuilding(building);

                    List<Spolie> spolienOfBuilding = GameManager.Instance.GetSpolienFromBuilding(associatedBuildung);

                    ////If this Building actually has any Spolien
                    //if(spolienOfBuilding.Count > 0)
                    //{
                        //We only actually add it to the list, when we know we want to keep the building entry
                        currentItems.Add(dynamicListItem);

                        Debug.Log($"Adding {GameManager.Instance.dataManager.sceneSpolien.Count} Spolien to {building.headline}");
                        foreach(Spolie spolie in spolienOfBuilding)
                        {
                            GameObject spolienListItem = Instantiate(listItemPrefab, dynamicList.transform);
                            DynamicListItem dynamicSpolienListItem = spolienListItem.GetComponent<DynamicListItem>();

                            if(dynamicSpolienListItem != null)
                            {
                                dynamicSpolienListItem.InitWithSpolie(spolie. data);
                                currentItems.Add(dynamicSpolienListItem);
                            }
                            else
                            {
                                Destroy(spolienListItem);
                                Debug.LogError("Instantiated Spolien List Item does not have the dynamicListItem Component! Aborting...");    
                            }
                        }
                    //}
                    //else
                    //{
                    //    //We do not list Buildings withour Spolien!
                    //    Destroy(buildingListItem);
                    //}
                }
                else
                {
                    Destroy(buildingListItem);
                    Debug.LogError("Instantiated Building List Item does not have the dynamicListItem Component! Aborting...");    
                }

            }

        }

        public void ClearList()
        {
            if(currentItems.Count > 0)
            {
                foreach(DynamicListItem item in currentItems)
                {
                    Destroy(item.gameObject);    
                }   
            }
            currentItems.Clear();
        }

        public void UpdateListLayout(bool resetListPositions = false)
        {
            if (resetListPositions)
            {
                foreach(var listItem in currentItems)
                {
                    listItem.transform.SetSiblingIndex(listItem.ogSiblingIndex);                    
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(listLayout.transform as RectTransform);

            ResetScrollable();
        }

        public void FocusBuilding(MapBuilding building)
        {
            //Find DynamicListItem of this spolie
            DynamicListItem listItem = GetListItemFromBuidling(building);

            //Make it the selected ListItem
            SelectItem(listItem, true);

            //Call function to extend it
            listItem.Extend();

            UpdateState();
        }

        public void FocusSpolie(Spolie spolie)
        {
            spolie.Focus();

            //Find DynamicListItem of this spolie
            DynamicListItem listItem = GetListItemFromSpolie(spolie);

            //Make it the selected ListItem
            SelectItem(listItem);

            //Call extend Function on it
            listItem.Extend();

            UpdateState();
        }

        public void UnfocusSelectedItem()
        {
            //Shrink Item
            selectedItems[0].Shrink();

            Hide2DGallery();

            //Make it the selected ListItem
            DeSelectItem(selectedItems[0]);
            
            UpdateState();
        }

        public void SwitchListLabel(string newLabel)
        {
            listTitle.text = newLabel;
        }

        public void ResetScrollable()
        {
            draggableList.scrollableList.normalizedPosition = Vector2.one;
        }

        // ==== Button Management =====

        public void UpdateState()
        {
            switch (GameManager.Instance.stateSelection)
            {
                case SelectionMode.Orbit:
                    orbitButton.SetActive(true);
                    mapButton.SetActive(false);
                    arButton.Show(false);                    
                    buildingStorySelection.SetActive(true);
                    ActivateViewDirectionButton();

                    ShowViewGizmo();

                    break;
                case SelectionMode.Map:
                    orbitButton.SetActive(false);
                    mapButton.SetActive(true);
                    arButton.Show(false);
                    buildingStorySelection.SetActive(false);

                    HideViewGizmo();
                    DeActivateViewDirectionButton();

                    //Hide Back Button
                    backButton.Show(false);
                    break;    
            }

            switch (GameManager.Instance.digitalTwinState)
            {
                case DigitalTwinState.Orbit:
                    ToggleVieModeButtons(false);
                    break;
                case DigitalTwinState.Map:
                    ToggleVieModeButtons(false);
                    break;
                case DigitalTwinState.SpolienFocus:
                    ToggleVieModeButtons(true);
                    twoDButton.SetActive(false);
                    threeDButton.SetActive(false);
                    HideViewGizmo();
                    buildingStorySelection.SetActive(false);
                    arButton.Show(true);
                    break;
                case DigitalTwinState.SpolienView2D:
                    ToggleVieModeButtons(true);
                    twoDButton.SetActive(true);
                    threeDButton.SetActive(false);
                    HideViewGizmo();
                    arButton.Show(true);
                    buildingStorySelection.SetActive(false);
                    break;
                case DigitalTwinState.SpolienView3D:
                    ToggleVieModeButtons(true);
                    twoDButton.SetActive(false);
                    threeDButton.SetActive(true);
                    HideViewGizmo();
                    arButton.Show(true);
                    buildingStorySelection.SetActive(false);
                    break;
                 case DigitalTwinState.BuildingFocus:
                    ToggleVieModeButtons(true);
                    twoDButton.SetActive(false);
                    threeDButton.SetActive(false);
                    HideViewGizmo();
                    arButton.Show(false);
                    break;
                case DigitalTwinState.BuildingView2D:
                    ToggleVieModeButtons(true);
                    twoDButton.SetActive(true);
                    threeDButton.SetActive(false);
                    HideViewGizmo();
                    arButton.Show(false);
                    buildingStorySelection.SetActive(false);
                    break;
                case DigitalTwinState.DetailFocus:
                    ToggleVieModeButtons(true);
                    twoDButton.SetActive(false);
                    threeDButton.SetActive(true);
                    HideViewGizmo();
                    arButton.Show(false);
                    buildingStorySelection.SetActive(false);
                    break;
            }
        }

        public void HideViewGizmo()
        {
                backButton.Show(true);
                viewDirectionButton.Show(false);            
        }

        public void ShowViewGizmo()
        {
                backButton.Show(false);
                viewDirectionButton.Show(true);
        }
        
        public void AssignBackButton(Action backButtonEvent)
        {
            backButtonFunction = backButtonEvent;
            backButton.SetInteractable(true);
            backButton.Show(true);
        }

        public void DeAssignBackButton()
        {
            backButtonFunction = null;
            backButton.SetInteractable(false);
            backButton.Show(false);
        }
        
        public void ToggleVieModeButtons(bool state)
        {
            if (state)
            {
                //Which Buttons are displayed depends on Spolie
                Spolie focusedSpolie = GameManager.Instance.GetSpolieFromCurrentlyFocusedInteractable();

                if(focusedSpolie != null)
                {
                    //We are focusing on a Spolie
                    bool is3D = focusedSpolie.data.Is3D();

                    threeDButton.Show(is3D);
                    threeDButton.SetInteractable(is3D);       

                    twoDButton.Show(state);
                    twoDButton.SetInteractable(state);
                }
                else
                {
                    //We are focusing on a Building, doesn't have 3D!
                    threeDButton.Show(false);
                    threeDButton.SetInteractable(false);       

                    twoDButton.Show(state);
                    twoDButton.SetInteractable(state);
                } 
            }
            else
            {
                threeDButton.Show(false);
                threeDButton.SetInteractable(false);       

                twoDButton.Show(false);
                twoDButton.SetInteractable(false);  
            }
        }

        public void ActivateViewDirectionButton()
        {
            viewDirectionButton.Show(true);
            viewDirectionButton.SetInteractable(true);
        }

        public void DeActivateViewDirectionButton()
        {
            viewDirectionButton.Show(false);
            viewDirectionButton.SetInteractable(false);
        }

        public void SetHomeButtonInteractable(bool state)
        {
            homeButton.SetInteractable(state);    
        }
                
        public void BackButtonClicked()
        {
            if(backButtonFunction != null)
            {
                backButtonFunction();    
            }
        }

        public void ThreeDButtonClicked()
        {
            if(GameManager.Instance.digitalTwinState == DigitalTwinState.SpolienFocus)
            {
                GameManager.Instance.LoadSpolie(ObjectMode.threeD, GameManager.Instance.GetSpolieFromCurrentlyFocusedInteractable());   
            }
            else
            {
                GameManager.Instance.Show3DView();
            }            
        }

        public void TwoDButtonClicked()
        {
            if(GameManager.Instance.digitalTwinState == DigitalTwinState.SpolienFocus)
            {
                GameManager.Instance.LoadSpolie(ObjectMode.twoD, GameManager.Instance.GetSpolieFromCurrentlyFocusedInteractable());   
            }

            if(GameManager.Instance.digitalTwinState == DigitalTwinState.SpolienView2D
                || GameManager.Instance.digitalTwinState == DigitalTwinState.SpolienView3D)
            {
                GameManager.Instance.Show2DGallery();
            } 
            
            if(GameManager.Instance.digitalTwinState == DigitalTwinState.BuildingFocus)
            {
                GameManager.Instance.Show2DGallery();                
            }
        }

        public void ViewDirectionButtonClicked()
        {
            GameManager.Instance.cameraManager.ToggleDefaultView();
        }

        public void MapButtonClicked()
        {
            GameManager.Instance.ShowMap();
        }

        public void OrbitButtonClicked()
        {
            GameManager.Instance.ShowOrbit();
            buildingStoryAnim.SetTrigger("OuterView");
        }

        public void HomeButtonClicked()
        {
            GameManager.Instance.LoadMainMenu();
        }

        public void ARButtonClicked()
        {
            GameManager.Instance.ShowARMode();
        }

        public void LoadSpolieDetails(Spolie spolie)
        {
            //Removing remaining Spolien Buttons, index start at 1. 0 is the focused spolie Item, now with details
            for(int i = 1; i < listLayout.transform.childCount; i++)
            {
                Destroy(listLayout.transform.GetChild(i).gameObject);
            }

            currentItems.Clear();

            DynamicListItem item = listLayout.transform.GetChild(0).GetComponent<DynamicListItem>();
            currentItems.Add(item);
            
            item.ShowDetails();

            ResetScrollable();
        }

        public void ShowOuterView()
        {
            GameManager.Instance.SetMuseumView(MuseumViewMode.Outer);
            buildingStoryAnim.SetTrigger("OuterView");
        }

        public void ShowFirstFloor()
        {
            GameManager.Instance.SetMuseumView(MuseumViewMode.FirstFloor);   
            buildingStoryAnim.SetTrigger("FirstFloor");         
        }

        public void ShowBottomFloor()
        {
            GameManager.Instance.SetMuseumView(MuseumViewMode.BottomFloor);  
            buildingStoryAnim.SetTrigger("BottomFloor");          
        }
        
        public void ShowBuildingLayerSelect()
        {
            buildingStoryAnim.SetBool("Show", true);
        }

        public void HideBuildingLayerSelect()
        {
            buildingStoryAnim.SetBool("Show", false);          
        }

        // ==== Gallery Management =====
        public void Show2DGallery()
        {
             galleryMenu.ShowMenu(true);
        }

        public void Hide2DGallery()
        {
             galleryMenu.HideMenu(true);
        }

        public void Init2DGallery(Interactable interactable, int pageIndex = -1)
        {
            Debug.Log("Initializing 2D Gallery");
            gallery.RemoveAllPages();

            Thumb[] thumbs = interactable.GetThumbList();
            Debug.Log($"Found {thumbs.Length} thumbs for {interactable.Name()}");

            foreach(Thumb thumb in thumbs)
            {
                if(thumb.mode == ObjectMode.twoD)
                {
                    Page page = gallery.AddPageUsingTemplate();    
                    Image pageImage = page.GetComponentInChildren<Image>();

                    pageImage.sprite = thumb.thumb;
                }
            }
            if(pageIndex != -1)
            {
                if (thumbs[0].mode == ObjectMode.twoD)
                {
                    //pageIndex += 1;
                }
                if (thumbs[pageIndex].mode == ObjectMode.twoD)
                {
                    Debug.Log(pageIndex);
                    gallery.SetCurrentPage(thumbs[0].mode == ObjectMode.twoD? pageIndex+1 : pageIndex);
                }
                
            }
            
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils

        public void SetMovedAlready(bool value)
        {
            movedAlready = value;
        }
        public bool GetMovedAlready()
        {
            return movedAlready;
        }

        public void ToggleBuildingInfo()
        {
            buildingInfo.DOKill(true);

            if (showingBuildingInfo)
            {
                HideBuildingDetails();
            }
            else
            {
                ShowBuildingDetails();
            }
        }
           
        public Tween ShowBuildingDetails()
        {
            showingBuildingInfo = true;
            return buildingInfo.DOAnchorPosX(-1530f, 0.2f)
                .SetEase(Ease.OutExpo);
        }
        
        public Tween HideBuildingDetails()
        {
            showingBuildingInfo = false;
            return buildingInfo.DOAnchorPosX(0f, 0.2f)
                .SetEase(Ease.OutExpo);
        }
        #endregion
        // =================================================================
    }
}