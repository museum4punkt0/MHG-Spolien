using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace jn
{
    public class Spolie : Interactable
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
        public Renderer[] renderer;

        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public Cinemachine.CinemachineVirtualCamera virtualCamera;

        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]
        [ValueDropdown("GetAllSpolien", IsUniqueList = true)]
        [FormerlySerializedAs("spolieName")]
        public string spolieID;

        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]
        public MuseumViewMode buildingStory;

        [System.NonSerialized]
        public SpolieData data; 

        public Color HiglightColor = new Color(0, 206f / 255f, 255 / 255);

        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        private MenuManager menuManager;
        private DataManager dataManager;

        private Dictionary<Renderer, Material[]> sourceMaterials = new Dictionary<Renderer, Material[]>();
        private Dictionary<Renderer, Material[]> highVisMaterial = new Dictionary<Renderer, Material[]>();
        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        private void Awake()
        {
            switchMaterial(true);
            updateHighvisMAterials();
            menuManager = GameManager.Instance.menuManager;
            dataManager = GameManager.Instance.dataManager;
            
            type = InteractableType.Spolie;

            dataManager.RegisterSpolie(this);            
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        [Button("enable Highlight")]
        public void switchToHighlight()
        {
            switchMaterial(true);
        }
        [Button("disable Highlight")]
        public void switchToNormal()
        {
            switchMaterial(false);
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Private Methods
        private static IEnumerable GetAllSpolien()
        {
            SpolienData data = DataManager.dataInstance;
            List<string> spolienNames = new List<string>();
            foreach(SpolieData spolie in data.spolien)
            {
                spolienNames.Add(spolie.id);
            }
            return spolienNames;
        }
        [Button("Update Materials")]
        private void updateHighvisMAterials()
        {
            foreach (Material[] hmats in highVisMaterial.Values)
            {
                foreach (Material mat in hmats)
                {
                    mat.color = HiglightColor;
                }
            }
        }

        private void getMaterials()
        {
            if (sourceMaterials.Count != 0) return;
            Renderer[] renderer = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderer)
            {
                Material[] SMats = r.materials;
                Material[] HMats = new Material[SMats.Length];
                sourceMaterials.Add(r, SMats);
                for (int i = 0; i < SMats.Length; i++)
                {
                    HMats[i] = new Material(SMats[i]);
                    HMats[i].name = SMats[i].name + "Highvis";
                    HMats[i].SetTexture("_MainTex", null);
                    HMats[i].color = new Color(189f/255f, 11f/255f, 65f/255);
                }
                highVisMaterial.Add(r, HMats);
            }
        }

        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public override void Interact()
        {
            Debug.Log($"Im focused {data.name}");
            
            //Focus on Spolie
            GameManager.Instance.FocusInteractable(this);
        }

        public override void UnFocus()
        {
            Debug.Log("Unfocus");
            switchMaterial(true);
        }

        public override void Focus()
        {
            switchMaterial(false);
            Debug.Log("Focus");
        }

        public void switchMaterial( bool DigitalTwinMode)
        {
            
            if (DigitalTwinMode)
            {
                getMaterials();
                foreach (KeyValuePair<Renderer, Material[]> kvp in sourceMaterials)
                {
                    kvp.Key.materials = highVisMaterial[kvp.Key];
                }
            }
            else
            {
                foreach (KeyValuePair<Renderer, Material[]> kvp in highVisMaterial)
                {
                    kvp.Key.materials = sourceMaterials[kvp.Key];
                }
            }
            
        }

        public void SwitchRenderLayer(string layer)
        {                
            foreach(Renderer r in renderer)
            r.gameObject.layer = LayerMask.NameToLayer(layer);
        }

        public override Thumb[] GetThumbList()
        {
            return data.thumbnails;
        }

        public override string Name()
        {
            return data.name;
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }
}
