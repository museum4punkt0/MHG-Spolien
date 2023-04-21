using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObjectsOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;

    [SerializeField]
    [Tooltip("Script that is responsible for getting the right Prefab. Probably on a Gameobject named ARManager")]
    ArManager manager;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }

    /// <summary>
    /// Invoked whenever an object is placed in on a plane.
    /// </summary>
    public static event Action onPlacedObject;

    ARRaycastManager m_RaycastManager;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    
    [SerializeField]
    int m_MaxNumberOfObjectsToPlace = 1;

    int m_NumberOfPlacedObjects = 0;

    [SerializeField]
    bool m_CanReposition = true;

    public bool canReposition
    {
        get => m_CanReposition;
        set => m_CanReposition = value;
    }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        Debug.LogWarning("Start Place Object");
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Debug.LogWarning("touch");
            if (touch.phase == TouchPhase.Began)
            {
                
                if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = s_Hits[0].pose;
                    m_PlacedPrefab = manager.spolie;

                    if (m_NumberOfPlacedObjects < m_MaxNumberOfObjectsToPlace)
                    {
                        //Quaternion rot = hitPose.rotation;
                        Vector3 t = Camera.current.transform.position- hitPose.position;
                        Vector3 n = Vector3.ProjectOnPlane(t, new Vector3(0, 1, 0));
                        placeObject(hitPose.position, Quaternion.LookRotation(n));
                        m_NumberOfPlacedObjects++;
                    }
                    else
                    {
                        if (m_CanReposition)
                        {
                            Vector3 t = Camera.current.transform.position - hitPose.position;
                            Vector3 n = Vector3.ProjectOnPlane(t, new Vector3(0, 1, 0));

                            spawnedObject.transform.SetPositionAndRotation(hitPose.position, Quaternion.LookRotation(n));
                            foreach (Transform child in spawnedObject.transform)
                            {
                                child.gameObject.layer = 11;
                            }
                        }
                    }
                    
                    if (onPlacedObject != null)
                    {
                        onPlacedObject();
                    }
                }
            }
        }
    }

    private void placeObject(Vector3 position, Quaternion rotation)
    {
        manager._init();
        m_PlacedPrefab = manager.spolie;
        manager.ArSession = this;
        spawnedObject = Instantiate(m_PlacedPrefab, position, rotation);
        spawnedObject.gameObject.layer = 11;
        spawnedObject.GetComponentInChildren<jn.Spolie>().switchToNormal();
        Transform[] children = spawnedObject.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            child.gameObject.layer = 11;
        }
    }

    public void destroyAll()
    {
        if(spawnedObject != null)
        {
            DestroyImmediate(spawnedObject);
        }
        
    }
}
