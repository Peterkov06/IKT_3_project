using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using System.Xml.Linq;
using InterfaceClass;
using System.Reflection.PortableExecutable;
using System.Data.Entity.Core.Metadata.Edm;

namespace IKT_3_project
{
    /// <summary>
    /// Interaction logic for EventsScreen.xaml
    /// </summary>
    public partial class EventsScreen : UserControl
    {
        MainWindow _main;
        public Dictionary<int, Func<string, int, object?>> ParameterMethods = [];
        public Dictionary<int, Func<string, int, object?>> EventMethods = [];
        Character player;
        ICharacter?[] teamMates;
        int currentEventID = 0;
        public EventsScreen(MainWindow main, SaveData state)
        {
            _main = main;
            InitializeComponent();
            player = state.player;
            teamMates = state.teammates;
            currentEventID = state.eventID;

            if (_main.dbPath == null)
            {
                SetFilePaths();
            }

            EventMethods.Add(1, StartFight);
            EventMethods.Add(2, GiveHPToPlayer);
            EventMethods.Add(3, GiveStatToPlayer);
            EventMethods.Add(4, GiveItemToPlayer);
            EventMethods.Add(5, GiveBuffToPlayer);

            ParameterMethods.Add(1, GetHPFromPlayer);
            ParameterMethods.Add(2, GetStatFromPlayer);
            ParameterMethods.Add(3, GetItemFromPlayer);
            ParameterMethods.Add(4, GetFromDB);
            ParameterMethods.Add(5, GetPlayerLevel);
            ParameterMethods.Add(6, GetPlayerClass);
            ParameterMethods.Add(7, GetPlayerRace);

            SaveBtn.Click += (s, e) => { GameSaver.SaveGame(player, teamMates, currentEventID, main.xmlPath, System.IO.Path.Combine("..\\..\\..\\SavedGames\\", main.fileName), [.. _main.unavailableChoicheIDs]); };

            GeneratePart(state.eventID);

        }
        public EventsScreen(MainWindow main)
        {
            _main = main;
            InitializeComponent();
            player = new("grg", "Fighter", "fwf3w", 20, 500, [], [], []);
            teamMates = [];

            SetFilePaths();

            EventMethods.Add(1, StartFight);
            EventMethods.Add(2, GiveHPToPlayer);
            EventMethods.Add(3, GiveStatToPlayer);
            EventMethods.Add(4, GiveItemToPlayer);
            EventMethods.Add(5, GiveBuffToPlayer);

            ParameterMethods.Add(1, GetHPFromPlayer);
            ParameterMethods.Add(2, GetStatFromPlayer);
            ParameterMethods.Add(3, GetItemFromPlayer);
            ParameterMethods.Add(4, GetFromDB);
            ParameterMethods.Add(5, GetPlayerLevel);
            ParameterMethods.Add(6, GetPlayerClass);
            ParameterMethods.Add(7, GetPlayerRace);

            player.Inventory.Add("Dagger", new Dictionary<string, int> { { "MinDamage", 10 }, { "MaxDamage", 40 }, { "Weapon", 1 } });
            player.Stats.Add("Strength", 20);

            LoadCharaterCreator.Click += (s, e) => { _main.SceneChanger(1, null); };
            LoadFight.Click += (s, e) => { _main.SceneChanger(3, new LoadFightScene([ player ,new Character("grg", "fesfg", "fwf3w", 3, 500, [], [], []), new Character("grg", "fesfg", "fwf3w", 3, 500, [], [], [])], [new Character("enemy1", "fesfg", "fwf3w", 3, 500, [], [], []), new Character("enemy2", "fesfg", "fwf3w", 3, 500, [], [], [])], 2)); };

            SaveBtn.Click += (s, e) => { GameSaver.SaveGame(player, teamMates, currentEventID, main.xmlPath, System.IO.Path.Combine("..\\..\\..\\SavedGames\\", main.fileName), [.. _main.unavailableChoicheIDs]); };
            GeneratePart(1);

        }

        private void SetFilePaths()
        {
            XDocument doc = XDocument.Load(_main.xmlPath);

            string _dbPath = doc.Root.Descendants("PathLinks").Descendants("StoryDatabase").Attributes("Path").Select(x => x.Value).FirstOrDefault();

            _main.storyFolder = System.IO.Path.GetDirectoryName(_main.xmlPath);

            _main.dbPath = System.IO.Path.Combine(_main.storyFolder, _dbPath);
        }

        public void GeneratePart(int ind)
        {
            currentEventID = ind;
            MainGrid.Children.Clear();
            string connString = $"Data Source={_main.dbPath};Version=3;";
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string sqlComm = $"SELECT text FROM Parts Where id = {ind}";
                using (SQLiteCommand cmd = new(sqlComm, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            WrapPanel textWrapper = new();
                            TextBlock test = new() { Text = reader.GetString(0), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(5), FontSize = 16 };
                            textWrapper.Children.Add(test);
                            MainGrid.Children.Add(textWrapper);
                        }
                    }

                    string sqlComm2 = $"SELECT text, next_part, method, parameters, choice_condition, is_retakable FROM Choiches Where part_id = {ind}";
                    StackPanel panel = new StackPanel();
                    Grid.SetColumn(panel, 1);

                    using (SQLiteCommand opts = new SQLiteCommand(sqlComm2, conn))
                    {
                        using (SQLiteDataReader r2 = opts.ExecuteReader())
                        {
                            while (r2.Read())
                            {
                                bool validOption = false;
                                if (r2.IsDBNull(4) && r2.IsDBNull(5))
                                {
                                    validOption = true;
                                }
                                else
                                {
                                    string json;
                                    if (r2.IsDBNull(4))
                                    {
                                        json = "";
                                    }
                                    else
                                    {
                                        json = r2.GetString(4);
                                    }
                                    validOption = IsValidChoice(json, ind);
                                }
                                if (validOption)
                                {
                                    Button button = new Button
                                    {
                                        Content = r2.GetString(0),
                                        FontSize = 12,
                                        Margin = new Thickness(5),
                                    };

                                    if (r2.IsDBNull(2))
                                    {
                                        int next_id = r2.GetInt32(1);
                                        button.Click += (sender, e) => { GeneratePart(next_id); };
                                    }
                                    else
                                    {
                                        int methodNum = Convert.ToInt32(r2.GetString(2));
                                        int? next_id = null;
                                        string json = r2.GetString(3);
                                        if (!r2.IsDBNull(1))
                                        {
                                            next_id = r2.GetInt32(1);
                                        }

                                        button.Click += (sender, e) =>
                                        {
                                            try
                                            {
                                                EventMethods[methodNum].Invoke(json, methodNum);

                                                if (next_id != null)
                                                {
                                                    GeneratePart((int)next_id);
                                                }
                                            }
                                            catch (Exception)
                                            {
                                            }
                                            //MessageBox.Show($"Player HP: {player.HP}");         
                                        };
                                    }

                                    if (!r2.IsDBNull(5))
                                    {
                                        button.Click += (sender, e) => { _main.unavailableChoicheIDs.Add(ind); };
                                    }

                                    panel.Children.Add(button);
                                }
                            }
                            MainGrid.Children.Add(panel);
                        }
                    }
                }
            }
        }
    }
}
