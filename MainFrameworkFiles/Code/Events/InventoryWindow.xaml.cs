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
            InventoryList.Items.Clear();
            foreach (var item in Player.Inventory)
            {
                InventoryList.Items.Add(new Item(item.Key, item.Value));
            }
        }

        private void ChangeSelectedWeapon(object sender, RoutedEventArgs e)
        {
            CheckBox thisCheck = sender as CheckBox;
            if (!(bool)thisCheck.IsChecked)
            {
                return;
            }

            var selectedBefore = Player.GetWeapon() as Dictionary<string, Dictionary<string, int>>;
            Player.Inventory[selectedBefore.Keys.First()].Remove("SelectedWeapon");

            Item itemName = thisCheck.DataContext as Item;
            Player.Inventory[itemName.Name].Add("SelectedWeapon", 1);
            ShowInevtory();
        }
    }

    class Item
    {
        public string Name { get; set; }
        public string Attributes { get; set; }
        public string IsWeapon { get; set; }
        public bool IsSelectedWeapon { get; set; }

        public Item(string name, Dictionary<string, int> attributes)
        {
            Attributes = "";
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsWeapon = "Hidden";
            IsSelectedWeapon = false;
            foreach (var attribute in attributes)
            {
                if (attribute.Key != "Weapon" && attribute.Key != "SelectedWeapon")
                {
                    Attributes += $"{attribute.Key}: {attribute.Value}, ";
                }
                else
                {
                    IsWeapon = "Visible";
                }
            }
            if (attributes.ContainsKey("SelectedWeapon"))
            {
                IsSelectedWeapon = true;
            }
        }
    }
}
