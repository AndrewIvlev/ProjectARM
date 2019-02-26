using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ProjectARM
{
    public partial class ProjARM : Form
    {
        List<Dpoint> DeltaPoints;
        MatrixMathModel MatrixModelMan;
        Manipulator Man;
        byte NumOfUnits;
        Path Way;
        Point OffSet;
        Graphics PicBoxGraphics;
        byte Flag;
        byte MousePressed;
        int index;
        
        public ProjARM()
        {
            InitializeComponent();
            DeltaPoints = new List<Dpoint>();
            PicBoxGraphics = pictureBox.CreateGraphics();
            PictureBoxShow(true);
            MousePressed = 0;
            NumOfUnits = 0;
            Flag = 0;
        }

        /// <summary>
        /// Перевод значений длин из сантиметров в пиксели picturebox
        /// </summary>
        /// <returns>Возвращает коэффициент перевода из реального мира в графический</returns>
        internal double CoefToGraphic()
        {
            double percent = 0.9;
            if (MatrixModelMan != null)
                return pictureBox.Width * percent / (2 * MatrixModelMan.MaxL(new double[1] { 0 }));
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

        #region PictureBox

        public void PictureBoxShow(bool AsSolid)
        {
            Pen p = new Pen(Color.Black, 7);
            Graphics gr = pictureBox.CreateGraphics();
            if (AsSolid)
                gr.FillRectangle(new SolidBrush(Color.LightBlue), 0, 0, pictureBox.Width, pictureBox.Height);
            gr.DrawLine(p, new Point(1, 1), new Point(pictureBox.Width - 2, 1));
            gr.DrawLine(p, new Point(1, pictureBox.Height - 2), new Point(pictureBox.Width - 2, pictureBox.Height - 2));
            gr.DrawLine(p, new Point(1, 1), new Point(1, pictureBox.Height - 2));
            gr.DrawLine(p, new Point(pictureBox.Width - 2, 1), new Point(pictureBox.Width - 2, pictureBox.Height - 2));
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (Flag)
            {
                case 1:
                    Way.AddAnchorPoint(e.Location);
                    if (Way.GetLen() != 0) label3.Text = $"Path lenght = {(CoeftoRealW() * Way.GetLen()).ToString("#.0000000000")}cm";
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
                if (Way.GetLen() != 0) label3.Text = $"Path lenght = {(CoeftoRealW() * Way.GetLen()).ToString("#.00")}cm";
                comboBox1.Items.Clear();
                Way.Show(PicBoxGraphics);
            }
        }

        #endregion

        #region Manipulator

        private void newMnpltrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Visible = true;
            NumOfUnitsTextBox.Visible = true;
            GoBtn.Visible = true;
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


        private void moveManipulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Man == null) MessageBox.Show("Firstly Create Manipulator");
            if (Way == null) MessageBox.Show("Firstly Create a Path");
            //if (S.IsSplit()) MessageBox.Show("Please split path firstly");
            Way.TransferFunction(OffSet, CoeftoRealW());
            backgroundWorker1.RunWorkerAsync();
        }

        private void destroyManipulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Man.Hide(PicBoxGraphics);
            //mnpltr.Dispose();
        }

        #endregion

        #region Manipulator Configuration File

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.Title = "Open manipulator configuration file";
            dialog.InitialDirectory = @"D:\repo\ProjectARM\ProjectARM\ManipConfig";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filename = dialog.FileName;
                string[] filelines = File.ReadAllLines(filename);
                NumOfUnits = Convert.ToByte(filelines[0].Trim());
                MatrixModelMan = new MatrixMathModel(NumOfUnits);
                int CurrUnitLine = 0;

                for (int i = 1; i < filelines.Length; i++)
                {
                    MatrixModelMan.type[CurrUnitLine] = Convert.ToChar(filelines[i].Trim());
                    MatrixModelMan.len[CurrUnitLine] = Convert.ToDouble(filelines[++i].Trim());
                    MatrixModelMan.angle[CurrUnitLine] = Convert.ToDouble(filelines[++i].Trim());
                    CurrUnitLine++;
                }
                ShowManipulator();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Path

        private void createPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Man == null)
                MessageBox.Show("Firstly Create Manipulator");
            else
            {
                Way = new Path();
                Way.AddAnchorPoint(Man.mnp[NumOfUnits - 1].end.dot);
                Flag = 1;
            }
        }

        private void editPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Flag = 2;
        }

        private void deletePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Way.Hide(PicBoxGraphics);
            Way.Clear();
            Flag = 7;
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            int k = Convert.ToInt32(comboBox1.Text);
            //if (k < (int)S.GetLen() || (S.GetLen() / k == 0))
            //  MessageBox.Show("k should be longer than path lenght or it is too big");
            //  MessageBox.Show("Число К должно быть больше длины пути или оно слишком большое");
            //else
            //{
            Way.ExactExtraPointsClear();
            Way.SplitPath(k);
            Way.ShowExtraPoints(PicBoxGraphics);
            //}
        }

        private void comboBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            double step = CoefToGraphic() * Convert.ToDouble(comboBox2.Text);
            Way.ExactExtraPointsClear();
            Way.SplitPath(step);
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

        private void interpolatePathToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Background Workers

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            MathEngine.MovingAlongThePath(Way, MatrixModelMan, Man, PicBoxGraphics, backgroundWorker1, ref DeltaPoints);
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                ;
            }
            progressBar1.Value = 0;
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

        private void ShowManipulator()
        {
            OffSet = new Point(pictureBox.Width / 2, pictureBox.Height - 10);
            Man = new Manipulator(NumOfUnits);
            Joint tmpJstart = new Joint('S', OffSet);
            Joint tmpJend = new Joint('S', OffSet);
            double anglemnpltr = 0;

            for (int i = 0; i < NumOfUnits; i++)
            {
                char type = MatrixModelMan.type[i];
                double len = CoefToGraphic() * MatrixModelMan.len[i];
                double angle = MatrixModelMan.angle[i];

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

        private void GoBtn_Click(object sender, EventArgs e) => DataGridViewLoad();

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Save manipulator '' ?"); Сделать диалог ДА,НЕТ,ОТМЕНА
            units.Rows.Clear();
            units.Refresh();
        }

        private void CreateManipulator_Click(object sender, EventArgs e)
        {
            MatrixModelMan = new MatrixMathModel(NumOfUnits);
            for (int i = 0; i < NumOfUnits; i++)
            {
                MatrixModelMan.type[i] = Convert.ToChar(units.Rows[i].Cells[1].Value.ToString());
                MatrixModelMan.len[i] = Convert.ToDouble(units.Rows[i + 1].Cells[2].Value.ToString());
                MatrixModelMan.angle[i] = - MathModel.DegreeToRadian(Convert.ToDouble(units.Rows[i + 1].Cells[3].Value.ToString()));
            }
            NumOfUnitsTextBox.Text = NumOfUnits.ToString();
            UnitsFilling(NumOfUnits);
            ManipulatorConfigShow(NumOfUnits);
            ShowManipulator();
        }

        private void NumOfUnitsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            DataGridViewLoad();
        }

        private void DataGridViewLoad()
        {
            if (byte.TryParse(NumOfUnitsTextBox.Text, out NumOfUnits))
            {
                UnitsFilling(NumOfUnits);
            }
            else
            {
                MessageBox.Show("Not correct input of the Number of Units");
                return;
            }
        }
        private void UnitsFilling(int NumOfUnits)
        {
            units.Visible = true;
            CancelBtn.Visible = true;
            CreateManipulator.Visible = true;
            units.ColumnCount = 4;
            units.RowCount = NumOfUnits;
            units.Height = 23 + NumOfUnits * 20;
            CancelBtn.Location = new Point(CancelBtn.Location.X, units.Location.Y + units.Height + 15);
            CreateManipulator.Location = new Point(CreateManipulator.Location.X, units.Location.Y + units.Height + 15);
            units.Columns[0].Name = "Num";
            units.Columns["Num"].Width = 40;
            units.Columns["Num"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            units.Columns[1].Name = "Type";
            units.Columns["Type"].Width = 48;
            units.Columns["Type"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            units.Columns[2].Name = "Lenght, cm";
            units.Columns["Lenght, cm"].Width = 85;
            units.Columns["Lenght, cm"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            units.Columns[3].Name = "Angle, °";
            units.Columns["Angle, °"].Width = 77;
            units.Columns["Angle, °"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            for (int i = 0; i < NumOfUnits; i++)
                units.Rows[i].Cells[0].Value = i;
        }

        private void ManipulatorConfigShow(int NumOfUnits)
        {
            for ( int i = 0; i < NumOfUnits; i++)
            {
                units.Rows[i].Cells[1].Value = MatrixModelMan.type[i];
                units.Rows[i].Cells[2].Value = MatrixModelMan.len[i];
                units.Rows[i].Cells[3].Value = MatrixModelMan.angle[i]; 
            }
        }
        
        private void ProjARM_Layout(object sender, LayoutEventArgs e)
        {
            pictureBox.Width = Width - 586;
            pictureBox.Height = Height - 85;
            Klabel.Location = new Point(Width - 293, Klabel.Location.Y);
            label2.Location = new Point(Width - 293, label2.Location.Y);
            label3.Location = new Point(Width - 293, label3.Location.Y);
            label4.Location = new Point(Width - 293, label4.Location.Y);
            comboBox1.Location = new Point(Width - 128, comboBox1.Location.Y);
            comboBox2.Location = new Point(Width - 109, comboBox2.Location.Y);
            chart1.Location = new Point(Width - 300, chart1.Location.Y);
            CancelMoveBtn.Location = new Point(Width - 109, CancelMoveBtn.Location.Y); 
            progressBar1.Location = new Point(Width - 289, progressBar1.Location.Y);
        }

        private void pictureBox_Layout(object sender, LayoutEventArgs e)
        {
            PictureBoxShow(true);
        }
    }
}
