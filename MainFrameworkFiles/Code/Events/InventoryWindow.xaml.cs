using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IKT_3_project
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class InventoryWindow : Window
    {
        Character Player;
        public InventoryWindow(ref Character player)
        {
            InitializeComponent();
            Player = player;
            ShowInevtory();
        }

        void ShowInevtory()
        {
            foreach (var item in Player.Inventory)
            {
                InventoryList.Items.Add(new Item(item.Key, item.Value));
            }
        }
    }

    class Item
    {
        public string Name { get; set; }
        public string Attributes { get; set; }

        public Item(string name, Dictionary<string, int> attributes)
        {
            Attributes = "";
            Name = name ?? throw new ArgumentNullException(nameof(name));
            foreach (var attribute in attributes)
            {
                if (attribute.Key != "Weapon" && attribute.Key != "SelectedWeapon")
                {
                    Attributes += $"{attribute.Key}: {attribute.Value}, ";
                }
            }
        }
    }
}
