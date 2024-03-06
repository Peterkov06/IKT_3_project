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
        public object? GiveToPlayer(string key, int method) // Interaction with the player: Adding sg to "it"
        {
            switch (method)
            {
                case 1:
                    player.CurrentHP += Convert.ToInt32(key);
                    return null;
                case 2:
                    //To be implemented: Stat adding
                    JObject stat = JsonConvert.DeserializeObject<JObject>(key);
                    IEnumerable<string> keys = stat.Properties().Select(p => p.Name);
                    foreach (string keyStrg in keys)
                    {
                        player.Stats.Add(keyStrg, (int)stat[keyStrg]);
                    }
                    return null;
                case 3:
                    //To be implemented: Inventory adding
                    JObject items = JsonConvert.DeserializeObject<JObject>(key);
                    IEnumerable<string> itemKeys = items.Properties().Select(p => p.Name);
                    foreach (string keyStrg in itemKeys)
                    {
                        JObject item = (JObject)items[keyStrg];
                        IEnumerable<string> itemData = item.Properties().Select(p => p.Name);
                        Dictionary<string, int> dat = new Dictionary<string, int>();
                        foreach (string dataKeyStrg in itemData)
                        {
                            dat.Add(dataKeyStrg, (int)item[dataKeyStrg]);
                        }
                        player.Inventory.Add(keyStrg, dat);
                    }
                    return null;
                case 4:
                    //To be implemented: Buff adding
                    JObject buff = JsonConvert.DeserializeObject<JObject>(key);
                    IEnumerable<string> buffKeys = buff.Properties().Select(p => p.Name);
                    foreach (string keyStrg in buffKeys)
                    {
                        player.Buffs.Add(keyStrg, (int)buff[keyStrg]);
                    }
                    MessageBox.Show($"{player.Buffs.Count}");
                    return null;
            }
            return null;
        }

        public object? StartFight(int id) // Starts the fight system
        {
            _main.SceneChanger(3, new LoadFightScene([player, new Character("grg", "fesfg", "fwf3w", 3, 500, new(), new(), new()), new Character("grg", "fesfg", "fwf3w", 3, 500, new(), new(), new())], [new Character("enemy1", "fesfg", "fwf3w", 3, 500, new(), new(), new()), new Character("enemy2", "fesfg", "fwf3w", 3, 500, new(), new(), new())]));
            return null;
        }
    }
}
