using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;

namespace jn
{
    public class MapBuilding : Interactable
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
        public SpriteRenderer sprite;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public Sprite spolienMarker;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public Sprite buildingMarker;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public float size = 5f;
        public bool hasSpolien = false;
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        //private Spolie associatedSpolie;
        public buildingData buildingData;
        private float unselectedSize;
        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        private void Awake()
        {
            unselectedSize = size;
        }
        private void Update()
        {
            //Constant Size in Screen Space
            // has been relocated to seperate script, so it can be used independant
            /*
            Camera mapCam = GameManager.Instance.cameraManager.mapDragCamera.associatedCam;

            float worldScreenHeight = mapCam.orthographicSize * 2;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
         
            transform.localScale = new Vector3(
                worldScreenWidth * size,
                worldScreenWidth * size,
                1);
            */
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public void Initialize(buildingData data, bool hasSpolien = false)
        {
            buildingData = data;
            type = InteractableType.Building;
            sprite.sprite = hasSpolien ? spolienMarker : buildingMarker;
        }
        
        public override void Interact()
        {
            Debug.Log($"Map Selected on Building {buildingData.headline}, ID: {buildingData.id}");

            //Focus on Spolie
            GameManager.Instance.FocusInteractable(this);
        }

        public override void UnFocus()
        {
            //throw new System.NotImplementedException();
            DOTween.Kill(true);
            DOTween.To(() => size, s => size = s, unselectedSize, 0.25f)
                .SetEase(Ease.OutExpo);
        }
        public override void Focus()
        {
            //Debug.Log("Opening Dynamic List Item for Building");
            //throw new System.NotImplementedException();
            DOTween.Kill(true);
            DOTween.To(() => size, s => size = s, unselectedSize * 1.5f, 0.25f)
                .SetEase(Ease.OutExpo);
        }

        public override Thumb[] GetThumbList()
        {
            return buildingData.thumbnails;
        }

        public override string Name()
        {
            return buildingData.headline;
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
