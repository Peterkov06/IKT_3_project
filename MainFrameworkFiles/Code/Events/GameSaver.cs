using System;
using InterfaceClass;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace IKT_3_project
{
    public static class GameSaver
    {
        public static void SaveGame(Character player, ICharacter[] team, int currID)
        {
            string jsonData = JsonConvert.SerializeObject(new SaveData(player, team, currID));
            string folderPath = "..\\..\\..\\SavedGames\\";
            string fileName = "testing1.hoi4";
            if (!File.Exists(folderPath + fileName))
            {
                File.Create(folderPath + fileName);
            }
            File.WriteAllText(folderPath + fileName, jsonData);
        }

        public static SaveData LoadGame()
        {
            string folderPath = "..\\..\\..\\SavedGames\\";
            string fileName = "testing1.hoi4";
            string jsonData = File.ReadAllText(folderPath + fileName);

            SaveData loadedData = JsonConvert.DeserializeObject<SaveData>(jsonData);
            return new SaveData(loadedData.player, loadedData.teammates, loadedData.eventID);
        }
    }
}
