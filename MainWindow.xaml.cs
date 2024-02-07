using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();

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

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            GeneratePart(1);
            player = new(100);

            doc.Save(path);

            var container = new CompositionContainer();
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("..\\..\\..\\TestStoryFiles"));
            container. = catalog;
        }

        public void Hello(object sender, RoutedEventArgs eventArgs)
        {
            MessageBox.Show("Hello There!");
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
                                    string methodName = r2.GetString(2);
                                    MethodInfo method = GetType().GetMethod(methodName);
                                    int num = Convert.ToInt32(r2.GetString(3));
                                    button.Click += (sender, e) => { method.Invoke(this, new object[] { num }); };
                                }
                                
                                panel.Children.Add(button);
                            }
                            MainGrid.Children.Add(panel);
                        }
                    }
                }
            }
        }

        public void Battle(int combatSit)
        {
            MainGrid.Children.Clear();
            StackPanel panel = new StackPanel();
            Grid.SetColumn(panel, 1);
            int win_id = 0, lose_id = 0;
            string connString = $"Data Source={dbPath};Version=3;";
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string sqlComm = $"SELECT win_part, defeat_part FROM CombatSituations WHERE id = {combatSit}";
                using (SQLiteCommand cmd = new(sqlComm, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            win_id = reader.GetInt32(0);
                            lose_id = reader.GetInt32(1);
                        }
                    }
                }
            }

            Button button = new Button
            {
                Content = "Take Damage",
                FontSize = 12,
                Margin = new Thickness(5),
            };

            button.Click += (sender, e) => { player.TakeDamage(25); };

            Button button2 = new Button
            {
                Content = "Win",
                FontSize = 12,
                Margin = new Thickness(5),
            };
            button2.Click += (sender, e) => { GeneratePart(win_id); };

            panel.Children.Add(button);
            panel.Children.Add(button2);
            MainGrid.Children.Add(panel);
        }
    }

    public class Player
    {
        public int HP { get; set; }

        public Player(int hP)
        {
            HP = hP;
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
        }
    }
}