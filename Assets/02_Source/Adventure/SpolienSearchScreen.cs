using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace jn
{
    /// <summary>
    /// Basic Screen with Text and Button(s)
    /// </summary>
    /// 
    public class SpolienSearchScreen : AdventureScreen
    {
        
        public string spolieToSearch = "";
        public GameObject model;
        public Camera modelCamera;
        public TextMeshProUGUI textField;

        public override void ActivateScreen()
        {
            model.SetActive(true);
            base.ActivateScreen();
        }

        public override void DeactivateScreen()
        {
            model.SetActive(false);
            base.DeactivateScreen();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                Debug.Log(Camera.main);
                Debug.Log(Input.mousePosition);

                Ray ray = modelCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    // Check if hit.transform is door, 
                    if (hit.transform.gameObject.CompareTag("Model Interactable Spolie")){
                        Debug.Log("spolie selected" + hit.collider.gameObject.name);
                        Spolie spolie = hit.collider.gameObject.GetComponent<Spolie>();
                        if(spolieToSearch == spolie.spolieID)
                        {
                            ScreenCompleted();
                        }
                        else
                        {
                            textField.text = "Falsche Spolie, suche " + spolieToSearch;
                        }
                    }
                    else
                    {
                        Debug.Log(hit.transform.gameObject.name);
                    }
                }
            }
        }
    }
}

