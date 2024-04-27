using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IKT_3_project
{
    
    /// <summary>
    /// Interaction logic for CharacterCustomizer.xaml
    /// </summary>
    public partial class CharacterCustomizer : UserControl
    {
        MainWindow _main;
        public string[] classes; // The choosable classes array
        public string[] races; // The choosable races array
        public string[] stats; // The array of stats that has to be assigned a value to (like Dexterity: 5)
        Dictionary<string, int> statsDict = new();
        string b64ICON = "";
        public CharacterCustomizer(MainWindow main, LoadCharacterCreatorObj choosableOptions)
        {
            _main = main;
            InitializeComponent();
            classes = choosableOptions.classes;
            races = choosableOptions.races;
            stats = choosableOptions.stats;
            GenerateDropdowns();
            confirmBtn.Visibility = Visibility.Collapsed;
        }

        private void GenerateStats_Click(object sender, RoutedEventArgs e)
        {
            GenerateCharacterStats();
        }

        private void RollAgain_Click(object sender, RoutedEventArgs e)
        {
            GenerateCharacterStats();
        }

        void GenerateDropdowns()
        {
            classComboBox.Items.Clear();
            raceComboBox.Items.Clear();
            foreach (var c in classes)
            {
                classComboBox.Items.Add(c);
            }

            foreach (var race in races)
            {
                raceComboBox.Items.Add(race);
            }
        }

        private void ConfirmChoices_Click(object sender, RoutedEventArgs e)
        {
            Character player = new(nameTextBox.Text, classComboBox.Text, raceComboBox.Text, 1, 100, statsDict, new(), new());
            player.b64IconString = b64ICON;
            _main.SceneChanger(2, new BeginNewStory(player));
        }

        private void GenerateCharacterStats()
        {
            initialStatsLabel.Visibility = Visibility.Collapsed;
            statsLabel.Visibility = Visibility.Visible;
            statsDict.Clear();

            Random random = new Random();
            string statsOutput = "Character Stats:\n\n";
            foreach (string stat in new string[] { "Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma" })
            {
                int statValue = random.Next(8, 17);
                statsOutput += $"{stat}: {statValue}\n";
                statsDict.Add(stat, statValue);
            }

            statsLabel.Content = statsOutput;
            confirmBtn.Visibility = Visibility.Visible;
        }

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

                using (MemoryStream ms = new MemoryStream())
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(ms);

                    byte[] imgData = ms.ToArray();
                    b64Strg = Convert.ToBase64String(imgData);
                    b64ICON = b64Strg;
                }
                BitmapImage iconBMP = MainWindow.BMPimgFormB64(b64ICON);
                Image icon = new Image() { Source = iconBMP, MaxHeight=150, MaxWidth=150, Margin= new Thickness(5) };
                IconPlace.Children.Clear();
                IconPlace.Children.Add(icon);
                
            }
        }
    }


}
