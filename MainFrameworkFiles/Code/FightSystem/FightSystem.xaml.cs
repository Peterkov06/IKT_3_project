using InterfaceClass;
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
    /// Interaction logic for FightSystem.xaml
    /// </summary>
    public partial class FightSystem : UserControl
    {
        MainWindow _main;
        ICharacter?[] playerSide;
        ICharacter?[] enemySide;

        public FightSystem(MainWindow main, ICharacter?[] playerSide, ICharacter?[] enemySide)
        {
            _main = main;
            InitializeComponent();
            this.playerSide = playerSide;
            this.enemySide = enemySide;
            ReturnToStory();
            
        }
        public void ReturnToStory()
        {
            _main.SceneChanger(2, new BackToStory(playerSide[0] as Player, playerSide, 2));
        }
    }
}
