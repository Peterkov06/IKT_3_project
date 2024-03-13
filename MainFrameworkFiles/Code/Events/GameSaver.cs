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
        public static void SaveGame(Character player, ICharacter[] team, int currID, string xmlPath, string filePath)
        {
            string jsonData = JsonConvert.SerializeObject(new SaveData(player, team, currID, xmlPath));
            if (!File.Exists(filePath))
            {
                var file = File.Create(filePath);
                file.Close();
            }
            File.WriteAllText(filePath, jsonData);
        }

        public static SaveData LoadGame(string filePath)
        {
            string jsonData = File.ReadAllText(filePath);

            SaveData loadedData = JsonConvert.DeserializeObject<SaveData>(jsonData);
            return new SaveData(loadedData.player, loadedData.teammates, loadedData.eventID, loadedData.XMLpath);
        }
    }
}
