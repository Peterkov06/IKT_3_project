using InterfaceClass;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKT_3_project
{
    public partial class EventsScreen
    {
        public object? GetFromPlayer(string key, int method) // Interaction with the player: Getting some data from "it"
        {
            try
            {
                switch (method)
                {
                    case 1:
                        return player.CurrentHP;
                    case 2:
                        if (player.Buffs.TryGetValue(key, out int value))
                        {
                            return player.Stats[key] + value;
                        }
                        return player.Stats[key];
                    case 3:
                        return player.Inventory[key];
                }
            }
            catch (Exception)
            { }
            return null;
        }

        public object? GetFromDB(string key, int method) // Get data out from the DB
        {
            string connString = $"Data Source={_main.dbPath};Version=3;";
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string sqlCommand = "";
                switch (method)
                {
                    case 1: // Get item
                        sqlCommand = $"SELECT * FROM Items WHERE id = {key}";
                        using (SQLiteCommand cmd = new(sqlCommand, conn))
                        {
                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    try
                                    {
                                        JObject attributesJO = JObject.Parse(reader.GetString(2)); // Item attributes read
                                        Dictionary<string, int> attributes = attributesJO.ToObject<Dictionary<string, int>>(); //Turn attributes into a dictionary
                                        return new { Name = $"{reader.GetString(1)}", Attributes = attributes }; // Return the objectified item with it's name and attributes

                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return null;
        }

        public object? PlayerHas(string key, int method) // Checks if the player has sg. like in the GetFromPlayer
        {
            var obj = GetFromPlayer(key, method);
            if (obj == null) return false;
            return true;
        }

        public ICharacter[] EnemyConstructor(int combatID) // Creates enemy squad based on the db combat situation
        {
            List<ICharacter> enemies = new List<ICharacter>();
            string connString = $"Data Source={_main.dbPath};Version=3;";
            string json = "";
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string sqlComm = $"SELECT enemies, win_part FROM CombatSituations Where id = {combatID}";
                using (SQLiteCommand cmd = new(sqlComm, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            json = reader.GetString(0);
                        }
                    }
                }
            }
            JArray enemyArray = JArray.Parse(json);

            foreach (JObject enemyType in enemyArray)
            {
                string[] enemyTypeNumStrg = enemyType.Properties().Select(p => p.Name).ToArray();
                for (int i = 0; i < (int)enemyType[enemyTypeNumStrg[0]]; i++)
                {
                    using (SQLiteConnection conn = new SQLiteConnection(connString))
                    {
                        conn.Open();
                        string sqlComm = $"SELECT * FROM Enemies Where id = {enemyTypeNumStrg[0]}";
                        using (SQLiteCommand cmd = new(sqlComm, conn))
                        {
                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var allEnemyAttributes = reader.GetValues();
                                    var statsObj = JObject.Parse(allEnemyAttributes[7]);
                                    var inventoryObj = JObject.Parse(allEnemyAttributes[8]);
                                    var stats = statsObj.ToObject<Dictionary<string, int>>();
                                    var inventory = inventoryObj.ToObject<Dictionary<string, Dictionary<string, int>>>();

                                    enemies.Add(new Character(allEnemyAttributes[1], allEnemyAttributes[2], allEnemyAttributes[3], Convert.ToInt32(allEnemyAttributes[4]), Convert.ToInt32(allEnemyAttributes[5]), stats, [], inventory));
                                }
                            }
                        }
                    }
                }
            }
            return enemies.ToArray();
        }
    }
}
