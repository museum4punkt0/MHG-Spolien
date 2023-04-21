using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Serialization;

namespace jn
{
    [CreateAssetMenu(menuName = "MHG/Create Map Data File")]
    public class MapData : SingletonScriptableObject<MapData>
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
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public GPSPos upperLeftCorner;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public GPSPos lowerRightCorner;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]  
        public string mapSpritePath;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]  
        public MapSector sector;
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
        public void CreateMapDataFrom()
        {
            //string code = "0_a1";
            //string path = mapSpritePath + "/" + code + ".png";
                
            //Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
                
            //sector = new MapSector();
            //sector.sectorImage = tex;
            //sector.sectorCode = code;

            //AddSubSectorData(sector);
        }

        private void AddSubSectorData(MapSector sector)
        {
            ////Resolution of Sector coming in
            //int resolution = -10;
            //int.TryParse(sector.sectorCode.Substring(0,1), out resolution);            

            //if(resolution > 0)
            //{
            //    string newCode = $"{resolution + 1}_a1";
            //    string assetPath = mapSpritePath + "/" + newCode + ".png";

            //    if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(assetPath)))
            //    {
            //        //SubSector Images Exist!
            //        sector.AddSubSectors();
            //        for (int i = 0; i < 4; i++)
            //        {
            //            MapSector currSector = sector.subsectors[i];

            //            string subSectorCode = 
            //        }
            //    }

            //    //Column of next Sector
            //    char newSectorCharacter = sector.sectorCode[2];
            //    (char)(Convert.ToUInt16(x) + 1);
            //}
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }

    [System.Serializable]
    public class MapSector
    {

        public MapSector[] subsectors;
        public Sprite sectorImage;
        public string sectorCode;

        [Button("Add Subsectors")]
        public void AddSubSectors()
        {
            subsectors = new MapSector[4];
        }
    }
}
