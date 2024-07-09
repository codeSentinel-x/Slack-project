using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem {


    public static void Save<T>(string path, string name, T data) {

        string fullPath = path + "/" + name + ".bak";
        string jsonData = JsonUtility.ToJson(data);
        using FileStream stream = new(fullPath, FileMode.Create);
        using StreamWriter writer = new(stream);
        writer.Write(jsonData);
    }
    public static T Load<T>(string path, string name) {
        T loadedData = default;
        string fullPath = path + "/" + name + ".bak";
        if (File.Exists(fullPath)) {

            string dataToLoad = "";
            using (FileStream stream = new(fullPath, FileMode.Open)) {
                using StreamReader reader = new(stream);
                dataToLoad = reader.ReadToEnd();
            }

            loadedData = JsonUtility.FromJson<T>(dataToLoad);
        }
        return loadedData;
    }
    public static void DeleteSave(string path, string name) {
        string fullPath = path + "/" + name + ".bak";
        if (File.Exists(fullPath)) {
            File.Delete(fullPath);
        }
    }
}
