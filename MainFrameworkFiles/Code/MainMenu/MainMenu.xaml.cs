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
        public MainMenu(MainWindow mainWindow)
        {
            _main = mainWindow;
            InitializeComponent();
            string path = "..\\..\\..\\Stories";
            Available_Stories.Items.Clear();
            Available_saves.Items.Clear();
            GetStories(path);
            GetSaves("..\\..\\..\\SavedGames\\");
            ShowArray(filePathgs, Available_Stories);
            ShowArray(saves, Available_saves);
            ContinueStory.Click += (s, e) => { _main.SceneChanger(2, GameSaver.LoadGame()); };
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
            var files = Directory.EnumerateFiles(path, "*.hoi4");
            foreach (var file in files)
            {
                saves.Add(file);
            }
        }

        private void ImportDefFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Story XML definition (*.xml)|*.xml";
            fileDialog.DefaultDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            if (fileDialog.ShowDialog() == true)
            {
                _main.SceneChanger(2, new LoadNewStory(fileDialog.FileName));
            }
        }

        public void LoadCharacterCreator(object sender, RoutedEventArgs e)
        {
            if (Available_Stories.SelectedItem != null)
            {
                var element = Available_Stories.SelectedItem as ListBoxItem;
                string selectedPath = element.Content.ToString();
                _main.SceneChanger(2, new LoadNewStory(selectedPath));
            }
        }
    }
}
