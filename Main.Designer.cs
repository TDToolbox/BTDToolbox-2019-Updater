namespace BTDToolbox_Updater
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Console = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.deleteFiles_list = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Update_Button = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.deleteFolders_list = new System.Windows.Forms.CheckedListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.None_Label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(31, 498);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(527, 41);
            this.progressBar1.TabIndex = 1;
            // 
            // Console
            // 
            this.Console.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.Console.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Console.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Console.ForeColor = System.Drawing.Color.White;
            this.Console.Location = new System.Drawing.Point(31, 398);
            this.Console.Name = "Console";
            this.Console.ReadOnly = true;
            this.Console.Size = new System.Drawing.Size(527, 110);
            this.Console.TabIndex = 2;
            this.Console.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(45, 382);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Console:";
            // 
            // deleteFiles_list
            // 
            this.deleteFiles_list.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.deleteFiles_list.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.deleteFiles_list.CheckOnClick = true;
            this.deleteFiles_list.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteFiles_list.ForeColor = System.Drawing.SystemColors.Info;
            this.deleteFiles_list.FormattingEnabled = true;
            this.deleteFiles_list.Location = new System.Drawing.Point(81, 247);
            this.deleteFiles_list.Name = "deleteFiles_list";
            this.deleteFiles_list.ScrollAlwaysVisible = true;
            this.deleteFiles_list.Size = new System.Drawing.Size(437, 80);
            this.deleteFiles_list.TabIndex = 5;
            this.deleteFiles_list.ThreeDCheckBoxes = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(78, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(214, 18);
            this.label2.TabIndex = 6;
            this.label2.Text = "Folders to delete during update:";
            // 
            // Update_Button
            // 
            this.Update_Button.Font = new System.Drawing.Font("Noto Mono", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Update_Button.Location = new System.Drawing.Point(370, 333);
            this.Update_Button.Name = "Update_Button";
            this.Update_Button.Size = new System.Drawing.Size(157, 29);
            this.Update_Button.TabIndex = 7;
            this.Update_Button.Text = "Begin Update";
            this.Update_Button.UseVisualStyleBackColor = true;
            this.Update_Button.Click += new System.EventHandler(this.Update_Button_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(78, 226);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(195, 18);
            this.label3.TabIndex = 9;
            this.label3.Text = "Files to delete during update:";
            // 
            // deleteFolders_list
            // 
            this.deleteFolders_list.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.deleteFolders_list.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.deleteFolders_list.CheckOnClick = true;
            this.deleteFolders_list.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteFolders_list.ForeColor = System.Drawing.SystemColors.Info;
            this.deleteFolders_list.FormattingEnabled = true;
            this.deleteFolders_list.Location = new System.Drawing.Point(81, 121);
            this.deleteFolders_list.Name = "deleteFolders_list";
            this.deleteFolders_list.ScrollAlwaysVisible = true;
            this.deleteFolders_list.Size = new System.Drawing.Size(437, 80);
            this.deleteFolders_list.TabIndex = 8;
            this.deleteFolders_list.ThreeDCheckBoxes = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::BTDToolbox_Updater.Properties.Resources.toolboxUpdater_img_2;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(71, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(465, 64);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::BTDToolbox_Updater.Properties.Resources.NK_Leaves;
            this.pictureBox2.Location = new System.Drawing.Point(-3, -4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(561, 177);
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            // 
            // None_Label
            // 
            this.None_Label.AutoSize = true;
            this.None_Label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.None_Label.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.None_Label.ForeColor = System.Drawing.Color.White;
            this.None_Label.Location = new System.Drawing.Point(86, 247);
            this.None_Label.Name = "None_Label";
            this.None_Label.Size = new System.Drawing.Size(45, 19);
            this.None_Label.TabIndex = 11;
            this.None_Label.Text = "None";
            this.None_Label.Visible = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(589, 557);
            this.Controls.Add(this.None_Label);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.deleteFolders_list);
            this.Controls.Add(this.Update_Button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.deleteFiles_list);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Console);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.pictureBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BTD Toolbox Updater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.RichTextBox Console;
        public System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.CheckedListBox deleteFiles_list;
        private System.Windows.Forms.Button Update_Button;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.CheckedListBox deleteFolders_list;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label None_Label;
    }
}

