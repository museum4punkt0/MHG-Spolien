using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Linq;

namespace jn
{
    public class MapManager : MonoBehaviour
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
        public GameObject buildingMapPrefab;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]
        public GameObject gpsMapPrefab;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public GameObject sectorPrefab;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public Camera mapDragCam;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public Transform sectorTransform;
        [TabGroup("$TAB_GROUP", "$REFERENCES_TAB")]   
        public Transform buildingsTransform;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]   
        [Range(0f, 1f)]
        public float sliceFactor = 0.1f;
        [TabGroup("$TAB_GROUP", "$SETTINGS_TAB")]  
        public MapData mapData;

        [System.NonSerialized]
        public List<MapBuilding> sceneBuildings = new List<MapBuilding>();
        #endregion
        // =================================================================

        // =================================================================
        #region Private Fields
        public List<Sector> activeSectors;
        private int mapTreeDepth = 0;
        private int currDepth;
        private Bounds mapWorldBounds;
        private GameObject gpsLocator;
        private LocationServiceProvider locationServiceProvider;
        private int[] buildingsWithSpolien;

        #endregion
        // =================================================================

        // =================================================================
        #region Unity Methods
        private void Awake()
        {
            GameObject subSector = Instantiate(sectorPrefab, sectorTransform);
            Sector newSector = subSector.GetComponent<Sector>();
            newSector.mapManager = this;
            newSector.mapSector = mapData.sector;
            newSector.SetGraphic();
            newSector.depth = 0;
            activeSectors.Add(newSector);

            mapWorldBounds = newSector.GetBounds2D();

            bool atEnd = false;
            MapSector currSec = mapData.sector;
            while (!atEnd)
            {
                if(mapData.sector.subsectors != null)
                {
                    try
                    {
                        currSec = currSec.subsectors[0];
                        mapTreeDepth++;
                    }
                    catch(IndexOutOfRangeException e)
                    {          
                        atEnd = true;
                    }
                }
                else
                {
                    atEnd = true;    
                }
            }

            locationServiceProvider = GetComponent<LocationServiceProvider>();
            locationServiceProvider.updatedGPS += UpdateGPS;
            locationServiceProvider.getGPSLocation();
            SetupSpolien();
        }

        private void OnDrawGizmos()
        {
        }

        private void Update()
        {
            Bounds cameraBounds = mapDragCam.OrthograpicBounds2D();

            //As we cannot edit the list while iterating through it
            List<Sector> sectorsToDisable = new List<Sector>();
            List<Sector> sectorsToAdd = new List<Sector>();

            foreach(Sector sector in activeSectors)
            {
                Bounds sectorBounds = sector.GetBounds2D();
                if (cameraBounds.Intersects(sectorBounds))
                {                    
                    sector.UnCull();

                    //Check if slicing/merging necessary
                    Slice sliceResult = CheckSliceCondition(cameraBounds, sectorBounds);

                    if(sector.depth < mapTreeDepth)
                    {
                        if(sliceResult == Slice.Slice)
                        {
                            Debug.Log("Sector needs to slice!");
                            sectorsToAdd.AddRange(sector.Slice());
                            sectorsToDisable.Add(sector);
                            sector.Cull();
                            break;
                        }

                    }

                    if(sector.depth > 0)
                    {
                        if(sliceResult == Slice.Merge)
                        {

                            Debug.Log("Sector needs to Merge!");
                            Sector parentSector = sector.GetParentSector();
                            Sector[] childSectors = parentSector.GetChildSectors();

                            sectorsToDisable.AddRange(childSectors);                                                    
                            sectorsToAdd.Add(parentSector);
                        
                            sector.MergeThis();
                            break;
                        }
                    }

                }
                else
                {
                    //Check if this sectors parent is also not colliding, if so merge
                    //This is an optimization to keep off-screen Sector at the largest size possible
                    Sector parentSector = sector.GetParentSector();
                    if (parentSector != null)
                    {
                        if (!cameraBounds.Intersects(parentSector.GetBounds2D()))
                        {
                            Sector[] childSectors = parentSector.GetChildSectors();
                            if(childSectors != null)
                            {
                                //Merge the sectors, as the camera isn't even colliding with the parent
                                sectorsToDisable.AddRange(childSectors);                                                    
                                sectorsToAdd.Add(parentSector);
                                sector.MergeThis();
                            }
                        
                        }
                    }

                    //Cull the Sector, as we are not colliding  
                    sector.Cull();
                }
            }

            foreach(Sector sector in sectorsToAdd)
            {
                activeSectors.Add(sector);  
            }

            for(int i = 0; i < sectorsToDisable.Count; i++)
            {
                Sector sector = sectorsToDisable[i];

                activeSectors.Remove(sector);

                //This means their parent is active, and they are scheduled for a merge, and we destroy them
                if (activeSectors.Contains(sector.GetParentSector()))
                {
                    Destroy(sector.gameObject);
                }
            }
        }
        #endregion
        // =================================================================

        // =================================================================
        
        #region Private Methods
        private Slice CheckSliceCondition(Bounds camBounds, Bounds sectorBounds)
        {
            float camSize = camBounds.size.x * sliceFactor;

            if(camSize / sectorBounds.size.x <= 0.25f)
            {
                return Slice.Slice;    
            }


            if(camSize / sectorBounds.size.x >= 0.75f)
            {
                return Slice.Merge;      
            }

            return Slice.None;
        }
        
        private void SetupSpolien()
        {
            SpolienData data = DataManager.dataInstance;
            List<string> BuildingIDs = new List<string>();
            foreach(SpolieData spolie in data.spolien)
            {
                BuildingIDs.Add(spolie.getBuildingID());
            }
            BuildingIDs.Distinct();
            foreach(buildingData buildingData in data.building)
            {
                placeMarker(buildingData.adress.location, buildingData, BuildingIDs.Contains(buildingData.id));
            }
        }

        private void placeMarker(GPSPos gpsPos, buildingData buildingData = null, bool hasSpolie = false)
        {
            Vector3 pos = GPSToWorldPos(gpsPos);
            //if (pos != default)
            //{
                GameObject marker;

                if(buildingData == null)
                {
                    //This is the players Marker
                    if (gpsLocator == null)
                    {
                        gpsLocator = Instantiate(gpsMapPrefab, buildingsTransform);
                    }
                    marker = gpsLocator;
                }
                else
                {
                    //This is a Map Building Marker
                    Debug.Log($"Creating Map Marker for building {buildingData.id}: {buildingData.headline}");
                    marker = Instantiate(buildingMapPrefab, buildingsTransform);
                    MapBuilding mapBuilding = marker.GetComponent<MapBuilding>();
                    mapBuilding.Initialize(buildingData, hasSpolie);
                    sceneBuildings.Add(mapBuilding);
                }

                marker.transform.position = pos;
            //}
        }

        private void UpdateGPS(GPSPos currentPos)
        {
            placeMarker(currentPos);
        }
        #endregion
        // =================================================================

        // =================================================================
        #region Public Methods
        public Vector3 GPSToWorldPos(GPSPos pos)
        {
            float longitude01 = (pos.longitude - mapData.upperLeftCorner.longitude) / Mathf.Abs(mapData.upperLeftCorner.longitude - mapData.lowerRightCorner.longitude);
            float latitude01 = (pos.latitude - mapData.lowerRightCorner.latitude) / Mathf.Abs(mapData.upperLeftCorner.latitude - mapData.lowerRightCorner.latitude);
            if(longitude01<0 || latitude01<0 || longitude01 >1 || latitude01 > 1)
            {
                Debug.LogWarning("GPS pos outside of Bounds");
                return default;
            }
            float xPos = Mathf.Lerp(mapWorldBounds.center.x - mapWorldBounds.extents.x, mapWorldBounds.center.x + mapWorldBounds.extents.x, longitude01);
            float yPos = Mathf.Lerp(mapWorldBounds.center.y - mapWorldBounds.extents.y, mapWorldBounds.center.y + mapWorldBounds.extents.y, latitude01);
        
            return new Vector2(xPos, yPos);
        }
        #endregion
        // =================================================================    

        // =================================================================
        #region Utils
        #endregion
        // =================================================================
    }

    public enum Slice
    {
        None,
        Slice,
        Merge
    }

    [System.Serializable]
    public class GPSPos
    {
        public float latitude;
        public float longitude;

        public GPSPos(float lat, float lon)
        {
            latitude = lat;
            longitude = lon;
        }
    }
}
