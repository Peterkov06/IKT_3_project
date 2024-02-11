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
            public Dictionary<string, object> Inventory { get; set; }

            public void TakeDamage(int damage);
            public int CalculateDamage();
            public void Heal(int heal);
        }

        public interface IEquippable
        {
            public string Name { get; set; }
            public int UsableTimes { get; set; }

            public void PickUp();
            public void Use();
        }

        public interface IOpenable
        {
            public string Name { get; set; }

            public void Open();
        }

        public interface IAdditionalSystem
        {
            public int GetID();
            public void Execute();
        }

    }
}
