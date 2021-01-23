using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class PlayerSave
{
    private static string path = Application.persistentDataPath + "/player.poker";

    public static void SavePlayer(string username, int cash)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData(username, cash);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static void SavePlayer(string username, int cash, string fileName)
    {
        path = Application.persistentDataPath + "/" + fileName + ".poker";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData(username, cash);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerData data = (PlayerData)formatter.Deserialize(stream);

            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in" + path);
            return null;
        }
    }

    public static bool SaveExist()
    {
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
