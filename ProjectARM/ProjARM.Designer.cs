namespace ProjectARM
{
    partial class ProjARM
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjARM));
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.manipulatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctreateMnpltrToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveManipulatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.destroyManipulatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.interpolatePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.obstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createObstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editObstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.destrouObstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.followForToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Klabel = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.CancelMoveBtn = new System.Windows.Forms.Button();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.units = new System.Windows.Forms.DataGridView();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.CreateManipulator = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.NumOfUnitsTextBox = new System.Windows.Forms.TextBox();
            this.GoBtn = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.units)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(580, 355);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(257, 50);
            this.label2.TabIndex = 5;
            this.label2.Text = "Current Generalized Coordinates Q=(0,0,0,0)";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manipulatorToolStripMenuItem,
            this.pathToolStripMenuItem,
            this.obstacleToolStripMenuItem,
            this.followForToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(863, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // manipulatorToolStripMenuItem
            // 
            this.manipulatorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctreateMnpltrToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.moveManipulatorToolStripMenuItem,
            this.destroyManipulatorToolStripMenuItem});
            this.manipulatorToolStripMenuItem.Name = "manipulatorToolStripMenuItem";
            this.manipulatorToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.manipulatorToolStripMenuItem.Text = "Manipulator";
            // 
            // ctreateMnpltrToolStripMenuItem
            // 
            this.ctreateMnpltrToolStripMenuItem.Name = "ctreateMnpltrToolStripMenuItem";
            this.ctreateMnpltrToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ctreateMnpltrToolStripMenuItem.Text = "New";
            this.ctreateMnpltrToolStripMenuItem.Click += new System.EventHandler(this.newMnpltrToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // moveManipulatorToolStripMenuItem
            // 
            this.moveManipulatorToolStripMenuItem.Name = "moveManipulatorToolStripMenuItem";
            this.moveManipulatorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.moveManipulatorToolStripMenuItem.Text = "Move by Path";
            this.moveManipulatorToolStripMenuItem.Click += new System.EventHandler(this.moveManipulatorToolStripMenuItem_Click);
            // 
            // destroyManipulatorToolStripMenuItem
            // 
            this.destroyManipulatorToolStripMenuItem.Name = "destroyManipulatorToolStripMenuItem";
            this.destroyManipulatorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.destroyManipulatorToolStripMenuItem.Text = "Delete";
            this.destroyManipulatorToolStripMenuItem.Click += new System.EventHandler(this.destroyManipulatorToolStripMenuItem_Click);
            // 
            // pathToolStripMenuItem
            // 
            this.pathToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createPathToolStripMenuItem,
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem1,
            this.sToolStripMenuItem,
            this.editPathToolStripMenuItem,
            this.interpolatePathToolStripMenuItem,
            this.deletePathToolStripMenuItem});
            this.pathToolStripMenuItem.Name = "pathToolStripMenuItem";
            this.pathToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.pathToolStripMenuItem.Text = "Path";
            // 
            // createPathToolStripMenuItem
            // 
            this.createPathToolStripMenuItem.Name = "createPathToolStripMenuItem";
            this.createPathToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.createPathToolStripMenuItem.Text = "New";
            this.createPathToolStripMenuItem.Click += new System.EventHandler(this.createPathToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.openToolStripMenuItem1.Text = "Open";
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            // 
            // sToolStripMenuItem
            // 
            this.sToolStripMenuItem.Name = "sToolStripMenuItem";
            this.sToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.sToolStripMenuItem.Text = "Save as";
            // 
            // editPathToolStripMenuItem
            // 
            this.editPathToolStripMenuItem.Name = "editPathToolStripMenuItem";
            this.editPathToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.editPathToolStripMenuItem.Text = "Edit";
            this.editPathToolStripMenuItem.Click += new System.EventHandler(this.editPathToolStripMenuItem_Click);
            // 
            // interpolatePathToolStripMenuItem
            // 
            this.interpolatePathToolStripMenuItem.Name = "interpolatePathToolStripMenuItem";
            this.interpolatePathToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.interpolatePathToolStripMenuItem.Text = "Interpolate";
            this.interpolatePathToolStripMenuItem.Click += new System.EventHandler(this.interpolatePathToolStripMenuItem_Click);
            // 
            // deletePathToolStripMenuItem
            // 
            this.deletePathToolStripMenuItem.Name = "deletePathToolStripMenuItem";
            this.deletePathToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.deletePathToolStripMenuItem.Text = "Delete";
            this.deletePathToolStripMenuItem.Click += new System.EventHandler(this.deletePathToolStripMenuItem_Click);
            // 
            // obstacleToolStripMenuItem
            // 
            this.obstacleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createObstacleToolStripMenuItem,
            this.editObstacleToolStripMenuItem,
            this.destrouObstacleToolStripMenuItem,
            this.deleteAllToolStripMenuItem});
            this.obstacleToolStripMenuItem.Name = "obstacleToolStripMenuItem";
            this.obstacleToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.obstacleToolStripMenuItem.Text = "Obstacle";
            // 
            // createObstacleToolStripMenuItem
            // 
            this.createObstacleToolStripMenuItem.Name = "createObstacleToolStripMenuItem";
            this.createObstacleToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.createObstacleToolStripMenuItem.Text = "New";
            // 
            // editObstacleToolStripMenuItem
            // 
            this.editObstacleToolStripMenuItem.Name = "editObstacleToolStripMenuItem";
            this.editObstacleToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.editObstacleToolStripMenuItem.Text = "Open";
            // 
            // destrouObstacleToolStripMenuItem
            // 
            this.destrouObstacleToolStripMenuItem.Name = "destrouObstacleToolStripMenuItem";
            this.destrouObstacleToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.destrouObstacleToolStripMenuItem.Text = "Delete";
            // 
            // deleteAllToolStripMenuItem
            // 
            this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
            this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.deleteAllToolStripMenuItem.Text = "Delete All";
            // 
            // followForToolStripMenuItem
            // 
            this.followForToolStripMenuItem.Name = "followForToolStripMenuItem";
            this.followForToolStripMenuItem.Size = new System.Drawing.Size(129, 20);
            this.followForToolStripMenuItem.Text = "Following the Cursor";
            this.followForToolStripMenuItem.Click += new System.EventHandler(this.followForToolStripMenuItem_Click);
            // 
            // Klabel
            // 
            this.Klabel.AutoSize = true;
            this.Klabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Klabel.Location = new System.Drawing.Point(580, 35);
            this.Klabel.Name = "Klabel";
            this.Klabel.Size = new System.Drawing.Size(269, 20);
            this.Klabel.TabIndex = 9;
            this.Klabel.Text = "Divide the path into           points";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(745, 35);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(45, 21);
            this.comboBox1.TabIndex = 10;
            this.comboBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBox1_KeyDown);
            this.comboBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.comboBox1_MouseDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(580, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(159, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "Path lenght = 0 cm";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(584, 314);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(178, 27);
            this.progressBar1.TabIndex = 12;
            // 
            // CancelMoveBtn
            // 
            this.CancelMoveBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CancelMoveBtn.Location = new System.Drawing.Point(764, 314);
            this.CancelMoveBtn.Name = "CancelMoveBtn";
            this.CancelMoveBtn.Size = new System.Drawing.Size(85, 27);
            this.CancelMoveBtn.TabIndex = 13;
            this.CancelMoveBtn.Text = "Cancel";
            this.CancelMoveBtn.UseVisualStyleBackColor = true;
            this.CancelMoveBtn.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // chart1
            // 
            chartArea1.AxisX.Title = "iteration";
            chartArea1.AxisX.TitleAlignment = System.Drawing.StringAlignment.Far;
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            chartArea1.AxisX.TitleForeColor = System.Drawing.Color.Red;
            chartArea1.AxisY.Title = "Delta, cm";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            chartArea1.AxisY.TitleForeColor = System.Drawing.Color.Red;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Location = new System.Drawing.Point(573, 111);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(269, 188);
            this.chart1.TabIndex = 14;
            this.chart1.Text = "delta";
            title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            title1.Name = "Title1";
            title1.Text = "Delta = ||p(i+1) - p\'(i+1)||";
            this.chart1.Titles.Add(title1);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(283, 35);
            this.pictureBox.MinimumSize = new System.Drawing.Size(293, 355);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(293, 355);
            this.pictureBox.TabIndex = 15;
            this.pictureBox.TabStop = false;
            this.pictureBox.Layout += new System.Windows.Forms.LayoutEventHandler(this.pictureBox_Layout);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(580, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(262, 20);
            this.label4.TabIndex = 16;
            this.label4.Text = "Or select a split step:            cm";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(764, 61);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(45, 21);
            this.comboBox2.TabIndex = 17;
            this.comboBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBox2_KeyDown);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // units
            // 
            this.units.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.units.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.units.Location = new System.Drawing.Point(16, 76);
            this.units.Name = "units";
            this.units.RowHeadersVisible = false;
            this.units.RowHeadersWidth = 20;
            this.units.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.units.Size = new System.Drawing.Size(252, 16);
            this.units.TabIndex = 18;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CancelBtn.Location = new System.Drawing.Point(16, 111);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 27);
            this.CancelBtn.TabIndex = 23;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // CreateManipulator
            // 
            this.CreateManipulator.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CreateManipulator.Location = new System.Drawing.Point(183, 111);
            this.CreateManipulator.Name = "CreateManipulator";
            this.CreateManipulator.Size = new System.Drawing.Size(85, 27);
            this.CreateManipulator.TabIndex = 22;
            this.CreateManipulator.Text = "Create";
            this.CreateManipulator.UseVisualStyleBackColor = true;
            this.CreateManipulator.Click += new System.EventHandler(this.CreateManipulator_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 20);
            this.label1.TabIndex = 19;
            this.label1.Text = "Number of Units";
            // 
            // NumOfUnitsTextBox
            // 
            this.NumOfUnitsTextBox.Location = new System.Drawing.Point(149, 40);
            this.NumOfUnitsTextBox.Name = "NumOfUnitsTextBox";
            this.NumOfUnitsTextBox.Size = new System.Drawing.Size(46, 20);
            this.NumOfUnitsTextBox.TabIndex = 20;
            this.NumOfUnitsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NumOfUnitsTextBox_KeyDown);
            // 
            // GoBtn
            // 
            this.GoBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GoBtn.Location = new System.Drawing.Point(201, 35);
            this.GoBtn.Name = "GoBtn";
            this.GoBtn.Size = new System.Drawing.Size(67, 27);
            this.GoBtn.TabIndex = 21;
            this.GoBtn.Text = "Go";
            this.GoBtn.UseVisualStyleBackColor = true;
            this.GoBtn.Click += new System.EventHandler(this.GoBtn_Click);
            // 
            // ProjARM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(863, 402);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.CreateManipulator);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NumOfUnitsTextBox);
            this.Controls.Add(this.GoBtn);
            this.Controls.Add(this.units);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.CancelMoveBtn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.Klabel);
            this.Controls.Add(this.label4);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(879, 440);
            this.Name = "ProjARM";
            this.Text = "ProjectARM";
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ProjARM_Layout);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.units)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem manipulatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ctreateMnpltrToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveManipulatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem destroyManipulatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deletePathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem obstacleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createObstacleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editObstacleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem destrouObstacleToolStripMenuItem;
        private System.Windows.Forms.Label Klabel;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button CancelMoveBtn;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ToolStripMenuItem followForToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem interpolatePathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.DataGridView units;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button CreateManipulator;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NumOfUnitsTextBox;
        private System.Windows.Forms.Button GoBtn;
    }
}

