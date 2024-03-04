using InterfaceClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IKT_3_project
{
    public class Character: ICharacter // Character object
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public string Race { get; set; }
        public int Level { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public Dictionary<string, int> Stats { get; set; }
        public Dictionary<string, int> Buffs { get; set; }
        public Dictionary<string, Dictionary<string, int>> Inventory { get; set; }

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
        }

        public void TakeDamage(int damage)
        {
            CurrentHP -= damage;
            if (CurrentHP <= 0)
            {
                Death();
            }
        }

        void Death()
        {
            MessageBox.Show("You died!");
            Application.Current.Shutdown();
        }

        public int CalculateDamage()
        {
            throw new NotImplementedException();
        }

        public void Heal(int heal)
        {
            CurrentHP += heal;
        }
    }


    public class LoadNewStory
    {
        public string dbPath;

        public LoadNewStory(string dbPath)
        {
            this.dbPath = dbPath;
        }
    }

    public class BackToStory
    {
        public Character player;
        public ICharacter?[] teammates;
        public int eventID;

        public BackToStory(Character palyer, ICharacter?[] teammates, int eventID)
        {
            this.player = palyer ?? throw new ArgumentNullException(nameof(palyer));
            this.teammates = teammates ?? throw new ArgumentNullException(nameof(teammates));
            this.eventID = eventID;
        }
    }

    public class LoadFightScene
    {
        public ICharacter?[] playerSide;
        public ICharacter?[] enemySide;

        public LoadFightScene(ICharacter?[] playerSide, ICharacter?[] enemySide)
        {
            this.playerSide = playerSide ?? throw new ArgumentNullException(nameof(playerSide));
            this.enemySide = enemySide ?? throw new ArgumentNullException(nameof(enemySide));
        }
    }

    public class LoadCharacterCreatorObj
    {
        public string[] classes;
        public string[] races;
        public string[] stats;

        public LoadCharacterCreatorObj(string[] classes, string[] races, string[] stats)
        {
            this.classes = classes ?? throw new ArgumentNullException(nameof(classes));
            this.races = races ?? throw new ArgumentNullException(nameof(races));
            this.stats = stats ?? throw new ArgumentNullException(nameof(stats));
        }
    }
}
