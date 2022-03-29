using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class PlayerSave{
    private static string path = Application.persistentDataPath + "/player.shs";
    
    public static void SavePlayer(playerController player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerData data = null;
            if (stream.Length > 0)
            {
                data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();
            }
            return data;
        }
        else
        {
            Debug.Log("Save File not found in " + path);
            return null;
        }
    }

    public static void deleteSave()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
