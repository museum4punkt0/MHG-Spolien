using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace jn
{
    public class Sector : MonoBehaviour
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
        public Transform subSectorTransform;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public SpriteRenderer graphic;
        //[TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        
        [System.NonSerialized]
        public int depth = 0;
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private Sector parentSector;
        private Sector[] childSectors;
        public MapManager mapManager;
        [System.NonSerialized]
        public MapSector mapSector;       
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
        public void SetGraphic()
        {
            graphic.sprite = mapSector.sectorImage;
        }

        public void Initialize(Sector parentSec, Quadrant quadrant, MapManager map, int depth)
        {
            parentSector = parentSec;
            mapManager = map;
            this.depth = depth + 1;

            float length = parentSec.GetBounds2D().size.x;

            Vector3 offset = new Vector3();
            switch (quadrant)
            {
                case Quadrant.initial:
                    break;
                case Quadrant.upperL:
                    offset = new Vector3(- length / 4,  length / 4, 0f);
                    break;
                case Quadrant.upperR:
                    offset = new Vector3(  length / 4,  length / 4, 0f);
                    break;
                case Quadrant.lowerL:
                    offset = new Vector3(- length / 4,- length / 4, 0f);
                    break;
                case Quadrant.lowerR:
                    offset = new Vector3(  length / 4,- length / 4, 0f);
                    break;
            }

            transform.position = parentSec.transform.position + offset;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            SetGraphic();
        }

        public void Cull()
        {
            graphic.enabled = false;
        }

        public void UnCull()
        {
            graphic.enabled = true;
        }

        public Sector[] Slice()
        {
            Sector[] res = new Sector[4];

            for(int i = 1; i <= 4; i++)
            {
                GameObject subSector = Instantiate(mapManager.sectorPrefab, subSectorTransform);
                Debug.Log("Instancing new Sector!");
                Sector newSector = subSector.GetComponent<Sector>();
                Quadrant quadrant = (Quadrant) i;

                if(mapSector.subsectors != null)
                {
                    newSector.mapSector = mapSector.subsectors[i - 1]; 
                }

                newSector.Initialize(this, quadrant, mapManager, depth);

                res[i - 1] = newSector;
            }                

            childSectors = res;
            return res;
        }

        public void MergeThis()
        {
            parentSector.MergeChildren();
        }

        public void MergeChildren()
        {
            //Actual deletion and merging process happens in MapManager
            childSectors = null;
        }

        public Bounds GetBounds2D()
        {
            Bounds bounds = graphic.bounds;
            bounds.size = new Vector3(bounds.size.x, bounds.size.y, Mathf.Infinity);
            return bounds;
        }

        public Sector[] GetChildSectors()
        {
            return childSectors;
        }

        public Sector GetParentSector()
        {
            return parentSector;
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }

    public enum Quadrant
        {
            initial = 0,
            upperL = 1,
            upperR = 2, 
            lowerL = 3,
            lowerR = 4
        }
}
