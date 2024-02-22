using InterfaceClass;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
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
using System.Xml.Linq;

namespace IKT_3_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string dbPath;
        public Player player;
        public Dictionary<int, IAdditionalSystem> instances = new();
        public Dictionary<int, Func<string, int,object?>> TestMethodDIct = new();

        public MainWindow()
        {
            InitializeComponent();

            TestMethodDIct.Add(1, GetFromPlayer);
            TestMethodDIct.Add(2, GiveToPlayer);
            TestMethodDIct.Add(3, GetFromDB);

            string path = "..\\..\\..\\Launcher\\UIxml.xml";
            XDocument doc = XDocument.Load(path);
            /*
            var parentTag = doc.Root.Descendants("MainMenu").FirstOrDefault();

            foreach (XElement element in parentTag.Descendants("Button"))
            {
                Button button = new Button { Name = (string)element.Attribute("Name"),
                    Content = (string)element.Attribute("Content"),
                    Margin = new Thickness(5),
                };
                string methodName = (string)element.Attribute("OnClick");
                MethodInfo method = GetType().GetMethod(methodName);
                button.Click += (sender, e) => { method.Invoke(this, new object[] { sender, e }); };
                panel.Children.Add(button);
            }
            MainGrid.Children.Add(panel);*/

            dbPath = doc.Root.Descendants("PathLinks").Descendants("StoryDatabase").Attributes("Path").Select(x=>x.Value).FirstOrDefault();
            GeneratePart(1);
            player = new(100);

            var systemPaths = doc.Root.Descendants("PathLinks").Descendants("LogicSystem").ToArray();

            foreach (var sytemPath in systemPaths)
            {
                LoadDLL(sytemPath.Attribute("Path").Value);
            }

            doc.Save(path);

            player.Inventory.Add("Weapon", new Dictionary<string, int> { { "MinDamage", 10 }, { "MaxDamage", 40 } });
            player.Stats.Add("Strength", 20);
            
        }

        public void LoadDLL(string path)
        {
            Assembly loadedDLL = Assembly.LoadFrom(path);
            Type[] type = loadedDLL.GetTypes();

            foreach (Type t in type)
            {
                if (t.IsInterface || !t.IsClass)
                {
                    continue; // Skip interfaces and non-class types
                }

                if (!t.GetInterfaces().Contains(typeof(IAdditionalSystem)))
                {
                    continue; // Skip types not implementing IAdditionalSystem
                }


                IAdditionalSystem newSystem = (IAdditionalSystem)Activator.CreateInstance(t);

                instances.Add(newSystem.GetID(), newSystem);
            }
        }

        public void GeneratePart(int ind)
        {
            MainGrid.Children.Clear();
            string connString = $"Data Source={dbPath};Version=3;";
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
                                                                    Dictionary<string, int> item = (Dictionary<string, int>)TestMethodDIct[1](elementProps[0], Convert.ToInt32(key));
                                                                    MessageBox.Show($"Got back: ({elementProps[0]}) ({(string)itemProp}) {item[(string)itemProp]}");
                                                                    parameters.Add(item[(string)itemProp]);
                                                                }
                                                            }
                                                        }
                                                        else if (value.Type == JTokenType.String) // If the array only contains property names
                                                        {
                                                            MessageBox.Show($"Got back: ({value}) {TestMethodDIct[1]((string)value, Convert.ToInt32(key))}");
                                                            parameters.Add(TestMethodDIct[1]((string)value, Convert.ToInt32(key)));
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show($"Got back: ({(string)jsObj[key]}) {TestMethodDIct[1]((string)jsObj[key], Convert.ToInt32(key))}");
                                                    parameters.Add(TestMethodDIct[1]((string)jsObj[key], Convert.ToInt32(key)));
                                                }
                                            }
                                           
                                            if (next_id != null)
                                            {
                                                GeneratePart((int)next_id);
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            throw;
                                        }                     
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

        public object? GetFromPlayer(string key, int method) // Interaction with the player: Getting some data from "it"
        {
            try
            {
            switch (method)
            {
                case 1:
                    return player.HP;
                case 2:
                        if (player.Buffs.ContainsKey(key))
                        {
                            return player.Stats[key] + player.Buffs[key];
                        }
                    return player.Stats[key];
                case 3:
                    return player.Inventory[key];
            }
            }
            catch (Exception)
            {

            }
            return null;
        }

        public object? GiveToPlayer(string key, int method) // Interaction with the player: Adding sg to "it"
        {
            switch (method)
            {
                case 1:
                    player.HP += Convert.ToInt32(key);
                    return null;
                case 2:
                    //To be implemented: Stat adding
                    JObject stat = JsonConvert.DeserializeObject<JObject>(key);
                    IEnumerable<string> keys = stat.Properties().Select(p => p.Name);
                    foreach (string keyStrg in keys)
                    {
                        player.Stats.Add(keyStrg, (int)stat[keyStrg]);
                    }
                    return null;
                case 3:
                    //To be implemented: Inventory adding
                    JObject items = JsonConvert.DeserializeObject<JObject>(key);
                    IEnumerable<string> itemKeys = items.Properties().Select(p => p.Name);
                    foreach (string keyStrg in itemKeys)
                    {
                        JObject item = (JObject)items[keyStrg];
                        IEnumerable<string> itemData = item.Properties().Select(p => p.Name);
                        Dictionary<string, int> dat = new Dictionary<string, int>();
                        foreach (string dataKeyStrg in itemData)
                        {
                            dat.Add(dataKeyStrg, (int)item[dataKeyStrg]);
                        }
                        player.Inventory.Add(keyStrg, dat);
                    }
                    return null;
                case 4:
                    //To be implemented: Buff adding
                    JObject buff = JsonConvert.DeserializeObject<JObject>(key);
                    IEnumerable<string> buffKeys = buff.Properties().Select(p => p.Name);
                    foreach (string keyStrg in buffKeys)
                    {
                        player.Buffs.Add(keyStrg, (int)buff[keyStrg]);
                    }
                    MessageBox.Show($"{player.Buffs.Count}");
                    return null;
            }
            return null;
        }

        public object? GetFromDB(string key, int method) // Get data out from the DB
        {
            string connString = $"Data Source={dbPath};Version=3;";
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string sqlCommand = "";
                switch (method)
                {
                    case 1: // Get item
                        sqlCommand = $"SELECT * FROM Items WHERE id = {key}";
                        using(SQLiteCommand cmd = new(sqlCommand, conn))
                        {
                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    try
                                    {
                                        JObject attributesJO = JObject.Parse(reader.GetString(2)); // Item attributes read
                                        Dictionary<string, int> attributes = attributesJO.ToObject<Dictionary<string, int>>(); //Turn attributes into a dictionary
                                        return new { Name = $"{reader.GetString(1)}", Attributes = attributes }; // Return the objectified item with it's name and attributes
                                        
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }

                        }
                        break;
                    case 2: // Get Enemy (for fight)
                        sqlCommand = $"SELECT * FROM Enemies WHERE id = {key}";
                        using (SQLiteCommand cmd = new(sqlCommand, conn))
                        {
                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    JObject statsJO = JObject.Parse(reader.GetString(7));
                                    Dictionary<string, int> stats = statsJO.ToObject<Dictionary<string, int>>();
                                    JObject inventoryJO = JObject.Parse(reader.GetString(8));
                                    Dictionary<string, Dictionary<string, int>> inventory = inventoryJO.ToObject<Dictionary<string, Dictionary<string, int>>>();
                                    return new { Name = reader.GetString(1), Class = reader.GetString(2), Race = reader.GetString(3), Level = reader.GetInt32(4) , MaxHP = reader.GetInt32(5), CurrentHP = reader.GetInt32(6), Stats = stats, Inventory = inventory};
                                }
                            }
                            break;
                        }
                }
            }          
            return null;
        }
    }

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