using System;
using System.Drawing;
using System.Windows.Forms;

namespace projarm
{
    public partial class Form1 : Form
    {
        MathModel ModelMnpltr;
        Manipulator mnpltr;
        public Graphics gr;
        byte MousePressed;
        byte numOfUnits;
        int index;
        byte flag;
        Path S;

        public Form1()
        {
            InitializeComponent();
            gr = CreateGraphics();
            MousePressed = 0;
            numOfUnits = 0;
            flag = 0;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics actionArea = e.Graphics;
            Pen p = new Pen(Color.Black, 6);
            actionArea.DrawLine(p, new Point(282, 38), new Point(this.Width - 27, 38));
            actionArea.DrawLine(p, new Point(282, this.Height - 50), new Point(this.Width - 27, this.Height - 50));
            actionArea.DrawLine(p, new Point(285, 38), new Point(285, this.Height - 50));
            actionArea.DrawLine(p, new Point(this.Width - 30, 38), new Point(this.Width - 30, this.Height - 50));
            actionArea.FillRectangle(new SolidBrush(System.Drawing.Color.LightBlue), 288, 41, this.Width - 321, this.Height - 94);
            actionArea.Dispose();
        }

        private void NumofUnits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                numOfUnits = Convert.ToByte(NumofUnits.Text);
                unitsDataGridView.ColumnCount = 4;
                unitsDataGridView.RowCount = numOfUnits;
                unitsDataGridView.RowHeadersVisible = false;
                
                unitsDataGridView.Columns[0].Name = "Number";
                unitsDataGridView.Columns[1].Name = "Type";
                unitsDataGridView.Columns[2].Name = "Lenght";
                unitsDataGridView.Columns[3].Name = "Angle";
                for( int i = 0; i < numOfUnits; i++)
                    unitsDataGridView.Rows[i].Cells[0].Value = i;
            }
        }

        private void unitsDataGridView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.Width = 5;
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
            unitsDataGridView.Rows[1].Cells[2].Value = 55;
            unitsDataGridView.Rows[1].Cells[3].Value = 0;

            unitsDataGridView.Rows[2].Cells[1].Value = "R";
            unitsDataGridView.Rows[2].Cells[2].Value = 50;
            unitsDataGridView.Rows[2].Cells[3].Value = 0;

            unitsDataGridView.Rows[3].Cells[1].Value = "P";
            unitsDataGridView.Rows[3].Cells[2].Value = 75;
            unitsDataGridView.Rows[3].Cells[3].Value = 0;

            unitsDataGridView.Rows[4].Cells[1].Value = "R";
            unitsDataGridView.Rows[4].Cells[2].Value = 25;
            unitsDataGridView.Rows[4].Cells[3].Value = 0;

            unitsDataGridView.Rows[5].Cells[1].Value = "G";
            unitsDataGridView.Rows[5].Cells[2].Value = 0;
            unitsDataGridView.Rows[5].Cells[3].Value = 0;
        }

        private void ctreateMnpltrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mnpltr = new Manipulator(numOfUnits);
            Point OffSet = new Point(288 + (this.Width - 321) / 2, this.Height - 58);
            Joint tmpJstart = new Joint('S', OffSet);
            Joint tmpJend = new Joint('S', OffSet);
            double anglemnpltr = 0;

            for (int i = 0; i < numOfUnits; i++)
            {
                char type = Convert.ToChar(unitsDataGridView.Rows[i].Cells[1].Value.ToString());
                double len = Convert.ToDouble(unitsDataGridView.Rows[i].Cells[2].Value.ToString());
                double angle = Convert.ToDouble(unitsDataGridView.Rows[i].Cells[3].Value.ToString());

                switch (type)
                {
                    case 'S':
                        tmpJend.TransferFunction(len, angle);
                        mnpltr.addUnit(new Unit(tmpJstart, tmpJend, len, angle));
                        tmpJstart.TransferFunction(len, angle);
                        break;
                    case 'R':
                        tmpJstart.type = 'R';
                        tmpJend.type = 'S';
                        anglemnpltr += angle;
                        tmpJend.TransferFunction(len, anglemnpltr);
                        mnpltr.addUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        tmpJstart.TransferFunction(len, anglemnpltr);
                        break;
                    case 'P':
                        tmpJstart.type = 'P';
                        tmpJend.type = 'S';
                        tmpJend.TransferFunction(len, anglemnpltr);
                        mnpltr.addUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        tmpJstart.TransferFunction(len, anglemnpltr);
                        break;
                    case 'G':
                        tmpJstart.type = tmpJend.type = 'G';
                        tmpJend.TransferFunction(len, anglemnpltr);
                        mnpltr.addUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        break;
                    default:
                        break;
                }
            }
            mnpltr.Show(gr); // Отображение манипулятора в начальном положении
        }
        private void moveManipulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelMnpltr = new MathModel(numOfUnits - 2);
            for (int i = 0; i < numOfUnits - 2; i++)
            {
                MathModel.len[i] = mnpltr.mnp[i + 1].lenght;
                MathModel.angle[i] = mnpltr.mnp[i + 1].angle;
            }

            S.ExactExtraPointOffset(new Point(288 + (this.Width - 321) / 2, this.Height - 58));

            backgroundWorker1.RunWorkerAsync();
        }

        private void destroyManipulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mnpltr.Hide(gr);
            //mnpltr.Dispose();
        }

        private void createPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S = new Path();
            S.AddAnchorPoint(mnpltr.mnp[numOfUnits - 1].end.dot);
            flag = 1;
        }

        private void editPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flag = 2;
        }

        private void deletePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S.Hide(gr);
            S.ClearAllList(); //Впоследствии заменить
            flag = 0;
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            switch(flag)
            {
                case 1:
                    S.AddAnchorPoint(e.Location);
                    if (S.GetLen() != 0) label3.Text = $"Path lenght = {S.GetLen().ToString("#.0000000000")}";
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
                        if (Math.Pow((e.Location.X - S.AnchorPoint[index].X), 2) +
                            Math.Pow((e.Location.Y + S.AnchorPoint[index].Y), 2) <= 40) 
                        {
                            S.AnchorPoint[index] = e.Location;
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

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MousePressed == 1)
            {
                S.Hide(gr);
                S.AnchorPoint[index] = e.Location;
                if (S.GetLen() != 0) label3.Text = $"Path lenght = {S.GetLen().ToString("#.0000000000")}";
                comboBox1.Items.Clear();
                S.Show(gr);
            }
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            Graphics actionArea = CreateGraphics();
            Pen p = new Pen(Color.White, 6);
            actionArea.DrawLine(p, new Point(282, 38), new Point(this.Width - 27, 38));
            actionArea.DrawLine(p, new Point(282, this.Height - 50), new Point(this.Width - 27, this.Height - 50));
            actionArea.DrawLine(p, new Point(285, 38), new Point(285, this.Height - 50));
            actionArea.DrawLine(p, new Point(this.Width - 30, 38), new Point(this.Width - 30, this.Height - 50));
            actionArea.FillRectangle(new SolidBrush(System.Drawing.Color.White), 288, 41, this.Width - 321, this.Height - 94);
            actionArea.Dispose();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            Graphics actionArea = CreateGraphics();
            Pen p = new Pen(Color.Black, 6);
            actionArea.DrawLine(p, new Point(282, 38), new Point(this.Width - 27, 38));
            actionArea.DrawLine(p, new Point(282, this.Height - 50), new Point(this.Width - 27, this.Height - 50));
            actionArea.DrawLine(p, new Point(285, 38), new Point(285, this.Height - 50));
            actionArea.DrawLine(p, new Point(this.Width - 30, 38), new Point(this.Width - 30, this.Height - 50));
            actionArea.FillRectangle(new SolidBrush(System.Drawing.Color.LightBlue), 288, 41, this.Width - 321, this.Height - 94);
            actionArea.Dispose();
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Int32 k = Convert.ToInt32(comboBox1.Text);
                double PathLen = S.GetLen();
                if (k < (int)PathLen || (PathLen / k == 0))
                    MessageBox.Show("k should be longer than path lenght or it is too big");
                //  MessageBox.Show("Число К должно быть больше длины пути или оно слишком большое");
                /* Чтобы шаг Step был в промежутке от 0 до 1,
                 * нужно чтобы отношение длины пути к количеству
                 * точек было больше 0 но меньше 1
                */
                else
                {
                    flag = 0;
                    S.ExtraPoint.Clear();
                    S.ExactExtraPoint.Clear();
                    S.SplitPath(k);
                    backgroundWorker2.RunWorkerAsync();
                }
            }
        }

        private void comboBox1_MouseDown(object sender, MouseEventArgs e)
        {
            comboBox1.Items.Add($"{(int)S.GetLen() + 1}");
            comboBox1.Items.Add($"{(int)S.GetLen() + 13}");
            comboBox1.Items.Add($"{(int)S.GetLen() * 2}");
            comboBox1.Items.Add($"{(int)S.GetLen() * 5}");
            comboBox1.Items.Add($"{(int)S.GetLen() * 10}");
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            MathEngine.MovingAlongThePath(gr, S, ModelMnpltr, mnpltr, backgroundWorker1);
            //label2.Text = $"Generalized Coordinates Q = ({mnpltr.Q[0]}, {mnpltr.Q[1]}, {mnpltr.Q[2]}, {mnpltr.Q[3]})";
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
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            backgroundWorker2.CancelAsync();
        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            S.ShowExtraPoints(gr, backgroundWorker2);
        }

        private void backgroundWorker2_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                ;//////////////
            }
            progressBar1.Value = 0;
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
