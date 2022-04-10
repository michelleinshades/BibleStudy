using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{
    public static List<SavedFiles> listOfSavedFiles = new List<SavedFiles>();


    public static bool Save(string saveName, List<string> fileDataToSave)
    {
        return Save(Application.persistentDataPath, saveName, fileDataToSave);
    }


    public static bool Save(string directoryLocation, string saveName, List<string> fileDataToSave)
    {

        BinaryFormatter formatter = new BinaryFormatter();

        if (!Directory.Exists(directoryLocation + "/saves"))
        {
            Directory.CreateDirectory(directoryLocation + "/saves");
        }

        string path = directoryLocation + "/saves/" + saveName + ".save";

        FileStream file = File.Create(path);

        formatter.Serialize(file, fileDataToSave);

        file.Close();

        return true;

    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        return formatter;
    }

    public static List<string> Load(string path)
    {
        return Load(Application.persistentDataPath, path);
    }

    public static List<string> Load(string directoryLocation, string path)
    {

        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            SavedFiles.current = new SavedFiles();
            SavedFiles.current.lines = (List<string>) bf.Deserialize(file);
            string fileNameToSave = path.Replace(directoryLocation + "/saves", "");
            fileNameToSave = fileNameToSave.Replace("\\", "");
            fileNameToSave = fileNameToSave.Replace("/", "");
            fileNameToSave = fileNameToSave.Replace(".save", "");
            
            SavedFiles.current.fileName = fileNameToSave;
            file.Close();
            return SavedFiles.current.lines;
        }
        catch
        {
            Debug.Log("Failed to load file " + path);
            file.Close();
            return null;

        }
    }
}