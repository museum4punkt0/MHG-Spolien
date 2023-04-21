using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace jn
{
    [CreateAssetMenu(fileName = "RadioPlay Data", menuName = "MHG/Create RadioPlay Data", order = 1)]
    [System.Serializable]
    public class RadioPlayData : ScriptableObject
    {
        [SerializeField]
        public AudioClip audio;

        [SerializeField]
        public SceneData[] radioPlayScenes;

        [System.Serializable]
        public class SceneData
        {
            public string title;
            public float startTime;
            public string displayText;
        }
    }
}

