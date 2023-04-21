using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

namespace jn
{
    public class LabelTranslatable : ITranslatable
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
        public string gerText;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        public string engText;
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private TextMeshProUGUI textMeshLabel;

        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        private void Awake()
        {
            textMeshLabel = GetComponent<TextMeshProUGUI>();
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public override void OnLanguageChange(LanguageCode newLanguage)
        {
            if(textMeshLabel != null)
            {
                switch (newLanguage)
                {
                    case LanguageCode.de:
                        textMeshLabel.text = gerText;
                        break;
                    case LanguageCode.en:
                        textMeshLabel.text = engText;
                        break;
                }
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
