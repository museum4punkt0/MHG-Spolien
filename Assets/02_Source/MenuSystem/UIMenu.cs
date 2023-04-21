using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace jn
{
    public enum UIMenuStart
    {
        activeOnStart = 0,
        inactiveOnStart = 1,
        leaveAsIs = 3
    }
    
    public class UIMenu : MonoBehaviour
    {
        // =================================================================
        #region Odin Menu Names
        static readonly string TAB_GROUP = "TabGroup";
        static readonly string REFERENCES_TAB = "References";
        static readonly string SETTINGS_TAB = "Settings";
        static readonly string ANIMATION_TAB = "Animation";
        #endregion

        #region Const Strings
        #endregion
        // =================================================================

        // =================================================================
        #region Public Fields
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]
        public UIMenuStart startBehaviour;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]
        [InfoBox("Makes the Object itself gets de-/activated after fading")]
        public bool turnOnAndOffCanvasObject = false; 
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]
        [InfoBox("If the Canvas Group should block Raycasts when visible")]
        public bool blocksRaycasts = true;               
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public CanvasGroup menuGroup;
        [InfoBox("If set, the Menu will use On, OInstant, Off & OffInstant animations")]
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]
        public Animator animator;

        [TabGroup("$TAB_GROUP", "$ANIMATION_TAB")]
        [Range(0f, 1f)]
        public float animationSpeed = 0.5f;

        [HideInInspector]
        public bool isActive = true;
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private bool initialized = false;
        private RectTransform rect;
        Tween currentTween;
        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        protected void Start()
        {
            if (!initialized) Initialize();
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods  
        protected void Initialize()
        {
            rect = GetComponent<RectTransform>();
            if (menuGroup == null)
            {
                //Debug.Log($"UIMenu {name} doesn't reference a CanvasGroup!\nTrying to get Component from {name} itself...");
                menuGroup = GetComponent<CanvasGroup>();
                if (menuGroup == null)
                {
                    Debug.LogError($"UIMenu {name} can't find any CanvasGroup!\nExecution will lead to NullRefs...");
                }
            }

            //De-/-Activate Menu into default state
            switch (startBehaviour)
            {
                case UIMenuStart.activeOnStart:
                    SetMenu(true, true);
                    break;
                case UIMenuStart.inactiveOnStart:
                    SetMenu(false, true);
                    break;
                case UIMenuStart.leaveAsIs:
                    break;
            }
            initialized = true;
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods 

        //Helper Methods for Registration within UnityEvents, as the only accept one or less parameter functions
        public void ActivateMenu()
        {
            ShowMenu(true);
        }

        public void DeActivateMenu()
        {
            HideMenu(true);
        }  
        //Helper functions end      

        public void SetInteractable(bool value)
        {
            menuGroup.interactable = value;
            menuGroup.blocksRaycasts = value? blocksRaycasts : false;
        }

        public virtual void ShowMenu(bool instant = false, bool setInteractable = true, System.Action onCmplete = null)
        {
            Debug.Log("Showing " + name + " UI Menu: " + (instant? "instant" : "fading"));

            //Before Fade
            isActive = true;
            BeforeShow();
            if(turnOnAndOffCanvasObject)
            {
                menuGroup.gameObject.SetActive(true);
            }

            //Fade
            if(!instant)
            {
                if (animator != null)
                {
                    animator.Play("On", 0);
                }
                else
                {
                    if(currentTween != null) currentTween.Complete();
                    currentTween = menuGroup.DOFade(1f, animationSpeed)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(() => OnComplete())
                        .SetUpdate(true);
                }
            } 
            else 
            {
                if (animator != null)
                {
                    animator.Play("OnInstant", 0);
                }
                else
                {
                    menuGroup.alpha = 1f;
                    OnComplete();
                }
            }

            //After Fade
            void OnComplete()
            {

                if(setInteractable)
                {
                    SetInteractable(true);
                }

                AfterShow();

                if(onCmplete != null)
                {
                    onCmplete.Invoke();
                }
            }
        }

        public void HideMenu(bool instant = false, bool setInteractable = true, System.Action onCmplete = null)
        {
            Debug.Log("Hiding " + name + " UI Menu: " + (instant? "instant" : "fading"));

            //Before Fade
            BeforeHide();

            if(setInteractable)
            {
                SetInteractable(false);
            }
            
            //Fade            
            if(!instant)
            {
                if (animator != null)
                {
                    animator.Play("Off", 0);
                }
                else
                {
                    if (currentTween != null) currentTween.Complete();
                    currentTween = menuGroup.DOFade(0f, animationSpeed)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(() => OnComplete())
                        .SetUpdate(true);
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.Play("OffInstant", 0);
                }
                else
                {
                    menuGroup.alpha = 0f;
                    OnComplete();
                }
            }

            //After fade
            void OnComplete()
            {
                isActive = false;   
                if(turnOnAndOffCanvasObject)
                {
                    menuGroup.gameObject.SetActive(false);
                }

                AfterHide();

                if(onCmplete != null)
                {
                    onCmplete.Invoke();
                }                
            }
        }

        public void SetMenu(bool On, bool instant = false)
        {
            if(On)
            {
                ShowMenu(instant);
            }
            else 
            {
                HideMenu(instant);
            }
        }

        public void UpdateMargin(float top = 0f, float bottom = 0f)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x, top);
            rect.offsetMin = new Vector2(rect.offsetMin.x, bottom);
        }
        
        protected virtual void BeforeShow(){}
        protected virtual void AfterShow(){}
        protected virtual void BeforeHide(){}
        protected virtual void AfterHide(){}

        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
