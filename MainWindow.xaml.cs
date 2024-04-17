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
        public string[] statsToShow;

        public delegate void ChangeScene(int sceneNum, object? arguments);
        public event ChangeScene ChangeSceneEvent;

        public MainWindow()
        {
            InitializeComponent();
            ChangeSceneEvent += SceneChanger;
            ChangeSceneEvent.Invoke(0, null);
            
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
                    if (arguments != null && arguments is LoadNewStory)
                    {
                        LoadNewStory specifiedObj = arguments as LoadNewStory;
                        xmlPath = specifiedObj.dbPath;
                        SetFilePaths();
                        DataForCharacter(out string[] classes, out string[] races);
                        OurWindow.Content = new CharacterCustomizer(this, new LoadCharacterCreatorObj(classes, races, ["Strength", "Dexterity", "Intelligance"]));
                    }
                    break;
                case 2: // Loads Events
                    if (arguments != null && arguments is BeginNewStory)
                    {
                        BeginNewStory specifiedObj = arguments as BeginNewStory;
                        OurWindow.Content = new EventsScreen(this, specifiedObj);
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
                        OurWindow.Content = new FightSystem(this, loadFightScene.playerSide, loadFightScene.enemySide, additionalSystems, loadFightScene.nextEventID, loadFightScene.fleeID);

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
            if (_imgLib != null)
            {
                imgLibraryFile = System.IO.Path.Combine(storyFolder, _imgLib);
                LoadImgDLL(imgLibraryFile);
            }
            statsToShow = doc.Root.Descendants("UI").Descendants("EventScreen").Descendants("StatBar").Attributes("Elements").Select(x => x.Value).FirstOrDefault().Split(';').ToArray();
        }

        /// <summary>
        /// Searches for a {name} named image file in the imported images dll, if it finds it, returns a BitmapImage.
        /// </summary>
        /// <param name="name">A string, conatining the name of an img (with it's type)</param>
        /// <returns>BitmapImage</returns>
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

        /// <summary>
        /// Creates a BitmapImage out from a base64 type string, used for storing the characters' icons.
        /// </summary>
        /// <param name="b64">A sting, which is a base64 type</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage BMPimgFormB64(string b64)
        {
            byte[] imgDataBack = Convert.FromBase64String(b64);
            using (MemoryStream ms = new(imgDataBack))
            {
                BitmapImage bmp = new();
                bmp.BeginInit();
                bmp.StreamSource = ms;
                bmp.CacheOption = BitmapCacheOption.OnLoad;

                bmp.EndInit();
                return bmp;

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

        public void DataForCharacter(out string[] classes, out string[] races)
        {
            classes = new string[0];
            races = new string[0];
            string connString = $"Data Source={dbPath};Version=3;";
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                List<string> classNames = new();
                List<Dictionary<string, int>> classAttributes = new();
                List<Dictionary<string, int>> raceAttributes = new();
                List<string> raceNames = new();
                conn.Open();
                string sqlCommand = "";
                string[] thingsToGet = ["Classes", "Races"];

                for (int i = 0; i < thingsToGet.Length; i++)
                {
                    sqlCommand = $"SELECT * FROM {thingsToGet[i]}";
                    using (SQLiteCommand cmd = new(sqlCommand, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            JObject attributesJO = JObject.Parse(reader.GetString(2)); // Class attributes read
                                            Dictionary<string, int> attributes = attributesJO.ToObject<Dictionary<string, int>>(); //Turn attributes into a dictionary
                                            classNames.Add(reader.GetString(1));
                                            classAttributes.Add(attributes);
                                            break;
                                        case 1:
                                            JObject raceAttributesJO = JObject.Parse(reader.GetString(2)); // Race attributes read
                                            Dictionary<string, int> raceAttributesc = raceAttributesJO.ToObject<Dictionary<string, int>>(); //Turn attributes into a dictionary
                                            raceNames.Add(reader.GetString(1));
                                            raceAttributes.Add(raceAttributesc);
                                            break;

                                    }
                                    

                                }
                                catch (Exception)
                                {}
                            }
                        }
                    }
                    classes = new string[classNames.Count];
                    races = new string[raceNames.Count];
                    classes = classNames.ToArray();
                    races = raceNames.ToArray();
                }
            }
        }
    }
}