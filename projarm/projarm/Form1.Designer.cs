namespace projarm
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.NumofUnits = new System.Windows.Forms.TextBox();
            this.unitsDataGridView = new System.Windows.Forms.DataGridView();
            this.DefaultManipulator = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.manipulatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctreateMnpltrToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveManipulatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.destroyManipulatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.obstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createObstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editObstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.destrouObstacleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Klabel = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.CancelButton = new System.Windows.Forms.Button();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.unitsDataGridView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(11, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of Units";
            // 
            // NumofUnits
            // 
            this.NumofUnits.Location = new System.Drawing.Point(147, 37);
            this.NumofUnits.Name = "NumofUnits";
            this.NumofUnits.Size = new System.Drawing.Size(30, 20);
            this.NumofUnits.TabIndex = 1;
            this.NumofUnits.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NumofUnits_KeyDown);
            // 
            // unitsDataGridView
            // 
            this.unitsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.unitsDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.unitsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.unitsDataGridView.Location = new System.Drawing.Point(15, 67);
            this.unitsDataGridView.Name = "unitsDataGridView";
            this.unitsDataGridView.RowHeadersWidth = 20;
            this.unitsDataGridView.Size = new System.Drawing.Size(252, 143);
            this.unitsDataGridView.TabIndex = 2;
            this.unitsDataGridView.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.unitsDataGridView_ColumnAdded);
            // 
            // DefaultManipulator
            // 
            this.DefaultManipulator.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DefaultManipulator.Location = new System.Drawing.Point(183, 34);
            this.DefaultManipulator.Name = "DefaultManipulator";
            this.DefaultManipulator.Size = new System.Drawing.Size(85, 27);
            this.DefaultManipulator.TabIndex = 3;
            this.DefaultManipulator.Text = "Default";
            this.DefaultManipulator.UseVisualStyleBackColor = true;
            this.DefaultManipulator.Click += new System.EventHandler(this.DefaultManipulator_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(13, 213);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(257, 50);
            this.label2.TabIndex = 5;
            this.label2.Text = "Generalized Coordinates Q=(0,0,0,0)";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manipulatorToolStripMenuItem,
            this.pathToolStripMenuItem,
            this.obstacleToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1064, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // manipulatorToolStripMenuItem
            // 
            this.manipulatorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctreateMnpltrToolStripMenuItem,
            this.moveManipulatorToolStripMenuItem,
            this.destroyManipulatorToolStripMenuItem});
            this.manipulatorToolStripMenuItem.Name = "manipulatorToolStripMenuItem";
            this.manipulatorToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.manipulatorToolStripMenuItem.Text = "Manipulator";
            // 
            // ctreateMnpltrToolStripMenuItem
            // 
            this.ctreateMnpltrToolStripMenuItem.Name = "ctreateMnpltrToolStripMenuItem";
            this.ctreateMnpltrToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.ctreateMnpltrToolStripMenuItem.Text = "Ctreate Manipulator";
            this.ctreateMnpltrToolStripMenuItem.Click += new System.EventHandler(this.ctreateMnpltrToolStripMenuItem_Click);
            // 
            // moveManipulatorToolStripMenuItem
            // 
            this.moveManipulatorToolStripMenuItem.Name = "moveManipulatorToolStripMenuItem";
            this.moveManipulatorToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.moveManipulatorToolStripMenuItem.Text = "Move Manipulator";
            this.moveManipulatorToolStripMenuItem.Click += new System.EventHandler(this.moveManipulatorToolStripMenuItem_Click);
            // 
            // destroyManipulatorToolStripMenuItem
            // 
            this.destroyManipulatorToolStripMenuItem.Name = "destroyManipulatorToolStripMenuItem";
            this.destroyManipulatorToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.destroyManipulatorToolStripMenuItem.Text = "Destroy Manipulator";
            this.destroyManipulatorToolStripMenuItem.Click += new System.EventHandler(this.destroyManipulatorToolStripMenuItem_Click);
            // 
            // pathToolStripMenuItem
            // 
            this.pathToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createPathToolStripMenuItem,
            this.editPathToolStripMenuItem,
            this.deletePathToolStripMenuItem});
            this.pathToolStripMenuItem.Name = "pathToolStripMenuItem";
            this.pathToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.pathToolStripMenuItem.Text = "Path";
            // 
            // createPathToolStripMenuItem
            // 
            this.createPathToolStripMenuItem.Name = "createPathToolStripMenuItem";
            this.createPathToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.createPathToolStripMenuItem.Text = "Create Path";
            this.createPathToolStripMenuItem.Click += new System.EventHandler(this.createPathToolStripMenuItem_Click);
            // 
            // editPathToolStripMenuItem
            // 
            this.editPathToolStripMenuItem.Name = "editPathToolStripMenuItem";
            this.editPathToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.editPathToolStripMenuItem.Text = "Edit Path";
            this.editPathToolStripMenuItem.Click += new System.EventHandler(this.editPathToolStripMenuItem_Click);
            // 
            // deletePathToolStripMenuItem
            // 
            this.deletePathToolStripMenuItem.Name = "deletePathToolStripMenuItem";
            this.deletePathToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.deletePathToolStripMenuItem.Text = "Delete Path";
            this.deletePathToolStripMenuItem.Click += new System.EventHandler(this.deletePathToolStripMenuItem_Click);
            // 
            // obstacleToolStripMenuItem
            // 
            this.obstacleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createObstacleToolStripMenuItem,
            this.editObstacleToolStripMenuItem,
            this.destrouObstacleToolStripMenuItem});
            this.obstacleToolStripMenuItem.Name = "obstacleToolStripMenuItem";
            this.obstacleToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.obstacleToolStripMenuItem.Text = "Obstacle";
            // 
            // createObstacleToolStripMenuItem
            // 
            this.createObstacleToolStripMenuItem.Name = "createObstacleToolStripMenuItem";
            this.createObstacleToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.createObstacleToolStripMenuItem.Text = "Create Obstacle";
            // 
            // editObstacleToolStripMenuItem
            // 
            this.editObstacleToolStripMenuItem.Name = "editObstacleToolStripMenuItem";
            this.editObstacleToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.editObstacleToolStripMenuItem.Text = "Edit Obstacle";
            // 
            // destrouObstacleToolStripMenuItem
            // 
            this.destrouObstacleToolStripMenuItem.Name = "destrouObstacleToolStripMenuItem";
            this.destrouObstacleToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.destrouObstacleToolStripMenuItem.Text = "Destrou Obstacle";
            // 
            // Klabel
            // 
            this.Klabel.AutoSize = true;
            this.Klabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Klabel.Location = new System.Drawing.Point(11, 292);
            this.Klabel.Name = "Klabel";
            this.Klabel.Size = new System.Drawing.Size(259, 20);
            this.Klabel.TabIndex = 9;
            this.Klabel.Text = "Divide the path into         points";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(174, 294);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(40, 21);
            this.comboBox1.TabIndex = 10;
            this.comboBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBox1_KeyDown);
            this.comboBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.comboBox1_MouseDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(12, 312);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "Path lenght = 0";
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
            this.progressBar1.Location = new System.Drawing.Point(16, 257);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(161, 23);
            this.progressBar1.TabIndex = 12;
            // 
            // CancelButton
            // 
            this.CancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CancelButton.Location = new System.Drawing.Point(182, 255);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(85, 27);
            this.CancelButton.TabIndex = 13;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.WorkerReportsProgress = true;
            this.backgroundWorker2.WorkerSupportsCancellation = true;
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker2_ProgressChanged);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1064, 682);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DefaultManipulator);
            this.Controls.Add(this.unitsDataGridView);
            this.Controls.Add(this.NumofUnits);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.Klabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(540, 360);
            this.Name = "Form1";
            this.Text = "ProjectARM";
            this.ResizeBegin += new System.EventHandler(this.Form1_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.unitsDataGridView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NumofUnits;
        private System.Windows.Forms.DataGridView unitsDataGridView;
        private System.Windows.Forms.Button DefaultManipulator;
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
        private System.Windows.Forms.Button CancelButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
    }
}

