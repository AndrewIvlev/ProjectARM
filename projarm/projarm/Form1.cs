using System;
using System.Drawing;
using System.Windows.Forms;

namespace projarm
{
    public partial class Form1 : Form
    {
        byte numOfUnits = 0;
        MathModel ModelMnpltr;
        Manipulator mnpltr;
        public Graphics gr;
        byte flag = 0;
        Path S;
        int k;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics actionArea = e.Graphics;
            gr = CreateGraphics();
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
                        mnpltr.Q[i - 1] = angle;
                        anglemnpltr += angle;
                        tmpJend.TransferFunction(len, anglemnpltr);
                        mnpltr.addUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        tmpJstart.TransferFunction(len, anglemnpltr);
                        unitsDataGridView.Rows[i].Cells[3].Style.BackColor = System.Drawing.Color.Red;
                        break;
                    case 'P':
                        tmpJstart.type = 'P';
                        tmpJend.type = 'S';
                        mnpltr.Q[i -1] = len;
                        tmpJend.TransferFunction(len, anglemnpltr);
                        mnpltr.addUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        tmpJstart.TransferFunction(len, anglemnpltr);
                        unitsDataGridView.Rows[i].Cells[2].Style.BackColor = System.Drawing.Color.Red;
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
            label2.Visible = true;
            mnpltr.Show(gr); // Отображение манипулятора в начальном положении
        }
        private void moveManipulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //double[] q = new double[4] { 0, 0, 75, 0};
            //double[] q = new double[numOfUnits - 2];
            ModelMnpltr = new MathModel(numOfUnits - 2);

            for (int i = 0; i < numOfUnits - 2; i++)
            {
                MathModel.len[i] = mnpltr.mnp[i + 1].lenght;
                MathModel.angle[i] = mnpltr.mnp[i + 1].angle;
                ModelMnpltr.dq[i] = mnpltr.Q[i];
            }

            progressBar1.Visible = true;
            CancelButton.Visible = true;
            backgroundWorker1.RunWorkerAsync();

            for (int i = 1; i < numOfUnits - 1; i++)
            {
                switch (mnpltr.mnp[i].start.type)
                {
                    case 'R':
                        unitsDataGridView.Rows[i].Cells[3].Value = mnpltr.Q[i - 1];
                        break;
                    case 'P':
                        unitsDataGridView.Rows[i].Cells[2].Value = mnpltr.Q[i - 1];
                        break;
                    default:
                        break;
                }
            }
        }

        private void destroyManipulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mnpltr.Hide(gr);
            //mnpltr.Dispose();
        }

        private void createPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S = new Path();
            Klabel.Visible = true;
            comboBox1.Visible = true;
            label3.Visible = true;
            flag = 1;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            switch(flag)
            {
                case 1:
                    S.AddAnchorPoint(e.Location);
                    label3.Text = $"Path lenght = {S.len.ToString("#.0000000000")}";
                    comboBox1.Items.Clear();
                    S.Show(gr);
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;
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
                if (Convert.ToInt32(comboBox1.Text) < (int)S.len && (S.len / Convert.ToInt32(comboBox1.Text) == 0))
                    MessageBox.Show("k should be longer than path lenght or it is too big");
                    //Число К должно быть больше длины пути или оно слишком большое
                else
                {
                    flag = 0;
                    k = Convert.ToInt32(comboBox1.Text);
                    S.SplitPath(k);
                    S.ShowExtraPoints(gr);
                }
            }
        }

        private void comboBox1_MouseDown(object sender, MouseEventArgs e)
        {
            comboBox1.Items.Add($"{(int)S.len + 1}");
            comboBox1.Items.Add($"{(int)S.len + 13}");
            comboBox1.Items.Add($"{(int)S.len * 2}");
            comboBox1.Items.Add($"{(int)S.len * 5}");
            comboBox1.Items.Add($"{(int)S.len * 10}");
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            PhysicsEngine.MovingAlongThePath(gr, S, ModelMnpltr, mnpltr, numOfUnits, k, backgroundWorker1);
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
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
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
