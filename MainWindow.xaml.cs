using InterfaceClass;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.DirectoryServices.ActiveDirectory;
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
        public string dbPath, xmlPath, storyFolder, fileName, saveFolder = "..\\..\\..\\SavedGames\\", imgLibraryFile;
        public static Assembly imagesDLL;
        public List<int> unavailableChoicheIDs = new();

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
            MainGrid.Children.Add(panel);*/
            
        }

        public static void LoadDLL(string path, ref Dictionary<int, IAdditionalSystem> additionalSystems)
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

                additionalSystems.Add(newSystem.GetID(), newSystem);
            }
        }

        public static void LoadImgDLL(string path)
        {
            Assembly loadedDLL = Assembly.LoadFrom(path);

            imagesDLL = loadedDLL;
        }

        public void SceneChanger(int num, object? arguments)
        {
            switch (num)
            {
                case 0: // Loads main menu
                    OurWindow.Content = new MainMenu(this);
                    break;
                case 1: // Loads Character customizer
                    OurWindow.Content = new CharacterCustomizer(this, new LoadCharacterCreatorObj(["Fighter", "Wizard", "Monk"], ["Ork", "High-elf", "Goblin", "Human"], ["Strength", "Dexterity", "Intelligance"]));
                    break;
                case 2: // Loads Events
                    if (arguments != null && arguments is LoadNewStory)
                    {
                        LoadNewStory specifiedObj = arguments as LoadNewStory;
                        xmlPath = specifiedObj.dbPath;
                        SetFilePaths();
                        OurWindow.Content = new EventsScreen(this);
                    }
                    else if (arguments != null && arguments is SaveData)
                    {
                        SaveData specifiedObj = arguments as SaveData;
                        xmlPath = specifiedObj.XMLpath ?? "";
                        SetFilePaths();
                        unavailableChoicheIDs = [.. specifiedObj.UnusableIDs];
                        OurWindow.Content = new EventsScreen(this, specifiedObj);
                    }
                    break;
                case 3: // Loads Fight system
                    if (arguments != null && arguments is LoadFightScene)
                    {
                        Dictionary<int, IAdditionalSystem> additionalSystems = [];
                        XDocument doc = XDocument.Load(xmlPath);
                        var systemPaths = doc.Root.Descendants("PathLinks").Descendants("LogicSystem").ToArray();

                        foreach (var sytemPath in systemPaths)
                        {
                            LoadDLL(storyFolder + "/" + sytemPath.Attribute("Path").Value, ref additionalSystems);
                        }

                        LoadFightScene loadFightScene = arguments as LoadFightScene;
                        OurWindow.Content = new FightSystem(this, loadFightScene.playerSide, loadFightScene.enemySide, additionalSystems, loadFightScene.nextEventID);

                    }
                    break;
            }
        }

        public void SetFilePaths()
        {
            XDocument doc = XDocument.Load(xmlPath);

            string _dbPath = doc.Root.Descendants("PathLinks").Descendants("StoryDatabase").Attributes("Path").Select(x => x.Value).FirstOrDefault();
            string _imgLib = doc.Root.Descendants("PathLinks").Descendants("ImageFile").Attributes("Path").Select(x => x.Value).FirstOrDefault();

            storyFolder = System.IO.Path.GetDirectoryName(xmlPath);

            dbPath = System.IO.Path.Combine(storyFolder, _dbPath);
            imgLibraryFile = System.IO.Path.Combine(storyFolder, _imgLib);
            LoadImgDLL(imgLibraryFile);
        }

        public BitmapImage GetImageAtIndex(string name)
        {
            string imgpath = $"pack://application:,,,/{imagesDLL.GetName().Name};component/{name}.jpg";
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            try
            {
                bitmap.UriSource = new Uri(imgpath);
                bitmap.EndInit();
                return bitmap;
            }
            catch (Exception)
            {
                return new BitmapImage();
            }
        }
        public void ClearData()
        {
            dbPath = null;
            xmlPath = null;
            unavailableChoicheIDs.Clear();
            fileName = null;
            imgLibraryFile = null;
        }
    }
}