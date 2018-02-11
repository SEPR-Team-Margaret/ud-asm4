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

    public static bool Load(string fileName)
    {
        string filePath = Application.persistentDataPath + "/";
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            using (StreamReader r = new StreamReader(filePath + fileName + ".xml"))
            {
                GameData savedGame = (GameData)serializer.Deserialize(r);
                // Access to saved game like -> savedGame.currentPlayerID
                // Calls to instantiate methods
                return true;
            }
        }
        catch (SerializationException)
        {
            return false;
        }
    }
}
