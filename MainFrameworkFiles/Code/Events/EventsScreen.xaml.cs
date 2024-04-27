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
        List<Character?> teamMates;
        int currentEventID = 0;
        public EventsScreen(MainWindow main, SaveData state)
        {
            _main = main;
            InitializeComponent();
            player = state.player;
            teamMates = state.teammates.ToList();
            currentEventID = state.eventID;

            //DB: method
            EventMethods.Add(1, StartFight); // param: combat ID
            EventMethods.Add(2, GiveHPToPlayer);
            EventMethods.Add(3, GiveStatToPlayer);
            EventMethods.Add(4, GiveItemToPlayer);
            EventMethods.Add(5, GiveBuffToPlayer);
            EventMethods.Add(6, AllyConstructor);

            //DB: parameter_method
            ParameterMethods.Add(1, GetHPFromPlayer);
            ParameterMethods.Add(2, GetStatFromPlayer);
            ParameterMethods.Add(3, GetItemFromPlayer);
            ParameterMethods.Add(4, GetPlayerLevel);
            ParameterMethods.Add(5, GetPlayerClass);
            ParameterMethods.Add(6, GetPlayerRace);

            SaveBtn.Click += (s, e) => { GameSaver.SaveGame(player, teamMates.ToArray(), currentEventID, main.xmlPath, System.IO.Path.Combine(_main.saveFolder, main.fileName), [.. _main.unavailableChoicheIDs]); };
            MainMenuBtn.Click += (s, e) => {
                _main.ClearData();
                _main.SceneChanger(0, null);
            };
            ShowPlayerStats();
            GeneratePart(state.eventID);

        }
        public EventsScreen(MainWindow main, BeginNewStory newStory)
        {
            _main = main;
            InitializeComponent();
            player = newStory.player;
            teamMates = [];

            //DB: method
            EventMethods.Add(1, StartFight); // param: combat ID
            EventMethods.Add(2, GiveHPToPlayer);
            EventMethods.Add(3, GiveStatToPlayer);
            EventMethods.Add(4, GiveItemToPlayer);
            EventMethods.Add(5, GiveBuffToPlayer);
            EventMethods.Add(6, AllyConstructor);

            //DB: parameter_method
            ParameterMethods.Add(1, GetHPFromPlayer);
            ParameterMethods.Add(2, GetStatFromPlayer);
            ParameterMethods.Add(3, GetItemFromPlayer);
            ParameterMethods.Add(4, GetPlayerLevel);
            ParameterMethods.Add(5, GetPlayerClass);
            ParameterMethods.Add(6, GetPlayerRace);

            SaveBtn.Click += (s, e) => { GameSaver.SaveGame(player, teamMates.ToArray(), currentEventID, main.xmlPath, System.IO.Path.Combine(_main.saveFolder, main.fileName), [.. _main.unavailableChoicheIDs]); };
            MainMenuBtn.Click += (s, e) => {
                _main.ClearData();
                _main.SceneChanger(0, null);
            };
            ShowPlayerStats();
            GeneratePart(1);

        }

        public void GeneratePart(int ind)
        {
            currentEventID = ind;
            MainGrid.Children.Clear();
            TextUndIMG.Children.Clear();
            string connString = $"Data Source={_main.dbPath};Version=3;";
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string sqlComm = $"SELECT text, image_name FROM Parts Where id = {ind}";
                using (SQLiteCommand cmd = new(sqlComm, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            WrapPanel textWrapper = new();
                            TextBlock test = new() { Text = reader.GetString(0), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(5), FontSize = 16 };
                            textWrapper.Children.Add(test);
                            TextUndIMG.Children.Add(textWrapper);
                            if (!reader.IsDBNull(1))
                            {
                                Image image = new Image();
                                image.BeginInit();
                                image.Source = _main.GetImageAtIndex(reader.GetString(1));
                                image.EndInit();
                                image.Margin = new Thickness(5);
                                image.Stretch = Stretch.Uniform;
                                Grid.SetColumn(image, 0);
                                Grid.SetRow(image, 1);

                                TextUndIMG.Children.Add(image);
                            }
                            MainGrid.Children.Add(TextUndIMG);
                        }
                    }
                    string sqlComm2 = $"SELECT text, next_part, method, parameters, choice_condition, is_retakable, tooltip FROM Choiches Where part_id = {ind}";
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
                                    if(!r2.IsDBNull(6))
                                    {
                                        string tooltipText = $"{r2.GetString(6)}";
                                        ToolTip ttp = new();
                                        ttp.Content = tooltipText;
                                        ttp.FontSize = 14;
                                        ttp.FontStyle = FontStyles.Italic;
                                        button.ToolTip = ttp;
                                    }

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

        public void ShowPlayerStats()
        {
            if (player.b64IconString != "")
            {
                BitmapImage bitmapImage = MainWindow.BMPimgFormB64(player.b64IconString);
                Image icon = new Image() { Source = bitmapImage, Stretch = Stretch.Uniform, Margin = new(5,0,5,0)};
                PlayerIcon.Children.Add(icon);
            }
            foreach (var item in _main.statsToShow)
            {
                Label? lbl = null;
                switch (item)
                {
                    case "Name":
                        lbl = new Label() { Content = $"{player.Name}", FontSize = 18 };
                        break;
                    case "HP":
                        lbl = new Label() { Content=$"HP: {player.CurrentHP}", FontSize=16};
                        break;
                    default:
                        bool got = player.Stats.TryGetValue(item, out int value);
                        if (got)
                        {
                            lbl = new() { Content = $"{item}:{value}", FontSize = 16 };
                        }
                        break;
                }
                if (lbl != null)
                {
                    PlayerStatBar.Children.Add(lbl);
                }
            }
            Button inventoryBtn = new() { Content = "Inventory", Margin = new Thickness(5) };
            inventoryBtn.Click += (sender, e) => { InventoryWindow invWndw = new(ref player); invWndw.ShowDialog(); };
            PlayerStatBar.Children.Add(inventoryBtn);
        }
    }
}
