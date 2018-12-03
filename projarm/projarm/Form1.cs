using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;

namespace projarm
{
    public partial class Form1 : Form
    {
        public List<dpoint> DeltaPoints;
        MathModel ModelMnpltr;
        Manipulator mnpltr;
        public Graphics gr;
        byte MousePressed;
        byte numOfUnits;
        Series series;
        Point OffSet;
        int index;
        byte flag;
        Path S;

        public Form1()
        {
            InitializeComponent();
            series = new Series("Delta");
            DeltaPoints = new List<dpoint>();
            gr = pictureBox.CreateGraphics();
            MousePressed = 0;
            numOfUnits = 0;
            flag = 0;
        }

        private double CoeftoGraphic()//Коэф. перевода значений длин из сантиметров в пиксели picturebox
        {
            double percent = 0.9;
            if (ModelMnpltr != null)
               return pictureBox.Width * percent / (2 * ModelMnpltr.MaxL(new double[1]{50}));
            return 0;
        }
        private double CoeftoRealW()
        {
            return 1f / CoeftoGraphic();
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //В начале и в конце слетает с пути
            //Путь интерполируется полиномом Лагранжа
            //Реализовать матричный MathEngine
            //Написать функцию следования хвата манипулятора за курсором на pictureBox
            //Чтение конфигурации манипулятора из файла.txt
            //Обход препятсвий
            //3D model
            if (flag == 0)
            {
                Pen p = new Pen(Color.Black, 7);
                gr.FillRectangle(new SolidBrush(System.Drawing.Color.LightBlue), 0, 0, pictureBox.Width, pictureBox.Height);
                gr.DrawLine(p, new Point(1, 1), new Point(pictureBox.Width - 2, 1));
                gr.DrawLine(p, new Point(1, pictureBox.Height - 2), new Point(pictureBox.Width - 2, pictureBox.Height - 2));
                gr.DrawLine(p, new Point(1, 1), new Point(1, pictureBox.Height - 2));
                gr.DrawLine(p, new Point(pictureBox.Width - 2, 1), new Point(pictureBox.Width - 2, pictureBox.Height - 2));
            }
        }

        private void NumofUnits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            numOfUnits = Convert.ToByte(NumofUnits.Text);
            unitsDataGridView.ColumnCount = 4;
            unitsDataGridView.RowCount = numOfUnits;
            unitsDataGridView.RowHeadersVisible = false;
            unitsDataGridView.Columns[0].Name = "Num";
            unitsDataGridView.Columns[0].Width = 1;
            unitsDataGridView.Columns[1].Name = "Type";
            unitsDataGridView.Columns[1].Width = 1;
            unitsDataGridView.Columns[2].Name = "Lenght, cm";
            unitsDataGridView.Columns[3].Name = "Angle, °";
            for( int i = 0; i < numOfUnits; i++)
                unitsDataGridView.Rows[i].Cells[0].Value = i;
        }

        private void DefaultManipulator_Click(object sender, EventArgs e)
        {
            if (numOfUnits != 6)
            {
                numOfUnits = 6;
                unitsDataGridView.Rows.Clear();
                unitsDataGridView.Refresh();
                for (int i = 0; i < numOfUnits; i++)
                    unitsDataGridView.Rows[i].Cells[0].Value = i;
            }
            /*     Unit Type:    
             *      S - Static(Start Unit)
             *      R - Revolute Unit
             *      P - Prismatic Unit
             *      G - Gripper
            */
            unitsDataGridView.Rows[0].Cells[1].Value = "S"; //Unit Type
            unitsDataGridView.Rows[0].Cells[2].Value = 0;   //Unit Length
            unitsDataGridView.Rows[0].Cells[3].Value = 0;   //Unit Angle

            unitsDataGridView.Rows[1].Cells[1].Value = "R";
            unitsDataGridView.Rows[1].Cells[2].Value = 25;
            unitsDataGridView.Rows[1].Cells[3].Value = 0;

            unitsDataGridView.Rows[2].Cells[1].Value = "R";
            unitsDataGridView.Rows[2].Cells[2].Value = 25;
            unitsDataGridView.Rows[2].Cells[3].Value = 0;

            unitsDataGridView.Rows[3].Cells[1].Value = "P";
            unitsDataGridView.Rows[3].Cells[2].Value = 30;
            unitsDataGridView.Rows[3].Cells[3].Value = 0;

            unitsDataGridView.Rows[4].Cells[1].Value = "R";
            unitsDataGridView.Rows[4].Cells[2].Value = 20;
            unitsDataGridView.Rows[4].Cells[3].Value = 0;

            unitsDataGridView.Rows[5].Cells[1].Value = "G";
            unitsDataGridView.Rows[5].Cells[2].Value = 0;
            unitsDataGridView.Rows[5].Cells[3].Value = 0;
        }

        private void ctreateMnpltrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelMnpltr = new MathModel(numOfUnits - 2);
            for (int i = 0; i < numOfUnits - 2; i++)
            {
                //у MatrixMathModel здесь будет считываться тип звена
                MathModel.len[i] = Convert.ToDouble(unitsDataGridView.Rows[i + 1].Cells[2].Value.ToString());
                MathModel.angle[i] = -MathModel.DegreeToRadian(Convert.ToDouble(unitsDataGridView.Rows[i + 1].Cells[3].Value.ToString()));
            }
            ShowManipulator();
            flag = 7;
        }
        private void ShowManipulator()
        {
            mnpltr = new Manipulator(numOfUnits);
            OffSet = new Point(pictureBox.Width / 2, pictureBox.Height - 10);
            Joint tmpJstart = new Joint('S', OffSet);
            Joint tmpJend = new Joint('S', OffSet);
            double anglemnpltr = 0;

            for (int i = 0; i < numOfUnits; i++)
            {
                char type = Convert.ToChar(unitsDataGridView.Rows[i].Cells[1].Value.ToString());
                double len = CoeftoGraphic() * Convert.ToDouble(unitsDataGridView.Rows[i].Cells[2].Value.ToString());
                double angle = MathModel.DegreeToRadian(Convert.ToDouble(unitsDataGridView.Rows[i].Cells[3].Value.ToString()));

                switch (type)
                {
                    case 'S':
                        tmpJend.TransferFunction(len, angle);
                        mnpltr.AddUnit(new Unit(tmpJstart, tmpJend, len, angle));
                        tmpJstart.TransferFunction(len, angle);
                        break;
                    case 'R':
                        tmpJstart.type = 'R';
                        tmpJend.type = 'S';
                        anglemnpltr += angle;
                        tmpJend.TransferFunction(len, anglemnpltr);
                        mnpltr.AddUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        tmpJstart.TransferFunction(len, anglemnpltr);
                        break;
                    case 'P':
                        tmpJstart.type = 'P';
                        tmpJend.type = 'S';
                        tmpJend.TransferFunction(len, anglemnpltr);
                        mnpltr.AddUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        tmpJstart.TransferFunction(len, anglemnpltr);
                        break;
                    case 'G':
                        tmpJstart.type = tmpJend.type = 'G';
                        tmpJend.TransferFunction(len, anglemnpltr);
                        mnpltr.AddUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        break;
                    default:
                        break;
                }
            }
            mnpltr.Show(gr); // Отображение манипулятора в начальном положении
        }
        private void moveManipulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void destroyManipulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mnpltr.Hide(gr);
            //mnpltr.Dispose();
        }

        private void createPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mnpltr == null)
                MessageBox.Show("Firstly Create Manipulator");
            else
            {
                S = new Path();
                S.AddAnchorPoint(mnpltr.mnp[numOfUnits - 1].end.dot);
                flag = 1;
            }
        }

        private void editPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flag = 2;
        }

        private void deletePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S.Hide(gr);
            S.Clear();
            flag = 7;
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
                Array.Clear(S.ExtraPoints, 0, k);
                S.SplitPath(k);
                S.ShowExtraPoints(gr);
                S.TransferFunction(OffSet, k, CoeftoRealW());
            //}
        }

        private void comboBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            double step = CoeftoGraphic() * Convert.ToDouble(comboBox2.Text);
            S.ExtraClear();
            S.SplitPath(step);
            S.ShowExtraPoints(gr);
            S.TransferFunction(OffSet, CoeftoRealW());
        }

        private void comboBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (S == null) return;
            double len = S.GetLen();
            comboBox1.Items.Add($"{(int)len}");
            comboBox1.Items.Add($"{(int)len / 2}");
            comboBox1.Items.Add($"{(int)len / 3}");
            comboBox1.Items.Add($"{(int)len / 4}");
            comboBox1.Items.Add($"{(int)len / 5}");           
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            MathEngine.MovingAlongThePath(S, ModelMnpltr, mnpltr, gr, backgroundWorker1, ref DeltaPoints);
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                ;//////////////
            }
            progressBar1.Value = 0;
            
            foreach (dpoint p in DeltaPoints)
                chart1.Series[0].Points.AddXY(p.x, (int)(CoeftoRealW() * p.y));
            label2.Text = $"Generalized Coordinates Q=({(int)mnpltr.Q[0]}, {(int)mnpltr.Q[1]}, {(int)mnpltr.Q[2]}, {(int)mnpltr.Q[3]})";
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (flag)
            {
                case 1:
                    S.AddAnchorPoint(e.Location);
                    if (S.GetLen() != 0) label3.Text = $"Path lenght = {(CoeftoRealW() * S.GetLen()).ToString("#.0000000000")}cm";
                    comboBox1.Items.Clear();
                    S.Show(gr);
                    break;
                case 2:
                    if (MousePressed == 0)
                    {
                        //Находим ближайшую опорную точку
                        index = S.NearestPointIndex(e.Location);
                        /*Нажал ли пользователь на опорную точку, если да, то её координаты
                        должны удовлетворять уравнению  (x - x')^2 + (y - y')^2 <= R^2
                        В нашей графической реализации радиус равен 6, но для удобства возьмём окрестность большего радиуса
                        */
                        if (Math.Pow((e.Location.X - S.AnchorPoints[index].X), 2) +
                            Math.Pow((e.Location.Y + S.AnchorPoints[index].Y), 2) <= 40)
                        {
                            S.AnchorPoints[index] = e.Location;
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
                    break;
                default:
                    break;
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (MousePressed == 1)
            {
                S.Hide(gr);
                S.AnchorPoints[index] = e.Location;
                if (S.GetLen() != 0) label3.Text = $"Path lenght = {(CoeftoRealW() * S.GetLen()).ToString("#.0000000000")}cm";
                comboBox1.Items.Clear();
                S.Show(gr);
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            /*Pen p = new Pen(Color.Black, 7);
            gr.FillRectangle(new SolidBrush(System.Drawing.Color.LightBlue), 0, 0, pictureBox.Width, pictureBox.Height);
            gr.DrawLine(p, new Point(1, 1), new Point(pictureBox.Width - 2, 1));
            gr.DrawLine(p, new Point(1, pictureBox.Height - 2), new Point(pictureBox.Width - 2, pictureBox.Height - 2));
            gr.DrawLine(p, new Point(1, 1), new Point(1, pictureBox.Height - 2));
            gr.DrawLine(p, new Point(pictureBox.Width - 2, 1), new Point(pictureBox.Width - 2, pictureBox.Height - 2));
            if (mnpltr != null)
                mnpltr.Show(gr);
            if (S != null)
                S.Show(gr);*/
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
    //MessageBox.Show("Left tButton");
}
/*BlockMatrix[] Mmas = new BlockMatrix[2 * numOfUnits - 2];
Mmas[0] = new BlockMatrix();
for (int i = 1, j = 1; i < numOfUnits - 1; i++, j++)
{
    switch (unitsDataGridView.Rows[i].Cells[1].Value)
    {
        case 'R':
            Mmas[j] = new BlockMatrix('R', Convert.ToDouble(unitsDataGridView[i, 3].Value));
            Mmas[++j] = new BlockMatrix('P', Convert.ToDouble(unitsDataGridView[i, 2].Value));
            break;
        case 'P':
            Mmas[j] = new BlockMatrix('P', Convert.ToDouble(unitsDataGridView[i, 2].Value));
            Mmas[++j] = new BlockMatrix('P', Convert.ToDouble(unitsDataGridView[i, 2].Value));
            break;
        default:
            Mmas[j] = new BlockMatrix();
            break;
    }
}*/
