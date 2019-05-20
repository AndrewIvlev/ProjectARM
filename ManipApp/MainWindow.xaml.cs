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
        /// <summary>
        /// 0 - camera rotation;
        /// 1 - path creation;
        /// 2 - path editing;
        /// </summary>
        private byte MouseMod;
        private Point MousePos;

        public MainWindow()
        {
            InitializeComponent();
            MouseMod = 0;
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
        /// По клику ЛКМ по сцене мы либо перемещаем камеру,
        /// либо создаём траекторию пути, либо редактируем траекторию пути.
        /// </summary>
        private void Canvas_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            switch (MouseMod)
            {
                case 0:
                    break;
                case 1:
                    //path.AddAnchorPoint(e.GetPosition());
                    //if (path.GetLen() != 0) label3.Text = $"Trajectory lenght = {(CoeftoRealW() * path.GetLen()).ToString("#.0000000000")}cm";
                    //comboBox1.Items.Clear();
                    //path.Show(PicBoxGraphics);
                    break;
                case 2:
                    //if (MousePressed == 0)
                    //{
                    //    index = path.NearestPointIndex(e.Location);
                    //    if (Math.Pow((e.Location.X - path.AnchorPoints[index].X), 2) +
                    //        Math.Pow((e.Location.Y + path.AnchorPoints[index].Y), 2) <= 40)
                    //    {
                    //        path.AnchorPoints[index] = e.Location;
                    //    }
                    //    MousePressed = 1;
                    //    return;
                    //}
                    //if (MousePressed == 1)
                    //{
                    //    MousePressed = 0;
                    //    return;
                    //}
                    break;
                case 3:
                    //following the cursor
                    break;
                default:
                    break;
            }
            this.Viewport3D.Camera = new PerspectiveCamera() { };
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                txt.Text = $"tmp = {tmp} holding...";
                tmp++;
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                txt.Text = $"tmp = {tmp} released!";
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Camera zoom
        }
    }
}
