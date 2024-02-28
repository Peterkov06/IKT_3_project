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
        public CharacterCustomizer(MainWindow main)
        {
            _main = main;
            InitializeComponent();
        }
    }
}
