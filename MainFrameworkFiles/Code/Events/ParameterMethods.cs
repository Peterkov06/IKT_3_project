using InterfaceClass;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IKT_3_project
{
    public partial class EventsScreen
    {
        public object? GetHPFromPlayer(string key, int method) // Interaction with the player: Getting some data from "it"
        {
            return player.CurrentHP;
        }

        public object? GetStatFromPlayer(string key, int method)
        {
            if (player.Buffs.TryGetValue(key, out int value))
            {
                return player.Stats[key] + value;
            }
            return player.Stats[key];
        }

        public object? GetItemFromPlayer(string key, int method)
        {
            if (player.Inventory.TryGetValue(key,out var value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Gets the id of an item, and returns it from the Items table.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="method"></param>
        /// <returns>Object: Name, Attributes (Dictionary<string, int>)</returns>
        public object? GetItemFromDB(string key, int method) // Get data out from the DB
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
                                        return new InventoryItem(Name = $"{reader.GetString(1)}", attributes) ; // Return the objectified item with it's name and attributes

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

        public object? GetPlayerLevel(string key, int method)
        {
            return player.Level;
        }

        public object? GetPlayerClass(string key, int method)
        {
            return player.Class;
        }

        public object? GetPlayerRace(string key, int method)
        {
            return player.Race;
        }

        public ICharacter[] EnemyConstructor(int combatID, out int nextPartID, out int fleeID) // Creates enemy squad based on the db combat situation
        {
            nextPartID = 0; 
            fleeID = 0;
            List<ICharacter> enemies = new List<ICharacter>();
            string connString = $"Data Source={_main.dbPath};Version=3;";
            string json = "";
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string sqlComm = $"SELECT enemies, win_part, flee_part FROM CombatSituations Where id = {combatID}";
                using (SQLiteCommand cmd = new(sqlComm, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            json = reader.GetString(0);
                            nextPartID = reader.GetInt32(1);
                            fleeID = reader.GetInt32(2);
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

        public object? AllyConstructor(string key, int method) // Creates enemy squad based on the db combat situation
        {
            int id = int.Parse(key);
            string connString = $"Data Source={_main.dbPath};Version=3;";

            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string sqlComm = $"SELECT * FROM Allies Where id = {id}";
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

                            teamMates.Add(new Character(allEnemyAttributes[1], allEnemyAttributes[2], allEnemyAttributes[3], Convert.ToInt32(allEnemyAttributes[4]), Convert.ToInt32(allEnemyAttributes[5]), stats, [], inventory));
                        }
                    }
                }
            }
            return null;
        }
    }
}
