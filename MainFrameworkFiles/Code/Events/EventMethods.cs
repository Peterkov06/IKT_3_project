using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IKT_3_project
{
    public partial class EventsScreen
    {
        public object? GiveHPToPlayer(string key, int method) // Interaction with the player: Adding sg to "it"
        {
            player.CurrentHP += Convert.ToInt32(key);
            return null;

        }

        public object? GiveStatToPlayer(string key, int method) // Interaction with the player: Adding sg to "it"
        {
            //To be implemented: Stat adding
            JObject stat = JObject.Parse(key);
            IEnumerable<string> keys = stat.Properties().Select(p => p.Name);
            foreach (string keyStrg in keys)
            {
                player.Stats.Add(keyStrg, (int)stat[keyStrg]);
            }
            MessageBox.Show($"{player.Stats.Count}");
            return null;
        }

        public object? GiveItemToPlayer(string key, int method) // Interaction with the player: Adding sg to "it"
        {
            //To be implemented: Inventory adding WORKS
            /*[{"LongBow": {"Damage": 4, "Endurance": 1}},{ "Sword": {"Damage":16}}]*/
            JArray items = JArray.Parse(key);

            foreach (JObject item in items)
            {
                IEnumerable<string> itemName = item.Properties().Select(p => p.Name);
                Dictionary<string, int> dat = item[itemName.First()].ToObject<Dictionary<string, int>>();
                player.Inventory.Add(itemName.First(), dat);
            }
            return null;
        }

        public object? GiveBuffToPlayer(string key, int method) // Interaction with the player: Adding sg to "it"
        {
            //To be implemented: Buff adding WORKS
            /*{"Strength": 20, ...}*/
            JObject buff = JObject.Parse(key);
            IEnumerable<string> buffKeys = buff.Properties().Select(p => p.Name);
            foreach (string keyStrg in buffKeys)
            {
                player.Buffs.Add(keyStrg, (int)buff[keyStrg]);
            }
            return null;
        }

        public object? StartFight(string parameters, int method) // Starts the fight system
        {;
            int sitID = int.Parse(parameters);
            int nextID;

            var enemies = EnemyConstructor(sitID, out nextID);

            _main.SceneChanger(3, new LoadFightScene([player, new Character("grgv edfws", "fesfg", "fwf3w", 3, 500, new(), new(), new()), new Character("grg", "fesfg", "fwf3w", 3, 500, new(), new(), new())], [.. enemies], nextID));
            return null;
        }
    }
}
