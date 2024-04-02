using InterfaceClass;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IKT_3_project
{
    /// <summary>
    /// Interaction logic for FightSystem.xaml
    /// </summary>
    public partial class FightSystem : UserControl
    {
        MainWindow _main; // main window
        List<ICharacter?> playerSide; // A list that stores the player's team [0] -> Player
        List<ICharacter?> enemySide; // A list that stores the enemy team [x] -> Enemy
        public Dictionary<int, IAdditionalSystem> additionalSystems; // This stores the methods required to calculate the damage and other thingsű
        int nexteventID; // The database ID of the next event after winning
        int selectedAllayID;
        
        public FightSystem(MainWindow main, ICharacter?[] playerSide, ICharacter?[] enemySide, Dictionary<int, IAdditionalSystem> addSys, int nexteventID)
        {
            InitializeComponent();
            _main = main;
            this.playerSide = [.. playerSide];
            this.enemySide = [.. enemySide];
            this.additionalSystems = addSys;
            this.nexteventID = nexteventID;
            PlayerSide();
            ToggleActionBtns();
            
        }
        public void ReturnToStory() // Returns to the story with the new data
        {
            _main.SceneChanger(2, new SaveData(playerSide[0] as Character, [.. playerSide], nexteventID));
        }




        private void Attackbutton_Click(object sender, RoutedEventArgs e)
        {
        }


        private void SelectAlly(object sender, RoutedEventArgs e)
        {
            selectedAllayID = 1;
        }


        public void PlayerSide()
        {
            for (int i = 0; i < playerSide.Count; i++)
            {
                var label = PlayerSidePanel.FindName($"label{i}") as Label;
                label.Content = $"{playerSide[i].Name}: {playerSide[i].Level}";
                var progressBar = PlayerSidePanel.FindName($"Playerprog{i}") as ProgressBar;
            }
        }

        public void ToggleActionBtns()
        {
            bool whatTO = !Attackbutton.IsEnabled;
            Attackbutton.IsEnabled = whatTO;
            Healbutton.IsEnabled = whatTO;
        }
    }
}
