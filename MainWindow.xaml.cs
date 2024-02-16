using InterfaceClass;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.IO;
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
                                
                                if (!r2.IsDBNull(1))
                                {
                                    int next_id = r2.GetInt32(1);
                                    button.Click += (sender, e) => { GeneratePart(next_id); };
                                }
                                else
                                {
                                    int methodNum = Convert.ToInt32(r2.GetString(2));
                                    string json = r2.GetString(3);

                                    button.Click += (sender, e) => 
                                    {
                                        try
                                        {
                                            JObject jsObj = JsonConvert.DeserializeObject<JObject>(json);
                                            IEnumerable<string> keys = jsObj.Properties().Select(p => p.Name);
                                            
                                            List<object> parameters = new();
                                            foreach (string key in keys)
                                            {
                                                if (jsObj[key].Type == JTokenType.Array)
                                                {
                                                    if (jsObj[key][1].Type == JTokenType.Array)
                                                    {
                                                        Dictionary<string, int> item = (Dictionary<string, int>)TestMethodDIct[methodNum](key, (int)jsObj[key][0]);
                                                        
                                                        foreach (string element in jsObj[key][1])
                                                        {
                                                            //MessageBox.Show($"Got back: ({key}) {item[element]}");
                                                             parameters.Add(item[element]);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //MessageBox.Show($"Got back: ({key}) {TestMethodDIct[methodNum](key, (int)jsObj[key])}");
                                                    parameters.Add(TestMethodDIct[methodNum](key, (int)jsObj[key]));
                                                }
                                            }
                                            //MessageBox.Show($"{parameters[0]} {parameters[1]} {parameters[2]}");
                                            //MessageBox.Show($"Damage: {(int)Math.Round((float)instances[3].Execute(parameters.ToArray()))}");
                                            //player.TakeDamage((int)Math.Round((float)instances[3].Execute(parameters.ToArray())));

                                        }
                                        catch (Exception)
                                        {
                                            throw;
                                        }                     
                                        MessageBox.Show($"Player HP: {player.HP}");         
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

        public object? GetFromPlayer(string key, int method)
        {
            switch (method)
            {
                case 1:
                    return player.HP;
                case 2:
                    return player.Stats[key]/* + player.Buffs[key]*/;
                case 3:
                    return player.Inventory[key];
            }
            return null;
        }

        public object? GiveToPlayer(string key, int method)
        {
            switch (method)
            {
                case 1:
                    player.HP += Convert.ToInt32(player.HP);
                    return null;
                case 2:
                    //To be implemented: Stat adding
                    return null;
                case 3:
                    //To be implemented: Inventory adding
                    return null;
                case 4:
                    //To be implemented: Buff adding
                    return null;
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