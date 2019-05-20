using System;
using System.Collections.Generic;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ManipulationSystemLibrary;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ManipApp
{
    using System.Collections.Generic;

    using OxyPlot;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenManipulatorFile_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true) return;
            var jsonStringMatrixMathModel = File.ReadAllText(openFileDialog.FileName);
            var manipConfig = JsonConvert.DeserializeObject<MatrixMathModel>(jsonStringMatrixMathModel);
            var model = new MatrixMathModel(manipConfig);
        }
        
        private void SplitPathByPointsQty_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SplitPathWithStep_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PathPlanningButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// On mouse click, select the specific board 
        /// where the click happened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        //public void OnViewportMouseDown(object sender, MouseEventArgs args)
        //{
        //    //if (vstuff.models == null)
        //    //{
        //    //    return;
        //    //}

        //    if (
        //        Keyboard.IsKeyDown(Key.LeftCtrl)
        //        || Keyboard.IsKeyDown(Key.RightCtrl)
        //    )
        //    {
        //        // extending the selection.  
        //        // don't unselect all first.
        //    }
        //    else
        //    {
        //        //UnselectAll();
        //    }

        //    RayMeshGeometry3DHitTestResult rayMeshResult =
        //        (RayMeshGeometry3DHitTestResult)
        //        VisualTreeHelper.HitTest(myVP, args.GetPosition(myVP));

        //    if (rayMeshResult != null)
        //    {
        //        PartialModel found = null;
        //        foreach (PartialModel pm in vstuff.models)
        //        {
        //            if (pm.mesh == rayMeshResult.MeshHit)
        //            {
        //                found = pm;
        //                break;
        //            }
        //        }

        //        if (found != null)
        //        {
        //            if (IsSelected(found.bag.solid))
        //            {
        //                Unselect(found.bag.solid);
        //            }
        //            else
        //            {
        //                Select(found.bag.solid);
        //            }
        //        }
        //      }
        //this.Viewport3D.Camera = new PerspectiveCamera()
        //}
    }
}
