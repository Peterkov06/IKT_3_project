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
                        return player.HP;
                    case 2:
                        if (player.Buffs.ContainsKey(key))
                        {
                            return player.Stats[key] + player.Buffs[key];
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
            string connString = $"Data Source={dbPath};Version=3;";
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
                    case 2: // Get Enemy (for fight)
                        sqlCommand = $"SELECT * FROM Enemies WHERE id = {key}";
                        using (SQLiteCommand cmd = new(sqlCommand, conn))
                        {
                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    JObject statsJO = JObject.Parse(reader.GetString(7));
                                    Dictionary<string, int> stats = statsJO.ToObject<Dictionary<string, int>>();
                                    JObject inventoryJO = JObject.Parse(reader.GetString(8));
                                    Dictionary<string, Dictionary<string, int>> inventory = inventoryJO.ToObject<Dictionary<string, Dictionary<string, int>>>();
                                    return new { Name = reader.GetString(1), Class = reader.GetString(2), Race = reader.GetString(3), Level = reader.GetInt32(4), MaxHP = reader.GetInt32(5), CurrentHP = reader.GetInt32(6), Stats = stats, Inventory = inventory };
                                }
                            }
                            break;
                        }
                }
            }
            return null;
        }
    }
}
