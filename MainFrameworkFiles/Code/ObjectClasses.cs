using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IKT_3_project
{
    public class Player
    {
        public int HP { get; set; }
        public Dictionary<string, int> Stats { get; set; }
        public Dictionary<string, int> Buffs { get; set; }
        public Dictionary<string, Dictionary<string, int>> Inventory { get; set; }

        public Player(int hP)
        {
            HP = hP;
            Stats = new();
            Buffs = new();
            Inventory = new();
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            if (HP <= 0)
            {
                Death();
            }
        }

        void Death()
        {
            MessageBox.Show("You died!");
            Application.Current.Shutdown();
        }
    }
}
