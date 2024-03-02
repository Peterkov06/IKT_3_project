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

namespace IKT_3_project
{
    /// <summary>
    /// Interaction logic for EventsScreen.xaml
    /// </summary>
    public partial class EventsScreen : UserControl
    {
        MainWindow _main;
        public Dictionary<int, Func<string, int, object?>> ParameterMethods = new();
        public Dictionary<int, Func<string, int, object?>> EventMethods = new();
        Player player;
        ICharacter?[] teamMates;
        public EventsScreen(MainWindow main, BackToStory state)
        {
            _main = main;
            InitializeComponent();
            player = state.player;
            teamMates = state.teammates;


            EventMethods.Add(1, GiveToPlayer);

            ParameterMethods.Add(1, GetFromPlayer);
            ParameterMethods.Add(2, GetFromDB);

            GeneratePart(state.eventID);

        }
        public EventsScreen(MainWindow main)
        {
            _main = main;
            InitializeComponent();
            player = new("grg", "fesfg", "fwf3w", 3, 500, new(), new(), new());

            XDocument doc = XDocument.Load(_main.xmlPath);

            string _dbPath = doc.Root.Descendants("PathLinks").Descendants("StoryDatabase").Attributes("Path").Select(x => x.Value).FirstOrDefault();

            _main.storyFolder = System.IO.Path.GetDirectoryName(main.xmlPath);

            _main.dbPath = System.IO.Path.Combine(_main.storyFolder, _dbPath);

            EventMethods.Add(1, GiveToPlayer);

            ParameterMethods.Add(1, GetFromPlayer);
            ParameterMethods.Add(2, GetFromDB);

            player.Inventory.Add("Weapon", new Dictionary<string, int> { { "MinDamage", 10 }, { "MaxDamage", 40 } });
            player.Stats.Add("Strength", 20);

            GeneratePart(1);

        }
        public void GeneratePart(int ind)
        {
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

                    string sqlComm2 = $"SELECT text, next_part, method, parameters FROM Choiches Where part_id = {ind}";
                    StackPanel panel = new StackPanel();
                    Grid.SetColumn(panel, 1);

                    using (SQLiteCommand opts = new SQLiteCommand(sqlComm2, conn))
                    {
                        using (SQLiteDataReader r2 = opts.ExecuteReader())
                        {
                            while (r2.Read())
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
                                            JObject jsObj = JsonConvert.DeserializeObject<JObject>(json);
                                            IEnumerable<string> keys = jsObj.Properties().Select(p => p.Name);

                                            List<object> parameters = new();
                                            foreach (string key in keys) // All keys in the object (the different kind of parameters getter method id)
                                            {
                                                if (jsObj[key].Type == JTokenType.Array) // If the parameter argument is an array of requests
                                                {
                                                    foreach (var value in jsObj[key]) // Value = array element
                                                    {
                                                        if (value.Type == JTokenType.Object) // If the value is an object
                                                        {
                                                            JObject keyValuePairs = value as JObject; // Set it as one
                                                            string[] elementProps = keyValuePairs.Properties().Select(p => p.Name).ToArray(); // Keys in the object (MUST be 1), the name of the obj (like sword)
                                                            if (value[elementProps[0]].Type == JTokenType.Array) // If the required object has multiple properties (like damage)
                                                            {
                                                                foreach (var itemProp in value[elementProps[0]])
                                                                {
                                                                    Dictionary<string, int> item = (Dictionary<string, int>)ParameterMethods[1](elementProps[0], Convert.ToInt32(key));
                                                                    MessageBox.Show($"Got back: ({elementProps[0]}) ({(string)itemProp}) {item[(string)itemProp]}");
                                                                    parameters.Add(item[(string)itemProp]);
                                                                }
                                                            }
                                                        }
                                                        else if (value.Type == JTokenType.String) // If the array only contains property names
                                                        {
                                                            MessageBox.Show($"Got back: ({value}) {ParameterMethods[1]((string)value, Convert.ToInt32(key))}");
                                                            parameters.Add(ParameterMethods[1]((string)value, Convert.ToInt32(key)));
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show($"Got back: ({(string)jsObj[key]}) {ParameterMethods[1]((string)jsObj[key], Convert.ToInt32(key))}");
                                                    parameters.Add(ParameterMethods[1]((string)jsObj[key], Convert.ToInt32(key)));
                                                }
                                            }

                                            if (next_id != null)
                                            {
                                                GeneratePart((int)next_id);
                                            }
                                        }
                                        catch (Exception)
                                        { }
                                        //MessageBox.Show($"Player HP: {player.HP}");         
                                    };
                                }

                                panel.Children.Add(button);
                            }
                            MainGrid.Children.Add(panel);
                        }
                    }
                }
            }
        }
    }
}
