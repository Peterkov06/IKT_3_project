using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

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
                player.Stats.TryAdd(keyStrg, (int)stat[keyStrg]);
            }
            return null;
        }

        public object? GiveItemToPlayer(string key, int method) // Interaction with the player: Adding sg to "it"
        {
            //To be implemented: Inventory adding WORKS
            /*[1,2]*/
            JArray items = JArray.Parse(key); // ids of objs

            foreach (int item in items)
            {
                var data = GetItemFromDB(item.ToString(), 1) as InventoryItem;
                player.Inventory.TryAdd(data.Name, data.Attributes );
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
                player.Buffs.TryAdd(keyStrg, (int)buff[keyStrg]);
            }
            return null;
        }

        public object? StartFight(string parameters, int method) // Starts the fight system
        {;
            int sitID = int.Parse(parameters);
            int nextID;

            var enemies = EnemyConstructor(sitID, out nextID, out int fleeID);

            _main.SceneChanger(3, new LoadFightScene([player/*, new Character("grgv edfws", "fesfg", "fwf3w", 3, 100, [], [], []), new Character("grg", "fesfg", "fwf3w", 3, 100, [], [], [])*/], [.. enemies], nextID, fleeID));
            return null;
        }

        public bool IsValidChoice(string json, int id)
        {
            bool isTrue = false;
            if (_main.unavailableChoicheIDs.Contains(id))
            {
                return false;
            }
            else if (json == "")
            {
                return true;
            }
            JArray conditions = JArray.Parse(json);
            foreach (JObject condition in conditions)
            {
                IEnumerable<string> keys = condition.Properties().Select(p => p.Name);
                string key = keys.First();
                int methodNum = (int)condition[key];
                if (methodNum >= 5)
                {
                    var element = ParameterMethods[methodNum].Invoke("", methodNum);
                    if (element != null)
                    {
                        if (element is int num && num >= int.Parse(key))
                        {
                            isTrue = true;
                        }
                        else if (element is string name && name == key)
                        {
                            isTrue = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    var obj = ParameterMethods[methodNum].Invoke(key, methodNum);
                    if (obj != null)
                    {
                        isTrue = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return isTrue;
        }

    }
}
