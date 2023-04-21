using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace jn { 
/// <summary>
/// Superclass for all Subscreens
/// </summary>

    public class AdventureScreen : MonoBehaviour
    {

        private CanvasGroup canvasGroup;
        [SerializeField]
        private UnityEvent OnScreenActivated = new UnityEvent();
        [SerializeField]
        private UnityEvent OnScreenDeactivated = new UnityEvent();

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void ActivateScreen()
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            OnScreenActivated.Invoke();
        }

        public virtual void ScreenCompleted()
        {
            AdventureManager.Instance.CurrentStepCompleted();
        }

        public virtual void DeactivateScreen()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
            OnScreenDeactivated.Invoke();
        }
    }
}