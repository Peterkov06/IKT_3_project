using InterfaceClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        List<Character?> playerSide; // A list that stores the player's team [0] -> Player
        List<Character?> enemySide; // A list that stores the enemy team [x] -> Enemy
        public Dictionary<int, IAdditionalSystem> additionalSystems; // This stores the methods required to calculate the damage and other things
        int nexteventID, fleeID; // The database ID of the next event after winning
        int selectedAllayID;
        int selectedEnemyID;
        List<Button> Playerbtns = new();
        List<ProgressBar> playerSideBars = new();
        List<Label> playerSideLabels = new();
        List<Label> enemySideLabels = new();
        List<ProgressBar> enemySideBars = new();
        List<Button> Enemybtns = new();
        List<int> Enabled=new();
        int turnNumber = -1;

        Random r = new();

        public FightSystem(MainWindow main, Character?[] playerSide, Character?[] enemySide, Dictionary<int, IAdditionalSystem> addSys, int nexteventID, int fleeID)
        {
            InitializeComponent();
            _main = main;
            this.playerSide = [.. playerSide];
            this.enemySide = [.. enemySide];
            this.additionalSystems = addSys;
            this.nexteventID = nexteventID;
            this.fleeID = fleeID;
            Playerbtns.Add(Ally0);
            Playerbtns.Add(Ally1);
            Playerbtns.Add(Ally2);
            Enemybtns.Add(EnemyButton0);
            Enemybtns.Add(EnemyButton1);
            Enemybtns.Add(EnemyButton2);
            PlayerSide();
            EnemySide();
            Turn();
        }
        public void ReturnToStory(int id) // Returns to the story with the new data
        {
            Character givePlayer = playerSide[0];
            playerSide.RemoveAt(0);
            _main.SceneChanger(2, new SaveData(givePlayer, [.. playerSide], id, _main.xmlPath, _main.unavailableChoicheIDs.ToArray()));
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
            Random r = new Random();
            for (int i = 0; i < playerSide.Count; i++)
            {
                var label = PlayerSidePanel.FindName($"label{i}") as Label;
                label.Content = $"{playerSide[i].Name}: {playerSide[i].Level}";
                label.Visibility = Visibility.Visible;
                var progressBar = PlayerSidePanel.FindName($"Playerprog{i}") as ProgressBar;
                progressBar.Visibility = Visibility.Visible;
                var btn = FindName($"Ally{i}") as Button;
                TextBlock newBlock = new TextBlock() { Text = $"{playerSide[i].Name}", TextWrapping = TextWrapping.Wrap };
                btn.Content = newBlock;
                btn.Visibility = Visibility.Visible;
                playerSideBars.Add(progressBar);
                playerSideLabels.Add(label);
                int val = i;
                btn.Click += (s, e) => 
                {
                    selectedAllayID = val;
                    if (turnNumber==0)
                    {
                        Enabled.Remove(val);
                    }             
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
                enemySideBars.Add(progressBar);
                enemySideLabels.Add(label);
                var btn = FindName($"EnemyButton{i}") as Button;
                TextBlock newBlock = new TextBlock() { Text = $"{enemySide[i].Name}", TextWrapping = TextWrapping.Wrap };
                btn.Content = newBlock;
                btn.Visibility=Visibility.Visible;
                int val = i;
                btn.Click += (s, e) =>
                {
                    selectedEnemyID = val;
                    if( turnNumber==1)
                    {
                        Enabled.Remove(val);
                        
                    }
                    else if(turnNumber==0)
                    {
                        AllyDamage(selectedEnemyID);
                        Turn();
                    }  
                };
            }   
        }

        public void DiceRoll()
        {
            int checkDice = 8;
            int Rolled = r.Next(1, checkDice);
        }

        public void AllyDamage(int enemyID)
        {
            int damage;
            Character enemy = enemySide[enemyID];
            Character player = playerSide[selectedAllayID];
            player.Stats.TryGetValue("Strength", out int strg);
            Dictionary<string,Dictionary<string, int>> weap = player.GetWeapon() as Dictionary<string, Dictionary<string, int>>;
            weap.FirstOrDefault().Value.TryGetValue("Damage", out int weapDamage);
            damage = strg + weapDamage;
            enemy.TakeDamage(damage);
            UpdateHealthBar(enemyID, turnNumber);
            if(!enemy.Alive)
            {
                RemoveCharacterAtIndex(enemyID, turnNumber);
            }
        }

        public void EnemyDamage(int playerID, int enemyID)
        {
            int damage;
            Character player = playerSide[playerID];
            if (enemyID < enemySide.Count)
            {
                Character enemy = enemySide[enemyID];
                enemy.Stats.TryGetValue("Strength", out int strg);
                Dictionary<string, Dictionary<string, int>> weap = enemy.GetWeapon() as Dictionary<string, Dictionary<string, int>>;
                weap.FirstOrDefault().Value.TryGetValue("Damage", out int weapDamage);
                damage = strg + weapDamage;
                player.TakeDamage(damage);
            }
            UpdateHealthBar(playerID, turnNumber);
            if (!player.Alive)
            {
                RemoveCharacterAtIndex(playerID, turnNumber);
            }
            EnemyTurn();
        }
        public void Heal(object senger, RoutedEventArgs dwa)// már van objclass-ba
        {
            int HealRestore = 20; // Random Heal number, going to depend on the potion
            playerSide[selectedAllayID].Heal(HealRestore);
            UpdateHealthBar(selectedAllayID,1);
            Turn();

        }
        public void PopP()
        {
            for (int i = 0; i < playerSide.Count; i++)
            {
                Enabled.Add(i);
            }
        }

        public void EnP()
        {
            for (int i = 0; i < enemySide.Count; i++)
            {
                Enabled.Add(i);
            }
        }
        public void Turn()
        {
            if(turnNumber==-1 || turnNumber==1)
            {
                PopP();
                turnNumber = 0;
            }
            if(Enabled.Count>0)
            {
                PlayerSelected(true);
                ToggleActionBtns(false);
                ToggleEnemy(false);
            }
            else
            {
                EnemyTurn();
            }
            
        }

        public void EnemyTurn()
        {
            if (turnNumber == 0)
            {
                turnNumber = 1;
                for (int i = 0; i < enemySide.Count; i++)
                {
                    if (playerSide.Count > 0 && enemySide[i].Alive)
                    {
                        selectedAllayID = r.Next(0, playerSide.Count);
                        EnemyDamage(selectedAllayID, i);
                    }

                }
                Turn(); 
            }
            
        }        

        public void ToggleActionBtns(bool changedTo)
        {
            bool whatTO = changedTo;
            Attackbutton.IsEnabled = whatTO;
            Healbutton.IsEnabled = whatTO;
            Defendbutton.IsEnabled = whatTO;
            Fleebutton.IsEnabled = whatTO;
            
        }

        public void ToggleEnemy(bool changedTo)
        {
            for (int i = 0; i < Enemybtns.Count; i++)
            {
                if(Enabled.Contains(i) && turnNumber == 1 || turnNumber == 0)
                {
                    Enemybtns[i].IsEnabled = changedTo;
                }
               
            }
        }

        public void PlayerSelected(bool changedTo)
        {
            for (int i = 0; i < Playerbtns.Count; i++)
            {
                if (Enabled.Contains(i) && turnNumber == 0 || turnNumber == 1)
                {
                    Playerbtns[i].IsEnabled = changedTo;
                }  
            }

        }

        private void Ally_Click(object sender, RoutedEventArgs e)
        {
            ToggleActionBtns(true);
            PlayerSelected(false);
        }

        
        private void Attackbutton_Click(object sender, RoutedEventArgs e)
        {
            ToggleActionBtns(false);
            ToggleEnemy(true);
        }
        private void Defendbutton_Click(object sender, RoutedEventArgs e)
        {
            ToggleActionBtns(false);     
            Turn();
        }
        private void Fleebutton_Click(object sender, RoutedEventArgs e)
        {
            ReturnToStory(fleeID);
        }

        private void Enemy_click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateHealthBar(int index, int sideAttacking)
        {
            switch (sideAttacking)
            {
                case 0:
                    enemySideBars[index].Value = enemySide[index].CurrentHP;
                    break;
                case 1:
                    playerSideBars[index].Value = playerSide[index].CurrentHP;
                    break;
            }
        }

        private void RemoveCharacterAtIndex(int index, int sideAttacking)
        {
            switch (sideAttacking)
            {
                case 0:
                    //enemySide.RemoveAt(index);
                    enemySideBars[index].Visibility = Visibility.Collapsed;
                    Enemybtns[index].Visibility = Visibility.Collapsed;
                    enemySideLabels[index].Visibility = Visibility.Collapsed;
                    CheckEnemyNum();
                    break;
                case 1:
                    if (index == 0)
                    {
                        MessageBox.Show("You died!");
                        turnNumber = -2;
                        _main.SceneChanger(0, null);
                    }
                    //playerSide.RemoveAt(index);
                    playerSideBars[index].Visibility = Visibility.Collapsed;
                    Playerbtns[index].Visibility = Visibility.Collapsed;
                    playerSideLabels[index].Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void CheckEnemyNum()
        {
            int alive = 0;
            for (int i = 0; i < enemySide.Count; i++)
            {
                if (enemySide[i].Alive)
                {
                    alive++;
                }
            }
            if (alive < 1)
            {
                MessageBox.Show("You won!");
                ReturnToStory(nexteventID);
            }
        }
    }
}
