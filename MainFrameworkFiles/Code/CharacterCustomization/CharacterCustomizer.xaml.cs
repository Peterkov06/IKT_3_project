using System;
using System.Collections.Generic;
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
        public CharacterCustomizer(MainWindow main, LoadCharacterCreatorObj choosableOptions)
        {
            _main = main;
            InitializeComponent();
            classes = choosableOptions.classes;
            races = choosableOptions.races;
            stats = choosableOptions.stats;
        }
    }
}
