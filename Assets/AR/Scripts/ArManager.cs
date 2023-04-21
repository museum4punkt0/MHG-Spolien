using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using jn;

public class ArManager : MonoBehaviour
{
    
    public GameObject spolie;
    public PlaceObjectsOnPlane ArSession;
    // Start is called before the first frame update
    void Start()
    {
        _init();
    }
    public void _init()
    {
        spolie = passInformation.objectToLoad;
        Debug.Log("start AR");
    }

    public void backToMenu()
    {
        try
        {
            ArSession.destroyAll();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("could not destroy Ar Session: " + e);
        }
        GameManager.Instance.menuManager.hudMenu.ShowMenu(true, true);
        GameManager.Instance.menuManager.galleryMenu.ShowMenu(true, true);
        SceneManager.UnloadSceneAsync("MHG_ArView");
    }
}