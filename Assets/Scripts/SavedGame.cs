using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;


public static class SavedGame
{
    // XML file creator
    public static bool Save(string fileName, Game game)
    {
        string filePath = Application.persistentDataPath + "/";
        GameData gameData = new GameData();
        gameData.SetupGameData(game);
        //Debug.Log(gameData.)
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            using (StreamWriter w = new StreamWriter(filePath + fileName + ".xml"))
            {
                serializer.Serialize(w, gameData);
            }
            return true;
        }
        catch (SerializationException)
        {
            return false;
        }
    }


    // Binary Serialization
    /*
    public static bool Save(string fileName)
    {
        Setup(Game.current);
        using (FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName + ".uds", FileMode.Create))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, SavedGame);
                return true;
            }
            catch (SerializationException)
            {
                return false;
            }

        }
    }

    public static bool Load(string fileName)
    {
        using (FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName + ".uds", FileMode.Open))
        {
            try
            {

            BinaryFormatter formatter = new BinaryFormatter();
            Debug.Log(formatter.Deserialize(stream));

            return true;
            }
            catch (SerializationException)
            {
                return false;
            }
        }
    }
    
*/
}
