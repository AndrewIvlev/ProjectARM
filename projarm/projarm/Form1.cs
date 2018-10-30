using System;
using System.Drawing;
using System.Windows.Forms;

namespace projarm
{
    public partial class Form1 : Form
    {
        byte numOfUnits = 0;
        Manipulator mnpltr;
        public Graphics gr;
        Point[] dots;
        Joint[] joints;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics actionArea = e.Graphics;
            gr = CreateGraphics();
            dots = new Point[25];
            joints = new Joint[25];
            joints[0] = new Joint('D');
            joints[1] = new Joint('D');
            Pen p = new Pen(Color.Black, 5);
            actionArea.DrawLine(p, new Point(270, 552), new Point(1000, 552));
            actionArea.DrawLine(p, new Point(272, 0), new Point(272, 550));
            actionArea.DrawLine(p, new Point(997, 0), new Point(997, 550));
            actionArea.DrawString("ProjectARM", new Font("Arial", 24),
                new SolidBrush(System.Drawing.Color.Red), new Point(550, 220));
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

        private void CreateMnpltrButton_Click(object sender, EventArgs e)
        {
            mnpltr = new Manipulator(numOfUnits);
            double anglemnpltr = 0;
            Joint tmpJstart = new Joint('S');
            Joint tmpJend = new Joint('S');

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
                        unitsDataGridView.Rows[i].Cells[3].Style.BackColor = System.Drawing.Color.Red;
                        break;
                    case 'P':
                        tmpJstart.type = 'P';
                        tmpJend.type = 'S';
                        tmpJend.TransferFunction(len, anglemnpltr);
                        mnpltr.addUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        tmpJstart.TransferFunction(len, anglemnpltr);
                        unitsDataGridView.Rows[i].Cells[2].Style.BackColor = System.Drawing.Color.Red;
                        break;
                    case 'G':
                        tmpJstart.type = tmpJend.type = 'G';
                        tmpJend.TransferFunction(len, anglemnpltr);
                        mnpltr.addUnit(new Unit(tmpJstart, tmpJend, len, anglemnpltr));
                        //tmpJstart.TransferFunction(len, anglemnpltr);
                        break;
                    default:
                        break;
                }
            }
            label2.Visible = true;
            mnpltr.Show(gr);
        }

        private void DestroyMnpltrButton_Click(object sender, EventArgs e)
        {
           mnpltr.Hide(gr);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                joints[0].Hide(gr);
                joints[0].Dot = new Point(Form1.MousePosition.X, 
                    Form1.MousePosition.Y);
                joints[0].Show(gr);
            }
            else if (e.Button == MouseButtons.Right)
            {
                joints[1].Hide(gr);
                joints[1].Dot = new Point(Form1.MousePosition.X,
                    Form1.MousePosition.Y);
                joints[1].Show(gr);
            }
        }

        private void MoveButon_Click(object sender, EventArgs e)
        {
            double[] q = new double[numOfUnits];
            q[0] = 0; q[numOfUnits - 1] = 0;
            for (int i = 1; i < numOfUnits - 1; i++)
            {
                switch (mnpltr.mnp[i].start.type)
                {
                    case 'R':
                        q[i] = Convert.ToDouble(unitsDataGridView.Rows[i].Cells[3].Value.ToString());
                        break;
                    case 'P':
                        q[i] = Convert.ToDouble(unitsDataGridView.Rows[i].Cells[2].Value.ToString());
                        break;
                    default:
                        break;
                }
                mnpltr.Move(gr, q);
            }
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
