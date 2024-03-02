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
        public MainMenu(MainWindow mainWindow)
        {
            _main = mainWindow;
            InitializeComponent();
            string path = "..\\..\\..\\Stories";
            Available_Stories.Items.Clear();
            GetStories(path);
            foreach (var story in filePathgs)
            {
                Available_Stories.Items.Add(new ListBoxItem() { Content = $"{story}" });
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

        private void LoadCharacterCreator(object sender, RoutedEventArgs e)
        {
            if (Available_Stories.SelectedItem != null)
            {
                var element = Available_Stories.SelectedItem as ListBoxItem;
                string selectedPath = element.Content.ToString();
                _main.SceneChanger(2, new LoadNewStory(selectedPath));
                _main.SceneChanger(3, new LoadFightScene([new Player("grg", "fesfg", "fwf3w", 3, 500, new(), new(), new()), new Player("grg", "fesfg", "fwf3w", 3, 500, new(), new(), new())], [] ));
            }
        }
    }
}
