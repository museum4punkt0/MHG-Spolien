using UnityEngine;
using System.Collections;
using UnityEngine.Android;
using System;
namespace jn
{
    public class LocationServiceProvider : MonoBehaviour
    {
        public Action<GPSPos> updatedGPS;
        public bool locationServiceRunning = false;
        public float sleepSeconds = 10;
        public void getGPSLocation()
        {
            #if UNITY_ANDROID || UNITY_IOS
            if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                locationServiceRunning = true;
                StartCoroutine(getLocation());
                
            }
            else
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }
            #endif
        }

        public void OnDestroy()
        {
            locationServiceRunning=false;
        }
        public void OnDisable()
        {
            locationServiceRunning = false;
        }
        IEnumerator getLocation()
        {
            bool goOn = locationServiceRunning;
            // Check if the user has location service enabled.
            if (!Input.location.isEnabledByUser)
            {
                Debug.LogWarning("location not enabled");
                yield break;
            }

            // Starts the location service.
            Input.location.Start();

            // Waits until the location service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // If the service didn't initialize in 20 seconds this cancels location service use.
            if (maxWait < 1)
            {
                Debug.Log("Timed out");
                yield break;
            }

            // If the connection failed this cancels location service use.
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("Unable to determine device location");
                yield break;
            }
            else
            {
                GPSPos currentLocation = new GPSPos(Input.location.lastData.latitude, Input.location.lastData.longitude);
                updatedGPS(currentLocation);
                // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
                Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            }

            // Stops the location service if there is no need to query location updates continuously.
            Input.location.Stop();

            if (goOn)
            {
                Debug.Log("location Service going to sleep");
                yield return new WaitForSeconds(sleepSeconds);
                StartCoroutine(getLocation());
            }
        }
    }
}
