using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Threading;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ProjectARM
{
    public partial class ProjARM : Form
    {
        #region COMPUTATION

        List<DPoint> DeltaPoints;
        MathModel modelMan;
        byte NumOfUnits;
        Trajectory Way;

        #endregion

        #region GRAPHICS

        Manipulator Man;
        Point OffSet;
        PlotModel myModel;
        LineSeries lineSeries;
        Graphics PicBoxGraphics;
        double[][] q; // Вектора обобщённых координат
        double SpeedMotion;
        byte MousePressed;
        //TODO: if it possible remove Flag and index
        bool IsUnitsDataGridCellChanged;
        bool DoesItStop;
        bool IsItRestarted;
        bool mathModelType = false; // MatrixMathModel if true, ExplicitMathModel if false
        byte Flag;
        int index;

        #endregion

        public ProjARM()
        {
            InitializeComponent();

            DeltaPoints = new List<DPoint>();
            q = new double[1024][];
            myModel = new PlotModel { Title = "Δ = ||P(i+1)-P'(i+1)||" };
            myModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "iteration" });
            myModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Δ, cm" });
            lineSeries = new LineSeries { Color = OxyColors.Blue };
            PicBoxGraphics = pbCanvas.CreateGraphics();
            IsUnitsDataGridCellChanged = false;
            DoesItStop = false;
            IsItRestarted = false;
            SpeedMotion = 1;
            MousePressed = 0;
            NumOfUnits = 0;
            Flag = 0;
        }

        #region Left Layout

        private void NumOfUnitsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            byte currNumOfUnits = 0;
            if (byte.TryParse(NumOfUnitsTextBox.Text, out currNumOfUnits))
                throw new Exception("Not correct input of the Number of Units");

            if (NumOfUnits != currNumOfUnits)
            {
                UnitsDataGridViewPreparation(currNumOfUnits);
                if (mathModelType)
                    modelMan = new MatrixMathModel(NumOfUnits);
                else
                    modelMan = new ExplicitMathModel(NumOfUnits);
            }
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            byte currNumOfUnits = 0;
            if (!byte.TryParse(NumOfUnitsTextBox.Text, out currNumOfUnits))
                throw new Exception("Not correct input of the Number of Units");

            if (NumOfUnits != currNumOfUnits)
            {
                NumOfUnits = currNumOfUnits;
                UnitsDataGridViewPreparation(NumOfUnits);
                if (mathModelType)
                    modelMan = new MatrixMathModel(NumOfUnits);
                else
                    modelMan = new ExplicitMathModel(NumOfUnits);
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Save manipulator '' ?"); Сделать диалог ДА, НЕТ, ОТМЕНА
            unitsDataGridView.Rows.Clear();
            unitsDataGridView.Refresh();
        }

        private void CreateManipulator_Click(object sender, EventArgs e)
        {
            if (IsUnitsDataGridCellChanged)
            {
                for (int i = 0; i < NumOfUnits; i++)
                {
                    modelMan.units[i].type = Convert.ToChar(unitsDataGridView.Rows[i].Cells[1].Value.ToString());
                    modelMan.units[i].len = Convert.ToDouble(unitsDataGridView.Rows[i].Cells[2].Value.ToString());
                    modelMan.units[i].angle = - MathModel.DegreeToRadian(Convert.ToDouble(unitsDataGridView.Rows[i].Cells[3].Value.ToString()));

                    if (modelMan.units[i].type == 'R' || modelMan.units[i].type == 'C')
                        modelMan.q[i - 1] = modelMan.units[i].angle;
                    else if (modelMan.units[i].type == 'P')
                            modelMan.q[i - 1] = modelMan.units[i].len;
                }
            }
            ManipulatorConfigShow();
            //ShowManipulator();
        }

        private void UnitsDataGridViewPreparation(int NumOfUnits)
        {
            unitsDataGridView.Visible = true;
            CancelBtn.Visible = true;
            CreateManipulator.Visible = true;
            unitsDataGridView.ColumnCount = 4;
            unitsDataGridView.RowCount = NumOfUnits;
            unitsDataGridView.Height = 25 + NumOfUnits * 25;
            CancelBtn.Location = new Point(CancelBtn.Location.X, unitsDataGridView.Location.Y + unitsDataGridView.Height + 15);
            CreateManipulator.Location = new Point(CreateManipulator.Location.X, unitsDataGridView.Location.Y + unitsDataGridView.Height + 15);
            unitsDataGridView.Columns[0].Name = "Num";
            unitsDataGridView.Columns["Num"].Width = 40;
            unitsDataGridView.Columns["Num"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            unitsDataGridView.Columns[1].Name = "Type";
            unitsDataGridView.Columns["Type"].Width = 48;
            unitsDataGridView.Columns["Type"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            unitsDataGridView.Columns[2].Name = "Lenght(cm)";
            unitsDataGridView.Columns["Lenght(cm)"].Width = 77;
            unitsDataGridView.Columns["Lenght(cm)"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            unitsDataGridView.Columns[3].Name = "Angle";
            unitsDataGridView.Columns["Angle"].Width = 67;
            unitsDataGridView.Columns["Angle"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            for (int i = 0; i < NumOfUnits; i++)
                unitsDataGridView.Rows[i].Cells[0].Value = i;
        }

        private void ManipulatorConfigShow()
        {
            for (int i = 0; i < NumOfUnits; i++)
            {
                unitsDataGridView.Rows[i].Cells[1].Value = modelMan.units[i].type;
                unitsDataGridView.Rows[i].Cells[2].Value = modelMan.units[i].len;
                unitsDataGridView.Rows[i].Cells[3].Value = modelMan.units[i].angle;
            }
        }

        #endregion

        #region Center Layout
        
        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            var rectangle = e.ClipRectangle;

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.FillRectangle(new SolidBrush(Color.LightBlue), rectangle);
        }

        private void pbCanvas_MouseEnter(object sender, EventArgs e)
        {
            pbCanvas.Cursor = Cursors.Hand;
        }

        private void pbCanvas_MouseLeave(object sender, EventArgs e)
        {
            pbCanvas.Cursor = Cursors.Default;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (Flag)
            {
                case 1:
                    Way.AddAnchorPoint(e.Location);
                    if (Way.GetLen() != 0) label3.Text = $"Trajectory lenght = {(CoeftoRealW() * Way.GetLen()).ToString("#.0000000000")}cm";
                    comboBox1.Items.Clear();
                    Way.Show(PicBoxGraphics);
                    break;
                case 2:
                    if (MousePressed == 0)
                    {
                        index = Way.NearestPointIndex(e.Location);
                        if (Math.Pow((e.Location.X - Way.AnchorPoints[index].X), 2) +
                            Math.Pow((e.Location.Y + Way.AnchorPoints[index].Y), 2) <= 40)
                        {
                            Way.AnchorPoints[index] = e.Location;
                        }
                        MousePressed = 1;
                        return;
                    }
                    if (MousePressed == 1)
                    {
                        MousePressed = 0;
                        return;
                    }
                    break;
                case 3:
                    //following the cursor
                    break;
                default:
                    break;
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (MousePressed == 1)
            {
                Way.Hide(PicBoxGraphics);
                Way.AnchorPoints[index] = e.Location;
                if (Way.GetLen() != 0) label3.Text = $"Trajectory lenght = {(CoeftoRealW() * Way.GetLen()).ToString("#.00")}cm";
                comboBox1.Items.Clear();
                Way.Show(PicBoxGraphics);
            }
        }

        private void startMotion_Click(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
        }
        
        private void stopMotion_Click(object sender, EventArgs e)
        {
            DoesItStop = true;
        }

        private void restartMotion_Click(object sender, EventArgs e)
        {
            IsItRestarted = true;
        }

        private void comboBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            SpeedMotion = Convert.ToDouble(comboBox3.Text);
        }

        private void comboBox3_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            for ( int i = 0; i < q.Length; i++)
            {
                if (DoesItStop)
                    while (DoesItStop) ;

                if (IsItRestarted)
                    i = 0;

                Man.Move(PicBoxGraphics, q[i]);

                Thread.Sleep((int)(1000 / SpeedMotion));

                label2.Invoke((MethodInvoker)delegate {
                    label2.Text = $"Generalized Coordinates \nQ=({(int)MathModel.RadianToDegree(modelMan.q[0])}," +
                    $"{(int)MathModel.RadianToDegree(modelMan.q[1])}, {(int)modelMan.q[2]}, {(int)MathModel.RadianToDegree(modelMan.q[3])})";
                });

                backgroundWorker2.ReportProgress((int)((float)(i + 1) / q.Length * 100));
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            showMotionProgressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

        }

        #endregion

        #region Right Layout

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            int k = Convert.ToInt32(comboBox1.Text);
        //if (k < (int)S.GetLen() || (S.GetLen() / k == 0))
        //  MessageBox.Show("k should be longer than trajectory lenght or it is too big");
        //  MessageBox.Show("Число К должно быть больше длины пути или оно слишком большое");
        //else
        //{
        Way.ExactExtraPointsClear();
            Way.SplitTrajectory(k);
            Way.ShowExtraPoints(PicBoxGraphics);
            //}
        }

        private void comboBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            double step = CoefToGraphic() * Convert.ToDouble(comboBox2.Text);
            Way.ExactExtraPointsClear();
            Way.SplitTrajectory(step);
            Way.ShowExtraPoints(PicBoxGraphics);
        }

        private void comboBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Way == null) return;
            double len = Way.GetLen();
            comboBox1.Items.Add($"{(int)len}");
            comboBox1.Items.Add($"{(int)len / 2}");
            comboBox1.Items.Add($"{(int)len / 3}");
            comboBox1.Items.Add($"{(int)len / 4}");
            comboBox1.Items.Add($"{(int)len / 5}");
        }


        private void pathPlanningBtn_Click(object sender, EventArgs e)
        {
            if (Man == null) MessageBox.Show("Firstly Create Manipulator");
            else
            {
                if (Way == null) MessageBox.Show("Firstly Create a Trajectory");
                else
                {
                    if (Way.IsSplit)
                    {
                        Way.TransferFunction(OffSet, CoeftoRealW());
                        backgroundWorker1.RunWorkerAsync();
                    }
                    else
                        MessageBox.Show("Please split trajectory firstly");
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            q = MathEngine.MovingAlongTheTrajectory(Way, modelMan, DeltaPoints, backgroundWorker1);
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            computetionProgressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled) ;
            computetionProgressBar.Value = 0;
            lineSeries.Points.Clear();
            foreach (DPoint p in DeltaPoints)
                lineSeries.Points.Add(new DataPoint(p.X, (int)(CoeftoRealW() * p.Y)));

            myModel.Series.Clear();
            myModel.Series.Add(lineSeries);

            this.plotView.Model = myModel;
            this.plotView.Refresh();
            this.plotView.Show();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        #endregion

        #region Menu Strip

        #region Manipulator

        private void newMnpltrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Visible = true;
            NumOfUnitsTextBox.Visible = true;
            GoBtn.Visible = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string manConfigDir = @"\ManipConfig";
            var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var varFileFullName = currentDirectory + manConfigDir;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.Title = "Open manipulator configuration file";
            dialog.InitialDirectory = varFileFullName;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filename = dialog.FileName;
                string[] filelines = File.ReadAllLines(filename);

                NumOfUnitsTextBox.Text = filelines[0];

                NumOfUnits = Convert.ToByte(filelines[0].Trim());

                UnitsDataGridViewPreparation(NumOfUnits);

                if (mathModelType)
                    modelMan = new MatrixMathModel(NumOfUnits);
                else
                    modelMan = new ExplicitMathModel(NumOfUnits);

                int CurrUnitLine = 0;
                for (int i = 1; i < filelines.Length; i++)
                {
                    modelMan.units[CurrUnitLine].type = Convert.ToChar(filelines[i].Trim());
                    modelMan.units[CurrUnitLine].len = Convert.ToDouble(filelines[++i].Trim());
                    modelMan.units[CurrUnitLine].angle = -MathModel.DegreeToRadian(Convert.ToDouble(filelines[++i].Trim()));
                    CurrUnitLine++;
                }
                UnitsDataGridViewPreparation(NumOfUnits);
                ManipulatorConfigShow();
                IsUnitsDataGridCellChanged = false;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void destroyManipulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Man.Hide(PicBoxGraphics);
            //mnpltr.Dispose();
        }

        #endregion

        #region Trajectory
        
        private void newTrajectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Man == null)
                MessageBox.Show("Firstly Create Manipulator");
            else
            {
                Way = new Trajectory();
                Way.AddAnchorPoint(Man.mnp[NumOfUnits - 1].end.dot);
                Flag = 1;
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void sToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void editTrajectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Flag = 2;
        }

        private void interpolateTrajectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteTrajectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Way.Hide(PicBoxGraphics);
            Way.Clear();
            Flag = 7;
        }

        #endregion

        #region Obstacle
        
        private void createObstacleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void editObstacleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void destrouObstacleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Following the Cursor

        #endregion

        #endregion

        /// <summary>
        /// Перевод значений длин из сантиметров в пиксели picturebox
        /// </summary>
        /// <returns>Возвращает коэффициент перевода из реального мира в графический</returns>
        internal double CoefToGraphic()
        {
            double percent = 0.9;
            if (modelMan != null)
                return pbCanvas.Width * percent / (2 * modelMan.MaxL(new double[1] { 0 }));
            return 0;
        }

        /// <summary>
        /// Перевод значений длин из пикселей picturebox в сантиметры
        /// </summary>
        /// <returns>Возвращает коэффициент перевода из графического мира в реальный</returns>
        public double CoeftoRealW() => 1f / CoefToGraphic();

        private void followForToolStripMenuItem_Click(object sender, EventArgs e) => Flag = 3;

        private void MathModelConfig()
        {
            double[] a = new double[NumOfUnits];
            for (int i = 0; i < NumOfUnits - 2; i++)
            {
                a[i] = Math.Pow(1f / 4, 2) / Math.Pow(Math.PI, 2);
            }
            a[2] = Math.Pow(1f / 4, 2) / 25;

            modelMan.SetA(a);
        }

        private void ShowManipulator()
        {
            OffSet = new Point(pbCanvas.Width / 2, pbCanvas.Height - 10);
            Man = new Manipulator(NumOfUnits);
            Joint tmpJstart = new Joint('S', OffSet);
            Joint tmpJend = new Joint('S', OffSet);
            double anglemnpltr = 0;

            for (int i = 0; i < NumOfUnits; i++)
            {
                char type = modelMan.units[i].type;
                double len = CoefToGraphic() * modelMan.units[i].len;
                double angle = modelMan.units[i].angle;

                switch (type)
                {
                    case 'S':
                        tmpJend.TransferFunction(len, angle);
                        Man.AddUnit(new Unit(tmpJstart, tmpJend, len, angle));
                        tmpJstart.TransferFunction(len, angle);
                        break;
                    case 'R':
                        tmpJstart.type = 'R';
                        tmpJend.type = 'S';
                        anglemnpltr += angle;
                        tmpJend.TransferFunction(len, anglemnpltr);
                        Man.AddUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        tmpJstart.TransferFunction(len, anglemnpltr);
                        break;
                    case 'P':
                        tmpJstart.type = 'P';
                        tmpJend.type = 'S';
                        tmpJend.TransferFunction(len, anglemnpltr);
                        Man.AddUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        tmpJstart.TransferFunction(len, anglemnpltr);
                        break;
                    default:
                        break;
                }
            }
            Man.Show(PicBoxGraphics); // Отображение манипулятора в начальном положении
        }

        private void ProjARM_Load(object sender, EventArgs e)
        {

        }

        private void units_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Событие начала редактирования ячейки (не нашёл событие "Ячейка изменена")
            IsUnitsDataGridCellChanged = true;
        }
    }
}
