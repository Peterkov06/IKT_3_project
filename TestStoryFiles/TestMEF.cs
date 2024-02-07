using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel.Composition;
using static IKT_3_project.MainFrameworkFiles.Models.SharedInterfaces;

namespace IKT_3_project.TestStoryFiles
{
    [Export(typeof(IAdditionalSystem))]
    class TestMEF : IAdditionalSystem
    {
        public void Execute()
        {
            MessageBox.Show("Test MEF window.");
        }
    }
}
