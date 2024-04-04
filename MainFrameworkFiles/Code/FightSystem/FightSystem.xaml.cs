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
            EnemySide();
            ToggleActionBtns();
            ToggleEnemy();
            
        }
        public void ReturnToStory() // Returns to the story with the new data
        {
            _main.SceneChanger(2, new SaveData(playerSide[0] as Character, [.. playerSide], nexteventID));
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

        public void EnemySide()
        {
            for (int i = 0; i < enemySide.Count; i++)
            {
                var label = EnemySidePanel.FindName($"Enemy{i}") as Label;
                label.Content = $"{enemySide[i].Name}: {enemySide[i].Level}";
            }
        }

        public void ToggleActionBtns()
        {
            bool whatTO = !Attackbutton.IsEnabled;
            Attackbutton.IsEnabled = whatTO;
            Healbutton.IsEnabled = whatTO;
            Defendbutton.IsEnabled = whatTO;
            Fleebutton.IsEnabled = whatTO;
            
        }

        public void ToggleEnemy()
        {
            bool whatTO = !Enemy1Button.IsEnabled;
            Enemy1Button.IsEnabled = whatTO;
            Enemy2Button.IsEnabled = whatTO;
            Enemy3Button.IsEnabled = whatTO;
        }

        public void PlayerSelected()
        {
            bool whatTO = !Ally2.IsEnabled;
            Ally1.IsEnabled = whatTO;
            Ally2.IsEnabled = whatTO;
            Ally3.IsEnabled = whatTO;
            
        }

        private void Ally_Click(object sender, RoutedEventArgs e)
        {
            PlayerSelected();
            ToggleActionBtns();
        }

        private void Actionbutton_Click(object sender, RoutedEventArgs e)
        {
            ToggleActionBtns();
            ToggleEnemy();

        }
    }
}
