using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

namespace jn
{
    public class DynamicListItem : MonoBehaviour
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
        public RectTransform smallBanner;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]  
        public RectTransform extendedBanner;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]  
        public RectTransform buildingBanner;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public TextMeshProUGUI nameLabelSmallBanner;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public TextMeshProUGUI nameLabelBuildingBanner;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public TextMeshProUGUI infoLabel;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public Button buildingListItemButton;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public Button listItemButton;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]  
        public GameObject modeButtonPrefab;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public LayoutGroup modesList;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public GameObject detailsGo;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public TextMeshProUGUI detailInfoLabel;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public GameObject detailList;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public GameObject detailListPrefab;
        //[TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   

        public InteractableType type;
        [System.NonSerialized]
        public SpolieData associatedSpolienData;
        [System.NonSerialized]
        public buildingData associatedBuildingData;
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private Spolie associatedSpolie;
        private MapBuilding associatedBuilding;

        public int ogSiblingIndex;
        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        private void Update()
        {
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public Spolie InitWithSpolie(SpolieData data)
        {
            Debug.Log($"Init Spolien List Item for {data.name}");
            type = InteractableType.Spolie;

            extendedBanner.gameObject.SetActive(false);
            smallBanner.gameObject.SetActive(true);
            buildingBanner.gameObject.SetActive(false);

            associatedSpolienData = data;

            //Setup the Name & Make it clickable to focus on a Spolie
            nameLabelSmallBanner.text = data.name;

            associatedSpolie = GameManager.Instance.dataManager.GetSceneSpolieFromData(data);
            listItemButton.onClick.AddListener(() => GameManager.Instance.FocusInteractable(associatedSpolie));
        
            ogSiblingIndex = transform.GetSiblingIndex();

            return associatedSpolie;    
        }

        public MapBuilding InitWithBuilding(buildingData data)
        {
            Debug.Log($"Init Building List Item for {data.headline}");
            type = InteractableType.Building;
            
            extendedBanner.gameObject.SetActive(false);
            smallBanner.gameObject.SetActive(false);
            buildingBanner.gameObject.SetActive(true);

            associatedBuildingData = data;

            //Setup the Name & Make it clickable to focus on a building
            nameLabelBuildingBanner.text = data.headline;

            associatedBuilding = GameManager.Instance.mapManager.sceneBuildings.Find(m => m.buildingData.id == data.id);
            //listItemButton.onClick.AddListener(() => GameManager.Instance.FocusInteractable(associatedBuilding));
            buildingListItemButton.onClick.AddListener(() => GameManager.Instance.FocusInteractable(associatedBuilding));
            
            ogSiblingIndex = transform.GetSiblingIndex();

            return associatedBuilding;    
        }

        public void Extend()
        {
            //Extend Transforms
            RectTransform rect = gameObject.transform as RectTransform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 1800f);

            if(type == InteractableType.Spolie)
            {
                //Add addtional Data & Buttons
                GameManager.Instance.menuManager.hudMenu.SwitchListLabel(associatedSpolienData.name);
                infoLabel.text = associatedSpolienData.titleInfo;
                extendedBanner.gameObject.SetActive(true);
                smallBanner.gameObject.SetActive(false);
                buildingBanner.gameObject.SetActive(false);
            
                RefreshTextTransforms();

                int i = 0;
                foreach(Thumb thumb in associatedSpolienData.thumbnails)
                {
                    GameObject modeButton = Instantiate(modeButtonPrefab, modesList.transform);
                    ModeButton mode = modeButton.GetComponent<ModeButton>();
                
                    mode.InitializeModeButton(thumb, thumb.mode, associatedSpolie,  i);
                    i++;
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(modesList.transform as RectTransform);    
            }

            if(type == InteractableType.Building)
            {
                //Add addtional Data & Buttons
                GameManager.Instance.menuManager.hudMenu.SwitchListLabel(associatedBuildingData.headline);
                infoLabel.text = associatedBuildingData.description;
                extendedBanner.gameObject.SetActive(true);
                smallBanner.gameObject.SetActive(false);
                buildingBanner.gameObject.SetActive(false);       
                detailsGo.SetActive(false);
            
                RefreshTextTransforms();

                int i = 0;
                foreach(Thumb thumb in associatedBuildingData.thumbnails)
                {
                    GameObject modeButton = Instantiate(modeButtonPrefab, modesList.transform);
                    ModeButton mode = modeButton.GetComponent<ModeButton>();
                
                    mode.InitializeModeButton(thumb, thumb.mode, associatedBuilding,  i);
                    i++;
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(modesList.transform as RectTransform);   
            }

            //Update List
            GameManager.Instance.menuManager.hudMenu.UpdateListLayout();
        }

        /// <summary>
        /// Creates list of Details for Spolie, this is not called for Buildings
        /// </summary>
        public void ShowDetails()
        {
            //Extend Transforms
            //RectTransform rect = gameObject.transform as RectTransform;
            //rect.sizeDelta = new Vector2(rect.sizeDelta.x, 720f);

            //Add addtional Data & Buttons          
            detailsGo.SetActive(true);
            detailInfoLabel.text = associatedSpolienData.detailedInfo;
            
            foreach(Detail detail in associatedSpolienData.details)
            {
                GameObject detailListItem = Instantiate(detailListPrefab, detailList.transform);

                DetailListItem listItem = detailListItem.GetComponent<DetailListItem>();
                listItem.Init(detail);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(detailList.transform as RectTransform);

            //Update List
            GameManager.Instance.menuManager.hudMenu.UpdateListLayout();

            //Write Spolienview Texts
            infoLabel.text = associatedSpolienData.titleInfo;
            //Check For Size of Label, then use autolayout
            detailInfoLabel.text = associatedSpolienData.detailedInfo;

            RefreshTextTransforms();
        }

        public void Shrink()
        {
            //Shrink Transforms
            RectTransform rect = gameObject.transform as RectTransform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 100f);

            //Active small Banner
            extendedBanner.gameObject.SetActive(false);
            if(type == InteractableType.Spolie)
            {
                smallBanner.gameObject.SetActive(true);
            }
            if(type == InteractableType.Building)
            {
                buildingBanner.gameObject.SetActive(true);
            }

            for(int i = 0; i < modesList.transform.childCount; i++)
            {
                GameObject modeButton = modesList.transform.GetChild(i).gameObject;
                
                Destroy(modeButton);
            }
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        public bool IsBuildingListItem()
        {
            return associatedBuildingData != null;    
        }

        public bool IsSpolienListItem()
        {
            return associatedSpolienData != null;    
        }

        public void RefreshTextTransforms()
        {
            foreach(TextSizer sizer in this.GetComponentsInChildren<TextSizer>())
            {
                sizer.Refresh();
            }    
        }
        #endregion
        // =================================================================
    }
}
