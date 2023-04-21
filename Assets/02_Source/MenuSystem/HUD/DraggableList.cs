using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;

namespace jn
{
    public class DraggableList : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
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
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public RectTransform hudParent;           
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public ScrollRect scrollableList;
        //[TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private bool dragging;
        private float deltaY;
        
        private float dragSpeed;
        private float lastYPos;

        private ListPositionState posState;
        private List<float> listPositions = new List<float>{0f, -940f, -1680f};
        private float _anchoredPositionY;
        [HideInInspector]
        public float anchoredPositionY 
        { 
            get{ return _anchoredPositionY; } 
            set 
            {
                updateAnchoredPos(value);
                _anchoredPositionY = value;
            } 
        }
        private UIMenu galleryMenu;
        private MenuManager menuManager;

        private ListPositionState currPos = ListPositionState.Low;
        private ListPositionState prevPos = ListPositionState.Low;
        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        private void Start()
        {
            galleryMenu = GameManager.Instance.menuManager.hudMenu.galleryMenu;
            menuManager = GameObject.FindObjectOfType<MenuManager>();
            hudParent.sizeDelta = new Vector2 (hudParent.rect.size.x, ((RectTransform)(menuManager.mainCanvas.transform)).sizeDelta.y - 250f);

            InitializeListPositions();

            ((RectTransform)scrollableList.transform).sizeDelta = new Vector2(((RectTransform)scrollableList.transform).sizeDelta.x, listPositions[2] * -1f);
            AnimateHUD(listPositions[2]);
        }

        private void Update()
        {
            if(hudParent.anchoredPosition.y < -620)
            {                
                GameManager.Instance.menuManager.hudMenu.ShowBuildingLayerSelect();
            }
            else
            {
                GameManager.Instance.menuManager.hudMenu.HideBuildingLayerSelect();                    
            }
        }

        private void InitializeListPositions()
        {
            float lowestBannerHeight = 200f;
            float lowestPosition = -1f * (((RectTransform)(menuManager.mainCanvas.transform)).sizeDelta.y - lowestBannerHeight);
            
            //float mediumBannerHeight = 1250f;
            float mediumPosition = lowestPosition / 2f;//-1f * (((RectTransform)(menuManager.mainCanvas.transform)).sizeDelta.y - mediumBannerHeight);

            listPositions = new List<float>{-250f, mediumPosition, lowestPosition};
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            float newPosY = Mathf.Min(hudParent.anchoredPosition.y + eventData.delta.y, 0f);
            newPosY = Mathf.Max(newPosY, listPositions[2]);
            anchoredPositionY = newPosY;
            deltaY += eventData.delta.y;

            dragSpeed = (eventData.position.y - lastYPos) * Time.deltaTime * Screen.dpi * 100f;
            Debug.Log($"Drag Speed {dragSpeed}");
            lastYPos = eventData.position.y;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            float closestAnchor = ClosestTo(listPositions, hudParent.anchoredPosition.y);
            AnimateHUD(closestAnchor);
            deltaY = 0f;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CycleListSize();
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        private void SetCurrPos(float newPos)
        {
            prevPos = currPos;
            currPos = (ListPositionState)listPositions.IndexOf(newPos);
            Debug.Log($"List Position is now: {currPos}");
        }

        public void CycleListSize()
        {
            switch (currPos)
            {
                case ListPositionState.Low:
                    ChangeListState(ListPositionState.Half);
                    break;
                case ListPositionState.Half:
                    //if(prevPos == ListPositionState.Full)
                    //{
                        ChangeListState(ListPositionState.Low);
                    //    break;
                    //}
                    //if(prevPos == ListPositionState.Low)
                    //{
                    //    ChangeListState(ListPositionState.Full);
                    //    break;                        
                    //}
                    break;
                case ListPositionState.Full:
                    ChangeListState(ListPositionState.Half);
                    break;
            }
        }

        private float ClosestTo(IEnumerable<float> collection, float target)
        {
            if(dragSpeed > 75f)
            {
                Debug.Log("Gesture Up");
                var list = collection.OrderBy(x => target - x);
                return list.First();
            }

            if(dragSpeed < -75f)
            {
                Debug.Log("Gesture Down");
                var list = collection.OrderBy(x => target - x);
                return list.ElementAt(1);
            }

            return collection.OrderBy(x => Mathf.Abs(target - x)).First();
        }

        private void AnimateHUD(float newYPos)
        {
            SetCurrPos(newYPos);

            hudParent.DOAnchorPosY(newYPos, .15f)
                .SetEase(Ease.OutExpo)
                .OnUpdate(() => updateAnchoredPos(hudParent.anchoredPosition.y));

            switch (currPos)
            {
                case ListPositionState.Full:
                    ((RectTransform)scrollableList.transform).sizeDelta = new Vector2(((RectTransform)scrollableList.transform).sizeDelta.x, listPositions[2] * -1f);
                    break;
                case ListPositionState.Half:
                    ((RectTransform)scrollableList.transform).sizeDelta = new Vector2(((RectTransform)scrollableList.transform).sizeDelta.x, listPositions[1] * -1f);
                    break;
                case ListPositionState.Low:
                    ((RectTransform)scrollableList.transform).sizeDelta = new Vector2(((RectTransform)scrollableList.transform).sizeDelta.x, listPositions[0] * -1f);
                    break;
            }

        }

        private void updateAnchoredPos(float pos)
        {
            // im not proud about this implementation, might be usefull to rewrite AnimateHUD so it doesn´t use the DOTween.
            // for now this works.
            if(hudParent.anchoredPosition.y!= pos)
            {
                hudParent.anchoredPosition = new Vector2(0f, pos);
            }
            if(pos<= listPositions[1])
            {
                galleryMenu.UpdateMargin(0, pos - listPositions[listPositions.Count - 1]);
            }
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public void ChangeListState(ListPositionState state)
        {
            switch (state)
            {
                case ListPositionState.Low:
                    AnimateHUD(GetListPos(state));
                    break;
                case ListPositionState.Half:
                    AnimateHUD(GetListPos(state));      
                    break;
                case ListPositionState.Full:
                    AnimateHUD(GetListPos(state));
                    break;
                default:
                    break;
            }

            posState = state;
        }

        public float GetListPos(ListPositionState state)
        {
            switch (state)
            {
                case ListPositionState.Low:
                    return listPositions[2];
                    break;
                case ListPositionState.Half:
                    return listPositions[1];
                    break;
                case ListPositionState.Full:
                    return listPositions[0];
                    break;
                default:
                    return listPositions[2];
                    break;
            }
        }

        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}