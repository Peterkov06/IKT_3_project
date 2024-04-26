using InterfaceClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IKT_3_project
{
    /// <summary>
    /// The main Character class, which is implemented by all characters.
    /// </summary>
    public class Character // Character object
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public string Race { get; set; }
        public int Level { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public bool Alive { get; set; }

        public Dictionary<string, int> Stats { get; set; }
        public Dictionary<string, int> Buffs { get; set; }
        public Dictionary<string, Dictionary<string, int>> Inventory { get; set; }
        public string b64IconString { get; set; }

        public Character(string name, string @class, string race, int level, int maxHP, Dictionary<string, int> stats, Dictionary<string, int> buffs, Dictionary<string, Dictionary<string, int>> inventory)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Class = @class ?? throw new ArgumentNullException(nameof(@class));
            Race = race ?? throw new ArgumentNullException(nameof(race));
            Level = level;
            MaxHP = maxHP;
            CurrentHP = maxHP;
            Stats = stats ?? throw new ArgumentNullException(nameof(stats));
            Buffs = buffs ?? throw new ArgumentNullException(nameof(buffs));
            Inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            b64IconString = "";
            Alive = true;
        }

        /// <summary>
        /// Substracts the damage parameter from the CurrentHP.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            CurrentHP -= damage;
            if (CurrentHP <= 0)
            {
                Death();
            }
        }

        public void Death()
        {
            Alive = false;
        }

        /// <summary>
        /// Gets the currently active weapon and returns it as an object.
        /// </summary>
        /// <returns>Dictionary with 1 element, which is the active weapon. If there isn't any, it returns null.</returns>
        public object? GetWeapon() // Returns the the only weapon present in the inventory, as a dictionary, like: "{name}" : {{"MinDamage": 5} {"MaxDamage": 10} {"Weapon": 1}}
        {
            var weapon = Inventory.Where(item => item.Value.ContainsKey("SelectedWeapon")).ToDictionary();
            //MessageBox.Show($"{weapon.Keys.First()}");
            if (weapon.Count > 0)
            {
                return weapon;
            }
            var weaps = Inventory.Where(item => item.Value.ContainsKey("Weapon")).ToDictionary();
            weaps.First().Value.Add("SelectedWeapon", 1);
            return Inventory.Where(item => item.Value.ContainsKey("SelectedWeapon")).ToDictionary();
        }

        /// <summary>
        /// Heals the player by the health int amount. Adds it to the current health. If it exceeds the max amount, it sets the CurrentHP to MaxHP.
        /// </summary>
        /// <param name="heal"></param>
        public void Heal(int heal)
        {
            if (CurrentHP + heal !> MaxHP)
            {
                CurrentHP += heal;
            }
            else
            {
                CurrentHP = MaxHP;
            }
        }
    }


    public class LoadNewStory(string dbPath)
    {
        public string dbPath = dbPath;
    }

    public class BeginNewStory(Character player)
    {
        public Character player = player;
    }

    public class LoadFightScene(Character?[] playerSide, Character?[] enemySide, int nextEventID, int fleeID)
    {
        public Character?[] playerSide = playerSide ?? throw new ArgumentNullException(nameof(playerSide));
        public Character?[] enemySide = enemySide ?? throw new ArgumentNullException(nameof(enemySide));
        public int nextEventID = nextEventID;
        public int fleeID = fleeID;
    }

    public class LoadCharacterCreatorObj(string[] classes, string[] races, string[] stats)
    {
        public string[] classes = classes ?? throw new ArgumentNullException(nameof(classes));
        public string[] races = races ?? throw new ArgumentNullException(nameof(races));
        public string[] stats = stats ?? throw new ArgumentNullException(nameof(stats));
    }

    public class SaveData
    {

        public Character player;
        public Character?[] teammates;
        public int eventID;
        public int[] UnusableIDs { get; set; }
        public string? XMLpath { get; set; }
        [JsonConstructor]
        public SaveData(Character player, Character?[] teammates, int eventID, string xml, int[] unusableIDs)
        {
            this.player = player ?? throw new ArgumentNullException(nameof(player));
            this.teammates = teammates ?? throw new ArgumentNullException(nameof(teammates));
            this.eventID = eventID;
            XMLpath = xml;
            UnusableIDs = unusableIDs;
        }
    }

    public class InventoryItem
    {
        public string Name { get; set; }
        public Dictionary<string, int> Attributes { get; set; }

        public InventoryItem(string name, Dictionary<string, int> attributes)
        {
            Name = name;
            Attributes = attributes;
        }
    }

}
