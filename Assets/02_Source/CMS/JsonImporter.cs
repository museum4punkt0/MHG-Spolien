#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using System.Globalization;
using Mono.Cecil;

[CreateAssetMenu(fileName = "JsonImporter", menuName = "MHG/import Json", order = 1)]
public class JsonImporter : ScriptableObject
{
    // Path to the JSON file to import
    public string filePath;
    public string ImagePath = "Sprites/thumb/_All";
    public jn.SpolienData DataDE;
    public jn.SpolienData DataEN;
    public DataType dataType;

    // Dictionary to hold the imported data
    private Dictionary<string, object> jsonData;

    // Start is called before the first frame update

    public enum DataType
    {
        building,
        spolien
    }
    [Button("import from Path")]
    void importFromPath()
    {
        // Load the JSON file from disk
        string jsonString = File.ReadAllText(filePath);
        jn.SpolienData data = filePath.Contains("_DE") ? DataDE : DataEN; 
        // Decode the JSON file into a dictionary
        jsonData = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonString);

        // Access the data in the dictionary
        iterateJson(data,jsonData);
    }
    void iterateJson(jn.SpolienData data, Dictionary<string, object> jsonData, string primaryKey = "")
    {
        if (primaryKey != "")
        {
            //jn.buildingData currentBuilding = new jn.buildingData();
            jn.GeneralData currentObject = new jn.GeneralData();

            bool match = false;
            //check if there is a match with the primary id
            jn.GeneralData[] dataset;
            if(dataType == DataType.building)
            {
                dataset = data.building;
            }
            else
            {
                dataset = data.spolien;
            }
            foreach (jn.GeneralData b in dataset)
            {
                if (b.id == primaryKey)
                {
                    currentObject = b;
                    match = true;
                    break;
                }
            }
            //if there is no entry with current id create new entry
            if (!match)
            {
                if(dataType == DataType.building)
                {
                    currentObject = new jn.buildingData();
                    currentObject.id = primaryKey;
                    jn.buildingData[] newArr = new jn.buildingData[data.building.Length + 1];
                    for (int i = 0; i < data.building.Length; i++)
                    {
                        newArr[i] = data.building[i];
                    }
                    newArr[newArr.Length - 1] = (jn.buildingData)currentObject;
                    data.building = newArr;
                }
                else if(dataType == DataType.spolien)
                {
                    currentObject = new jn.SpolieData();
                    currentObject.id = primaryKey;
                    jn.SpolieData[] newArr = new jn.SpolieData[data.spolien.Length + 1];
                    for (int i = 0; i < data.spolien.Length; i++)
                    {
                        newArr[i] = data.spolien[i];
                    }
                    newArr[newArr.Length - 1] = (jn.SpolieData)currentObject;
                    data.spolien = newArr;
                }
                
            }
            matchObjectToJson(currentObject, jsonData);
            
        }
        foreach (KeyValuePair<string, object> kvp in jsonData)
        {
            string key = kvp.Key;
            object value = kvp.Value;
            if (value == null)
            {
                continue;
            }
            if (value.GetType() == typeof(Dictionary<string, object>))
            {
                iterateJson(data,value as Dictionary<string, object>,key);
            }
                
        }
        
    }

    void matchObjectToJson(object currentObject, Dictionary<string, object> jsonData)
    {
        var list = currentObject.GetType().GetFields();
        foreach (var field in list)
        {
            string name = field.Name;
            Debug.Log(name);
            if (field.FieldType == typeof(jn.Adress))
            {
                if (field.GetValue(currentObject) == null)
                {
                    field.SetValue(currentObject, new jn.Adress());
                }
                matchObjectToJson(field.GetValue(currentObject), jsonData);
            }
            else if (field.FieldType == typeof(jn.Thumb[]))
            {
                Dictionary<string, string> paths = new Dictionary<string, string>();
                foreach(KeyValuePair<string, object> kvp in jsonData)
                {
                    if (kvp.Value == null) continue;
                    try
                    {
                        string lower = ((string)kvp.Value).ToLower();
                        if (lower.Contains(".jpg") || lower.Contains(".png") || lower.Contains(".tif"))
                        {
                            paths.Add(kvp.Key, (string)kvp.Value);
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.LogError(e);
                    }
                    
                }
                jn.Thumb[] thumbs = new jn.Thumb[paths.Count];
                int i = 0;
                foreach(KeyValuePair<string, string> rawpath in paths)
                {
                    string path = ImagePath + "/" + rawpath.Value;   
                    
                    
                    // #todo implement a propper way to import Images from any Path and save to Sprite
                    //if (!File.Exists(path))
                    //{
                        //Debug.LogError("Image path does not exist!");
                        //return;
                    //}

                    // Load the image from the file path
                    //byte[] imageData = File.ReadAllBytes(path);
                    //Texture2D texture = new Texture2D(2, 2);
                    //texture.LoadImage(imageData);
                    // Create a new sprite from the loaded texture
                    //Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);


                    Sprite sprite = Resources.Load<Sprite>(path.Split('.')[0]);
                    //sprite.name = rawpath.Value.Split('.')[0];
                    thumbs[i] = new jn.Thumb();
                    thumbs[i].thumb = sprite;
                    thumbs[i].thumbInfo = (string)jsonData[rawpath.Key + "_Subline"];
                    thumbs[i].thumbTitle = (string)jsonData[rawpath.Key + "_Headline"];
                    if(jsonData.ContainsKey(rawpath.Key + "_type"))
                    {
                        string mode = (string)jsonData[rawpath.Key + "_type"];
                        switch (mode.ToLower())
                        {
                            case "2d":
                                thumbs[i].mode = jn.ObjectMode.twoD;
                                break;
                            case "3d":
                                thumbs[i].mode = jn.ObjectMode.threeD;
                                break;
                            default:
                                thumbs[i].mode = jn.ObjectMode.twoD;
                                break;
                        }
                    }
                    else
                    {
                        thumbs[i].mode = jn.ObjectMode.twoD;
                    }
                    
                    
                    i++;
                }
                field.SetValue(currentObject, thumbs);

            }
            else if (jsonData.ContainsKey(name) && jsonData[name] != null)
            {
                Debug.Log("match found in json");
                try
                {
                    var value = jsonData[name];
                    Debug.Log("setting "+ name+ " to: "+ value);
                    field.SetValue(currentObject, (string)value);
                }
                catch (InvalidCastException e)
                {
                    Debug.LogWarning(e);
                }
            }
            else if(name == "location")
            {
                try
                {
                    jn.GPSPos loc = (jn.GPSPos)field.GetValue(currentObject);
                    loc.latitude = float.Parse(((string)jsonData["GPS"]).Split(',')[0].Trim(), CultureInfo.InvariantCulture);
                    loc.longitude = float.Parse(((string)jsonData["GPS"]).Split(',')[1].Trim(), CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }
}
#endif