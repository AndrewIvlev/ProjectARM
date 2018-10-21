using System;
using System.Drawing;
using System.Windows.Forms;

namespace projarm
{
    public partial class Form1 : Form
    {
        byte numOfUnits = 0;
        Graphics gr;
        Joint j1, j2, j3, j4, j5, j6;
        Unit Un1, Un2, Un3, Un4, Un5, Un6;
        public Form1()
        {
            j1 = new Joint('S', new Point(0, 0));
            j2 = new Joint('R', new Point(50, 0));
            j3 = new Joint('R', new Point(100, 0));
            j4 = new Joint('P', new Point(50, 0));
            j5 = new Joint('R', new Point(50, 0));
            j6 = new Joint('G', new Point(50, 0));
            Un1 = new Unit(j1, j1, 0, 0);
            Un2 = new Unit(j1, j2, 0, 0);
            Un3 = new Unit(j2, j3, 0, 0);
            Un4 = new Unit(j3, j4, 0, 0);
            Un5 = new Unit(j4, j5, 0, 0);
            Un6 = new Unit(j5, j6, 0, 0);
            gr = this.CreateGraphics();
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics actionArea = e.Graphics;
            Pen p = new Pen(Color.Black, 5);
            actionArea.DrawLine(p, new Point(270, 550), new Point(1000, 550));
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
            unitsDataGridView.Rows[1].Cells[2].Value = 20;
            unitsDataGridView.Rows[1].Cells[3].Value = 0;

            unitsDataGridView.Rows[2].Cells[1].Value = "R";
            unitsDataGridView.Rows[2].Cells[2].Value = 30;
            unitsDataGridView.Rows[2].Cells[3].Value = 0;

            unitsDataGridView.Rows[3].Cells[1].Value = "P";
            unitsDataGridView.Rows[3].Cells[2].Value = 40;
            unitsDataGridView.Rows[3].Cells[3].Value = 0;

            unitsDataGridView.Rows[4].Cells[1].Value = "R";
            unitsDataGridView.Rows[4].Cells[2].Value = 10;
            unitsDataGridView.Rows[4].Cells[3].Value = 0;

            unitsDataGridView.Rows[5].Cells[1].Value = "G";
            unitsDataGridView.Rows[5].Cells[2].Value = 0;
            unitsDataGridView.Rows[5].Cells[3].Value = 0;
        }

        private void CreateMnpltrButton_Click(object sender, EventArgs e)
        {
            Graphics gr = CreateGraphics();
            Manipulator mnpltr = new Manipulator(numOfUnits);
            //for (int i = 0; i < numOfUnits; i++)
                mnpltr.addUnit(Un1);
            mnpltr.addUnit(Un2);
            mnpltr.addUnit(Un3);
            mnpltr.addUnit(Un4);
            mnpltr.addUnit(Un5);
            mnpltr.addUnit(Un6);
            mnpltr.Show(gr);
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
        }
    }
}