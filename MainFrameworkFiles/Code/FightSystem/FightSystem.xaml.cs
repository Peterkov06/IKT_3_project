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
        int selectedEnemyID;
        int turnNumber=0;
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
        private void SelectEnemy(object sender, RoutedEventArgs e)
        {
            selectedEnemyID = 1;
        }




        public void PlayerSide()
        {
            for (int i = 0; i < playerSide.Count; i++)
            {
                var label = PlayerSidePanel.FindName($"label{i}") as Label;
                label.Content = $"{playerSide[i].Name}: {playerSide[i].Level}";
                label.Visibility = Visibility.Visible;
                var progressBar = PlayerSidePanel.FindName($"Playerprog{i}") as ProgressBar;
                progressBar.Visibility = Visibility.Visible;
                var btn = FindName($"Ally{i}") as Button;
                btn.Visibility = Visibility.Visible;
                int val = i;
                btn.Click += (s, e) => 
                {
                    selectedAllayID = val;
                    Turn(23);
                    
                };

            }

        }

        public void EnemySide()
        {
            for (int i = 0; i < enemySide.Count; i++)
            {
                var label = EnemySidePanel.FindName($"Enemy{i}") as Label;
                label.Content = $"{enemySide[i].Name}: {enemySide[i].Level}";
                label.Visibility = Visibility.Visible;
                var progressBar = EnemySidePanel.FindName($"Enemyprog{i}") as ProgressBar;
                progressBar.Visibility= Visibility.Visible;
                progressBar.Maximum = enemySide[selectedEnemyID].MaxHP;
                progressBar.Value =enemySide[selectedEnemyID].CurrentHP;
                var btn = FindName($"EnemyButton{i}") as Button;
                btn.Visibility=Visibility.Visible;
                int val = i;
                btn.Click += (s, e) =>
                {
                    selectedEnemyID = val;
                    AllyDamage(selectedEnemyID);
                    progressBar.Value = enemySide[selectedEnemyID].CurrentHP;
                };


            }
            
        }

        public void DiceRoll()
        {
            Random r = new();
            int checkDice = 8;
            int Rolled = r.Next(1, checkDice);
        }

        public void AllyDamage(int enemyID)
        {
            int damage = 20;
            enemySide[enemyID].TakeDamage(damage);

            MessageBox.Show($"{enemySide[enemyID].MaxHP} ; {enemySide[enemyID].CurrentHP}");



        }

        public void Turn(int turnNumber)
        {
            MessageBox.Show(turnNumber.ToString());

        }

        public void Death()
        {

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
            bool whatTO = !EnemyButton0.IsEnabled;
            EnemyButton0.IsEnabled = whatTO;
            EnemyButton1.IsEnabled = whatTO;
            EnemyButton2.IsEnabled = whatTO;
        }

        public void PlayerSelected()
        {
            bool whatTO = !Ally0.IsEnabled;
            Ally0.IsEnabled = whatTO;
            Ally1.IsEnabled = whatTO;
            Ally2.IsEnabled = whatTO;
            
        }

        private void Ally_Click(object sender, RoutedEventArgs e)
        {
            PlayerSelected();
            ToggleActionBtns();
            
        }

        
        private void Attackbutton_Click(object sender, RoutedEventArgs e)
        {
            ToggleActionBtns();
            ToggleEnemy();
            

        }
        private void Defendbutton_Click(object sender, RoutedEventArgs e)
        {
            ToggleActionBtns();
            PlayerSelected();

        }
        private void Healbutton_Click(object sender, RoutedEventArgs e)
        {
            ToggleActionBtns();
            PlayerSelected();

        }
        private void Fleebutton_Click(object sender, RoutedEventArgs e)
        {

            ReturnToStory();
        }
    }
}
