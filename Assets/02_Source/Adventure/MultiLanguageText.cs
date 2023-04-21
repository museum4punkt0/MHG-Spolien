using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handle Multilanguage Text
/// </summary>

namespace jn
{
    public class MultiLanguageText : MonoBehaviour
    {
        [SerializeField]
        private List<string> texts;

        private void Awake()
        {
            TextMeshProUGUI textField = GetComponent<TextMeshProUGUI>();
            if (textField != null)
            {
                textField.text = texts[(int)GameManager.Instance.currentLanguage];
            }
        }
        
    }
}
