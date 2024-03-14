using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKT_3_project.MainFrameworkFiles.Models
{
    public class SharedInterfaces
    {
        public interface ICharacter 
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

            public void TakeDamage(int damage);
            public void Heal(int heal);
        }

        public interface IAdditionalSystem
        {
            public int GetID();
            public object? Execute(object[] parameters);
        }

        public interface IStoryImages
        {
            
        }

    }
}
