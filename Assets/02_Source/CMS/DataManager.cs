using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace jn
{
    public class DataManager : ITranslatable
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
        private static SpolienData _dataInstance;
        public static SpolienData dataInstance
        {

            get{
                if(_dataInstance == null)
                {
                    //TODO: Uncomment this for language usage!
                    switch (GameManager.Instance.currentLanguage)
                    {
                        case LanguageCode.de:
                            _dataInstance = Resources.Load("SpolienData_DE") as SpolienData;
                    break;
                            case LanguageCode.en:
                                _dataInstance = Resources.Load("SpolienData_EN") as SpolienData;
                    break;
                    }   
                }   
                if(_dataInstance == null)
                    {
                        Debug.LogError("Data JSON/Asset couldn't be loaded from Resources!");
                    }
                return _dataInstance; 
            }
        }

        [ReadOnly]
        public List<Spolie> sceneSpolien = new List<Spolie>();
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
        public static void SaveToJSON()
        {
                string resourcesPath = Application.dataPath + "/Resources/";

                string data = JsonUtility.ToJson(dataInstance, true);
                System.IO.File.WriteAllText(resourcesPath + "/SpolienDataSAVE.json", data);
        }

        public static void LoadJSON()
        {
            Debug.LogWarning("Loading JSON not implemented yet");
                //string resourcesPath = Application.dataPath + "/Resources/";
                //string spolienDataJSON = System.IO.File.ReadAllText(resourcesPath + "/SpolienDataSAVE.json");
                //SpolienData data = JsonUtility.FromJson<SpolienData>(spolienDataJSON);

        }

        public SpolieData GetSpolieByID(string id)
        {
            SpolienData data = dataInstance;
            try
            {
                SpolieData found = data.spolien.First(s => string.Equals(s.id, id, System.StringComparison.OrdinalIgnoreCase));
                return found;
            } catch (System.Exception e) 
            {
                Debug.LogError($"A Spolie was searched by id {id}, that doesnt exist! Returning null...\n {e.Message}");
                return null;
            }
        }

        public Spolie GetSceneSpolieFromData(SpolieData data)
        {
            foreach(Spolie spolie in sceneSpolien)
            {
                //Debug.Log($"Testing {spolie.spolieName} & {data.name}");
                if(string.Equals(spolie.spolieID, data.id, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return spolie;
                }
            }
            return null;

            //return sceneSpolien.First(s => string.Equals(s.spolieName, data.name, System.StringComparison.InvariantCultureIgnoreCase));
        }

        public void RegisterSpolie(Spolie spolie)
        {
            //Save SpolienData from JSON in each of the spolien
            SpolieData data = GetSpolieByID(spolie.spolieID);
            spolie.data = data;
            sceneSpolien.Add(spolie);

            //Sort dem
            sceneSpolien.Sort((s1, s2) => 
            {
                int s1ID;
                int.TryParse(s1.data.id, out s1ID);
                
                int s2ID;
                int.TryParse(s2.data.id, out s2ID);

                return s1ID.CompareTo(s2ID);
            });
        }

        public void UnRegisterAllSpolien()
        {
            sceneSpolien.Clear();
        }

        public buildingData GetBuildingFromSpolie(SpolieData spolieData)
        {
            //TODO: Implement this
            //Building id aus spolien id generieren
            //Building suchen
            string buildingID = spolieData.id.Substring(1, 3);
            for(int i = 0; i < dataInstance.building.Length; i++)
            {
                buildingData buildingData = dataInstance.building[i];
                if(string.Equals(buildingData.id.Substring(1, 3), buildingID))
                {
                    return buildingData;
                }
            }

            Debug.LogError($"Building with ID {buildingID} couldn't be found was searched for via Spolie {spolieData.id}!");
            return null;
        }

        public SpolieData[] GetSpolienFromBuilding(buildingData building)
        {
            //TODO: Implement this
            //Search through spolien and return all, that begin with building ID

            List<SpolieData> resultingSpolien = new List<SpolieData>();

            for(int i = 0; i < dataInstance.spolien.Length; i++)
            {
                SpolieData spolieData = dataInstance.spolien[i];
                if(spolieData.id.Length > 4)
                {
                    string buildingID = spolieData.id.Substring(1, 3);

                    if(string.Equals(building.id.Substring(1, 3), buildingID))
                    {
                        resultingSpolien.Add(spolieData);
                    }
                }
            }

            if(resultingSpolien.Count == 0) Debug.LogWarning($"Building with ID {building.id} couldn't find any Spolien associated with it!");

            return resultingSpolien.ToArray();
        }

        public override void OnLanguageChange(LanguageCode lang)
        {
            Debug.Log("Switched Language on Spolien data");
            _dataInstance = null;
            //This should reload SpolienData, in the correct lang as soon as it is accessed again
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
