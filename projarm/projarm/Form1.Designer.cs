﻿namespace projarm
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
            this.label1 = new System.Windows.Forms.Label();
            this.NumofUnits = new System.Windows.Forms.TextBox();
            this.unitsDataGridView = new System.Windows.Forms.DataGridView();
            this.DefaultManipulator = new System.Windows.Forms.Button();
            this.CreateMnpltrButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.DestroyMnpltrButton = new System.Windows.Forms.Button();
            this.MoveButon = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.unitsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of Units";
            // 
            // NumofUnits
            // 
            this.NumofUnits.Location = new System.Drawing.Point(149, 9);
            this.NumofUnits.Name = "NumofUnits";
            this.NumofUnits.Size = new System.Drawing.Size(29, 20);
            this.NumofUnits.TabIndex = 1;
            this.NumofUnits.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NumofUnits_KeyDown);
            // 
            // unitsDataGridView
            // 
            this.unitsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.unitsDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.unitsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.unitsDataGridView.Location = new System.Drawing.Point(16, 42);
            this.unitsDataGridView.Name = "unitsDataGridView";
            this.unitsDataGridView.RowHeadersWidth = 20;
            this.unitsDataGridView.Size = new System.Drawing.Size(252, 143);
            this.unitsDataGridView.TabIndex = 2;
            this.unitsDataGridView.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.unitsDataGridView_ColumnAdded);
            // 
            // DefaultManipulator
            // 
            this.DefaultManipulator.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DefaultManipulator.Location = new System.Drawing.Point(184, 6);
            this.DefaultManipulator.Name = "DefaultManipulator";
            this.DefaultManipulator.Size = new System.Drawing.Size(84, 27);
            this.DefaultManipulator.TabIndex = 3;
            this.DefaultManipulator.Text = "Default";
            this.DefaultManipulator.UseVisualStyleBackColor = true;
            this.DefaultManipulator.Click += new System.EventHandler(this.DefaultManipulator_Click);
            // 
            // CreateMnpltrButton
            // 
            this.CreateMnpltrButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CreateMnpltrButton.Location = new System.Drawing.Point(16, 190);
            this.CreateMnpltrButton.Name = "CreateMnpltrButton";
            this.CreateMnpltrButton.Size = new System.Drawing.Size(252, 33);
            this.CreateMnpltrButton.TabIndex = 4;
            this.CreateMnpltrButton.Text = "Create Manipulator";
            this.CreateMnpltrButton.UseVisualStyleBackColor = true;
            this.CreateMnpltrButton.Click += new System.EventHandler(this.CreateMnpltrButton_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(16, 273);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(252, 48);
            this.label2.TabIndex = 5;
            this.label2.Text = "В красных ячейках значение обобщенных координат";
            this.label2.Visible = false;
            // 
            // DestroyMnpltrButton
            // 
            this.DestroyMnpltrButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DestroyMnpltrButton.Location = new System.Drawing.Point(16, 229);
            this.DestroyMnpltrButton.Name = "DestroyMnpltrButton";
            this.DestroyMnpltrButton.Size = new System.Drawing.Size(252, 33);
            this.DestroyMnpltrButton.TabIndex = 6;
            this.DestroyMnpltrButton.Text = "Destroy Manipulator";
            this.DestroyMnpltrButton.UseVisualStyleBackColor = true;
            this.DestroyMnpltrButton.Click += new System.EventHandler(this.DestroyMnpltrButton_Click);
            // 
            // MoveButon
            // 
            this.MoveButon.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MoveButon.Location = new System.Drawing.Point(20, 324);
            this.MoveButon.Name = "MoveButon";
            this.MoveButon.Size = new System.Drawing.Size(89, 33);
            this.MoveButon.TabIndex = 7;
            this.MoveButon.Text = "MOVE";
            this.MoveButon.UseVisualStyleBackColor = true;
            this.MoveButon.Click += new System.EventHandler(this.MoveButon_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1002, 556);
            this.Controls.Add(this.MoveButon);
            this.Controls.Add(this.DestroyMnpltrButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CreateMnpltrButton);
            this.Controls.Add(this.DefaultManipulator);
            this.Controls.Add(this.unitsDataGridView);
            this.Controls.Add(this.NumofUnits);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "ProjectARM";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.unitsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NumofUnits;
        private System.Windows.Forms.DataGridView unitsDataGridView;
        private System.Windows.Forms.Button DefaultManipulator;
        private System.Windows.Forms.Button CreateMnpltrButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button DestroyMnpltrButton;
        private System.Windows.Forms.Button MoveButon;
    }
}

