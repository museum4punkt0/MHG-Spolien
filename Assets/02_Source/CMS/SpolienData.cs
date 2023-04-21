using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace jn
{
    [CreateAssetMenu(fileName = "SpolienData", menuName = "MHG/Create Spolien Data", order = 1)]
    [System.Serializable]
    public class SpolienData : ScriptableObject
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

        [SerializeField]
        [OnValueChanged("EditedSpolien")]
        public SpolieData[] spolien;
        [SerializeField]
        [OnValueChanged("EditedSpolien")]
        public buildingData[] building;

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
        private void EditedSpolien()
        {
            Debug.Log("Saving new JSON!");
            DataManager.SaveToJSON();
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public buildingData getBuildingData(string id)
        {
            foreach(buildingData data in building)
            {
                if(data.id == id)return data;
            }
            return null;
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }


    [System.Serializable]
    public class GeneralData
    {
        [Title("General")]
        [OnValueChanged("EditedSpolien")]
        public string name;
        [OnValueChanged("EditedSpolien")]
        public string id;
    }

    [System.Serializable]
    public class SpolieData: GeneralData
    {
        [Title("Thumbnails")]
        [OnValueChanged("EditedSpolien")]
        public string thumbInfo;
        [OnValueChanged("EditedSpolien")]
        public Thumb[] thumbnails;
        [OnValueChanged("EditedSpolien")]
        public Detail[] details;
        [Title("Spolien View")]
        [OnValueChanged("EditedSpolien")]
        public string titleInfo;
        [OnValueChanged("EditedSpolien")]
        public string detailedInfo;
        [OnValueChanged("EditedSpolien")]
        public GPSPos location;

        private void EditedSpolien()
            {
                Debug.Log("Saving new JSON!");
                DataManager.SaveToJSON();
            }
    
        public bool Is3D()
        {
            foreach(Thumb thumb in thumbnails)
            {
                if(thumb.mode == ObjectMode.threeD)
                {
                    return true;    
                }    
            }
            return false;
        }
        
        public string getBuildingID()
        {
            return '#'+this.id.Trim('#').Substring(0, 3);
        }
        
    }

    [System.Serializable]
    public class buildingData: GeneralData
    {
        [OnValueChanged("EditedSpolien")]
        public Adress adress;
        [OnValueChanged("EditedSpolien")]
        public string headline;
        [Title("Thumbnails")]
        [OnValueChanged("EditedSpolien")]
        public Thumb[] thumbnails;
        [Title("Data")]
        [OnValueChanged("EditedSpolien")]
        public string titleInfo;
        [OnValueChanged("EditedSpolien")]
        public string description;
        [OnValueChanged("EditedSpolien")]
        [Tooltip("probably the architect")]
        public string creator;
        [OnValueChanged("EditedSpolien")]
        [Tooltip("How was this building destroyed?")]
        public string destruction;
        [OnValueChanged("EditedSpolien")]
        [Tooltip("did any person or group of public interest live in this building?")]
        public string user;
        [OnValueChanged("EditedSpolien")]
        public string date;

        private void EditedSpolien()
        {
            Debug.Log("Saving new JSON!");
            DataManager.SaveToJSON();
        }
    }

    [System.Serializable]
    public class Thumb
    {
        [Title("General")]
        [OnValueChanged("EditedSpolien")]
        public Sprite thumb;
        [OnValueChanged("EditedSpolien")]
        public string thumbTitle;
        [OnValueChanged("EditedSpolien")]
        public string thumbInfo;
        [OnValueChanged("EditedSpolien")]
        public ObjectMode mode;

        private void EditedSpolien()
            {
                Debug.Log("Saving new JSON!");
                DataManager.SaveToJSON();
            }
    }

    [System.Serializable]
    public class Detail
    {
        [Title("General")]
        [OnValueChanged("EditedSpolien")]
        public string id;
        [OnValueChanged("EditedSpolien")]
        public string detailTitle;

        private void EditedSpolien()
            {
                Debug.Log("Saving new JSON!");
                DataManager.SaveToJSON();
            }
    }

    [System.Serializable]
    public class Adress
    {
        public GPSPos location;
        public string what3words;
        public string Standort;
    }
}

