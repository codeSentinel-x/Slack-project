using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveSystem {
    public const string NOISE_SETTING_DEFAULT_SAVE_PATH = "/NoiseSettings";
    public static string PERSISTANCE_DATA_PATH = Application.persistentDataPath;


    public static void Save<T>(string path, string name, T data, string ext = ".json") {
        path = Path.Combine(PERSISTANCE_DATA_PATH + path);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        string fullPath = Path.Combine(path, name + ext);
        string jsonData = JsonUtility.ToJson(data);
        using FileStream stream = new(fullPath, FileMode.Create);
        using StreamWriter writer = new(stream);
        writer.Write(jsonData);

    }
    public static List<string> GetAllFileNameFromDirectory(string path, string ext = ".json") {
        List<string> result = new();
        string fullPath = Path.Combine(PERSISTANCE_DATA_PATH, path);
        if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
        foreach (var s in Directory.EnumerateFiles(fullPath, ext, SearchOption.TopDirectoryOnly)) {
            // if (!s.EndsWith(ext)) continue;
            string name = Path.GetFileNameWithoutExtension(s);//.TrimStart("fullPath".ToArray<Char>());
            // name = name.TrimEnd(ext.ToArray<Char>());
            result.Add(name);
        }

        return result;
    }
    public static T Load<T>(string path, string name, string ext = ".json") {

        T loadedData = default;
        string fullPath = Path.Combine(PERSISTANCE_DATA_PATH, path, name + ext);
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
    public static void DeleteSave(string path, string name, string ext = ".json") {

        string fullPath = Path.Combine(PERSISTANCE_DATA_PATH, path, name + ext);
        if (File.Exists(fullPath)) {
            File.Delete(fullPath);
        }
    }
}
