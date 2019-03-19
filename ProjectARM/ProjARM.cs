using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace ProjectARM
{
    public partial class ProjARM : Form
    {
        #region ENVIRONMENT
        // MatrixMathModel if true
        // ExplicitMathModel if false
        bool mathModelType = false;
        
        #endregion

        #region COMPUTATION

        List<Dpoint> DeltaPoints;
        MathModel modelMan;
        byte NumOfUnits;
        Trajectory Way;

        #endregion

        #region GRAPHICS

        Manipulator Man;
        Point OffSet;
        Graphics PicBoxGraphics;
        byte MousePressed;
        //TODO: if it possible remove Flag and index
        bool IsUnitsDataGridCellChanged;
        byte Flag;
        int index;

        #endregion

        public ProjARM()
        {
            InitializeComponent();

            DeltaPoints = new List<Dpoint>();
            modelMan = new MatrixMathModel(NumOfUnits);
            PicBoxGraphics = pbCanvas.CreateGraphics();
            IsUnitsDataGridCellChanged = false;
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
                UnitsDataGridViewPreparation(currNumOfUnits);
                if (mathModelType)
                    modelMan = new MatrixMathModel(NumOfUnits);
                else
                    modelMan = new ExplicitMathModel(NumOfUnits);
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Save manipulator '' ?"); Сделать диалог ДА,НЕТ,ОТМЕНА
            unitsDataGridView.Rows.Clear();
            unitsDataGridView.Refresh();
        }

        private void CreateManipulator_Click(object sender, EventArgs e)
        {
            if (IsUnitsDataGridCellChanged)
            {
                byte currNumOfUnits = 0;
                if (!byte.TryParse(NumOfUnitsTextBox.Text, out currNumOfUnits))
                    throw new Exception("Not correct input of the Number of Units");
                if (NumOfUnits != currNumOfUnits)
                {
                    UnitsDataGridViewPreparation(NumOfUnits);
                }
                for (int i = 0; i < NumOfUnits; i++)
                {
                    MathModel.type[i] = Convert.ToChar(unitsDataGridView.Rows[i].Cells[1].Value.ToString());
                    MathModel.len[i] = Convert.ToDouble(unitsDataGridView.Rows[i + 1].Cells[2].Value.ToString());
                    MathModel.angle[i] = -MathModel.DegreeToRadian(Convert.ToDouble(unitsDataGridView.Rows[i + 1].Cells[3].Value.ToString()));
                }
            }
            ManipulatorConfigShow();
            ShowManipulator();
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
                unitsDataGridView.Rows[i].Cells[1].Value = MathModel.type[i];
                unitsDataGridView.Rows[i].Cells[2].Value = MathModel.len[i];
                unitsDataGridView.Rows[i].Cells[3].Value = MathModel.angle[i];
            }
        }

        #endregion

        #region Center Layout

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
                        //Находим ближайшую опорную точку
                        index = Way.NearestPointIndex(e.Location);
                        /*Нажал ли пользователь на опорную точку, если да, то её координаты
                        должны удовлетворять уравнению  (x - x')^2 + (y - y')^2 <= R^2
                        В нашей графической реализации радиус равен 6, но для удобства возьмём окрестность большего радиуса
                        */
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

        }

        private void stopMotion_Click(object sender, EventArgs e)
        {

        }

        private void restartMotion_Click(object sender, EventArgs e)
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

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DeltaPoints = MathEngine.MovingAlongTheTrajectory(Way, modelMan, backgroundWorker1);
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            computetionProgressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled) ;
            computetionProgressBar.Value = 0;
            chart1.Series[0].Points.Clear();
            foreach (Dpoint p in DeltaPoints)
                chart1.Series[0].Points.AddXY(p.x, (int)(CoeftoRealW() * p.y));
            label2.Text = $"Generalized Coordinates Q=({(int)MathModel.RadianToDegree(Man.Q[0])}, {(int)MathModel.RadianToDegree(Man.Q[1])}," +
                          $" {(int)Man.Q[2]}, {(int)MathModel.RadianToDegree(Man.Q[3])})";
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
                    MathModel.type[CurrUnitLine] = Convert.ToChar(filelines[i].Trim());
                    MathModel.len[CurrUnitLine] = Convert.ToDouble(filelines[++i].Trim());
                    MathModel.angle[CurrUnitLine] = -MathModel.DegreeToRadian(Convert.ToDouble(filelines[++i].Trim()));
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

        private void moveManipulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Man == null) MessageBox.Show("Firstly Create Manipulator");
            if (Way == null) MessageBox.Show("Firstly Create a Trajectory");
            //if (S.IsSplit()) MessageBox.Show("Please split trajectory firstly");
            Way.TransferFunction(OffSet, CoeftoRealW());
            backgroundWorker1.RunWorkerAsync();
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

        private void followForToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Flag = 3;
        }

        public void PictureBoxShow(bool AsSolid)
        {
            Pen p = new Pen(Color.Black, 7);
            Graphics gr = pbCanvas.CreateGraphics();
            if (AsSolid)
                gr.FillRectangle(new SolidBrush(Color.LightBlue), 0, 0, pbCanvas.Width, pbCanvas.Height);

            gr.DrawLine(p, new Point(0, 0), new Point(pbCanvas.Width, 0));
            gr.DrawLine(p, new Point(0, pbCanvas.Height), new Point(pbCanvas.Width, pbCanvas.Height));
            gr.DrawLine(p, new Point(0, 0), new Point(0, pbCanvas.Height));
            gr.DrawLine(p, new Point(pbCanvas.Width, 0), new Point(pbCanvas.Width, pbCanvas.Height));
        }

        private void MathModelConfig()
        {
            double[] a = new double[NumOfUnits];
            for (int i = 0; i < NumOfUnits - 2; i++)
            {
                a[i] = Math.Pow(1f / 4, 2) / Math.Pow(Math.PI, 2);
            }
            a[2] = Math.Pow(1f / 4, 2) / 25;

            MathModel.SetA(a);
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
                char type = MathModel.type[i];
                double len = CoefToGraphic() * MathModel.len[i];
                double angle = MathModel.angle[i];

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
                    case 'G':
                        tmpJstart.type = tmpJend.type = 'G';
                        tmpJend.TransferFunction(len, anglemnpltr);
                        Man.AddUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
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

        private void mainTableLayoutPanel_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            paindBorder(sender, e);
        }

        private void paindBorder(object sender, TableLayoutCellPaintEventArgs e)
        {
            var panel = sender as TableLayoutPanel;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            var rectangle = e.CellBounds;
            using (var pen = new Pen(Color.Black, 1))
            {
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                if (e.Row == (panel.RowCount - 1))
                {
                    rectangle.Height -= 1;
                }

                if (e.Column == (panel.ColumnCount - 1))
                {
                    rectangle.Width -= 1;
                }

                e.Graphics.DrawRectangle(pen, rectangle);
            }
        }

        private void units_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Событие начала редактирования ячейки (не нашёл событие "Ячейка изменена")
            IsUnitsDataGridCellChanged = true;
        }
    }
}
