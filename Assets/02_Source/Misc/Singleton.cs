using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace jn
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
		public static T Instance { get; private set; }
	
		public virtual void OnEnable ()
		{
			Debug.Log($"{gameObject.name} Singleton initializing...");
			if (Instance == null) {
				Instance = this as T;
				DontDestroyOnLoad (this);
			} else {
				Destroy (gameObject);
			}
		}
    }
}
