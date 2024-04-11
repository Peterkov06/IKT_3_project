using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace IKT_3_project
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        MainWindow _main;
        List<string> filePathgs = new List<string>();
        List<string> saves = new List<string>();
        string path = "..\\..\\..\\Stories";
        public MainMenu(MainWindow mainWindow)
        {
            _main = mainWindow;
            InitializeComponent();
            RefreshMainMenu();
            
            Available_saves.SelectionChanged += (s, e) =>
            {
                if (!ContinueStory.IsEnabled && Available_saves.SelectedIndex != -1)
                {
                    NewGameBtn.IsEnabled = false;
                    Available_Stories.SelectedIndex = -1;
                    ContinueStory.IsEnabled = true;
                }
            };
            Available_Stories.SelectionChanged += (s, e) =>
            {
                if (!NewGameBtn.IsEnabled && Available_Stories.SelectedIndex != -1)
                {
                    ContinueStory.IsEnabled = false;
                    Available_saves.SelectedIndex = -1;
                    NewGameBtn.IsEnabled = true;
                }
            };
            ContinueStory.Click += (s, e) => {
                var element = Available_saves.SelectedItem as ListBoxItem;
                string selectedPath = element.Content.ToString();
                _main.fileName = selectedPath.Split('\\').Last();
                _main.SceneChanger(2, GameSaver.LoadGame(selectedPath)); };
        }

        private void RefreshMainMenu()
        {
            Available_Stories.Items.Clear();
            Available_saves.Items.Clear();
            filePathgs.Clear();
            saves.Clear();
            GetStories(path);
            GetSaves(_main.saveFolder);
            ShowArray(filePathgs, Available_Stories);
            ShowArray(saves, Available_saves);
        }

        private void ShowArray(List<string> strings,  ListBox parentElement)
        {
            foreach (var text in strings)
            {
                parentElement.Items.Add(new ListBoxItem() { Content = $"{text}" });
            }
        }

        private void GetStories(string path)
        {
            var directories = Directory.EnumerateDirectories(path);
            foreach (var directory in directories)
            {
                GetStories(directory);
            }

            var files = Directory.EnumerateFiles(path, "*.xml");
            foreach (var file in files)
            {
                filePathgs.Add(file);
            }
        }

        private void GetSaves(string path)
        {
            var files = Directory.EnumerateFiles(path, "*.hoi5");
            foreach (var file in files)
            {
                saves.Add(file);
            }
        }

        private void ImportDefFile(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog folderDialogue = new OpenFolderDialog();
            folderDialogue.Multiselect = false;
            if (folderDialogue.ShowDialog() == true)
            {
                Directory.Move(folderDialogue.FolderName, $"..\\..\\..\\Stories\\{folderDialogue.SafeFolderName}");
            }
            RefreshMainMenu();
        }

        //When Importing the img of the character
        private void ImportIMG(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = false;
            string b64Strg;
            if (file.ShowDialog() == true)
            {
                string imgpath = file.FileName;
                BitmapImage bitmap = new();
                bitmap.BeginInit();

                bitmap.UriSource = new Uri(imgpath);
                bitmap.EndInit();
                
                using(MemoryStream ms = new MemoryStream())
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(ms);

                    byte[] imgData = ms.ToArray();
                    b64Strg = Convert.ToBase64String(imgData);
                }

                    byte[] imgDataBack = Convert.FromBase64String(b64Strg);
                using (MemoryStream ms = new(imgDataBack))
                {
                    BitmapImage bmp = new();
                    bmp.BeginInit();
                    bmp.StreamSource = ms;
                    bmp.CacheOption = BitmapCacheOption.OnLoad;

                    bmp.EndInit();
                    Image img = new Image();
                    img.BeginInit();
                    img.Source = bmp;
                    img.EndInit();
                    img.Margin = new Thickness(5);
                    img.Stretch = Stretch.Uniform;
                    selucaSecondus.Children.Add(img);

                }
                    

            }
        }

        public void LoadCharacterCreator(object sender, RoutedEventArgs e)
        {
            if (Available_Stories.SelectedItem != null)
            {
                var element = Available_Stories.SelectedItem as ListBoxItem;
                string selectedPath = element.Content.ToString();
                Popup enterFileName = new Popup() { IsOpen = true, Placement = PlacementMode.Center  };
                Label text = new Label() { Content = "Gave a name to the save", FontSize=20 };
                TextBox fileNameBox = new() { Margin = new Thickness(15), Text = $"{selectedPath.Split('\\').Last().Split('.').First()}_save001.hoi5" };
                Button continueBtn = new Button() { Content = "Continue", Margin = new Thickness(15) };
                continueBtn.Click += (s,e) => {
                    
                    if (fileNameBox.Text.Length > 0)
                    {
                        _main.fileName = fileNameBox.Text;
                    }
                    _main.SceneChanger(2, new LoadNewStory(selectedPath));
                };
                Button backBtn = new Button() { Content = "Back", Margin = new Thickness(15) };
                backBtn.Click += (s,e) => { MainGrid.Children.Remove(enterFileName); };
                Border border = new Border() { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(1) };
                StackPanel panel = new StackPanel() {Background = new SolidColorBrush(Colors.White), MinHeight=100, MinWidth=160 };
                panel.Children.Add(text);
                panel.Children.Add(fileNameBox);
                panel.Children.Add(backBtn);
                panel.Children.Add(continueBtn);
                border.Child = panel;
                enterFileName.Child = border;
                MainGrid.Children.Add(enterFileName);
            }
        }

    }
}
