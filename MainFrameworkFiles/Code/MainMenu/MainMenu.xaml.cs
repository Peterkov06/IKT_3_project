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
            GetStories(path);
            GetSaves(_main.saveFolder);
            ShowArray(filePathgs, Available_Stories);
            ShowArray(saves, Available_saves);
            ContinueStory.Click += (s, e) => {
                var element = Available_saves.SelectedItem as ListBoxItem;
                string selectedPath = element.Content.ToString();
                _main.fileName = selectedPath.Split('\\').Last();
                _main.SceneChanger(2, GameSaver.LoadGame(selectedPath)); };
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
