using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Serializer : MonoBehaviour
{
    private string ext = ".dat";

    private BinaryFormatter bf;
    private FileStream file;

    public void Serialization(GameData tempData, string filename, string path)
    {
        bf = new BinaryFormatter();
        file = File.Create(path + "/" + filename + ext);

        bf.Serialize(file, tempData);
        file.Close();
    }

    public void DeSerialization(string filename, string path, out GameData deserialized)
    {
        bf = new BinaryFormatter();
        if (File.Exists(path + "/" + filename + ext))
        {
            file = File.Open(path + "/" + filename + ext, FileMode.Open);
        }
        else
        {
            deserialized = null;
            return;
        }

        if (file != null && file.Length > 0)
        {
            deserialized = (GameData)bf.Deserialize(file);
        }
        else
        {
            deserialized = null;
        }

        file.Close();
    }
}

