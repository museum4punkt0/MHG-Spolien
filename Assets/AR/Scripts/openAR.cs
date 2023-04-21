using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class openAR : MonoBehaviour
{
    public void loadObjectToAR(GameObject prefab)
    {
        passInformation.objectToLoad = prefab;
        Debug.Log("Opening " + prefab.name);
        SceneManager.LoadScene("MHG_ArView", LoadSceneMode.Additive);
    }
}
