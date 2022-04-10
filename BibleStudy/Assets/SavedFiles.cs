using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SavedFiles
{

    public static SavedFiles current;
    public string fileName;
    public string shortName;
    public List<string> lines;

    public SavedFiles()
    {
        fileName = "";
        lines = new List<string>();
    }

}