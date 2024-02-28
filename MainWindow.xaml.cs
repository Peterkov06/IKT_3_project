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
        public Dictionary<int, IAdditionalSystem> instances = new();

        public delegate void ChangeScene(int sceneNum, object? arguments);
        public event ChangeScene ChangeSceneEvent;

        public MainWindow()
        {
            InitializeComponent();
            ChangeSceneEvent += SceneChanger;
            ChangeSceneEvent.Invoke(0, null);
            /*EventMethods.Add(1, GiveToPlayer);

            ParameterMethods.Add(1, GetFromPlayer);
            ParameterMethods.Add(2, GetFromDB);

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
            MainGrid.Children.Add(panel);

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
            player.Stats.Add("Strength", 20);*/
            
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

        

        public void SceneChanger(int num, object? arguments)
        {
            switch (num)
            {
                case 0: // Loads main menu
                    OurWindow.Content = new MainMenu(this);
                    break;
                case 1: // Loads Character customizer
                    OurWindow.Content = new CharacterCustomizer(this);
                    break;
                case 2: // Loads Events
                    if (arguments != null && arguments is LoadNewStory)
                    {
                        LoadNewStory specifiedObj = arguments as LoadNewStory;
                        string path = specifiedObj.dbPath;
                        OurWindow.Content = new EventsScreen(path, this);
                    }
                    break;
                case 3: // Loads Fight system
                    OurWindow.Content = new FightSystem(this);
                    break;
            }
        }
    }
}