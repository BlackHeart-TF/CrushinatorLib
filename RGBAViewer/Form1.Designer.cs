
namespace RGBAViewer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.suggestComboBox1 = new RGBAViewer.SuggestComboBox();
            this.suggestComboBox2 = new RGBAViewer.SuggestComboBox();
            this.suggestComboBox3 = new RGBAViewer.SuggestComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(0, 123);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(445, 343);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(358, 63);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Extract File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(72, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(280, 23);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "SubItem:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "File:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "POD:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path:";
            // 
            // suggestComboBox1
            // 
            this.suggestComboBox1.FilterRule = null;
            this.suggestComboBox1.FormattingEnabled = true;
            this.suggestComboBox1.Location = new System.Drawing.Point(72, 35);
            this.suggestComboBox1.Name = "suggestComboBox1";
            this.suggestComboBox1.PropertySelector = null;
            this.suggestComboBox1.Size = new System.Drawing.Size(280, 23);
            this.suggestComboBox1.SuggestBoxHeight = 96;
            this.suggestComboBox1.SuggestListOrderRule = null;
            this.suggestComboBox1.TabIndex = 1;
            this.suggestComboBox1.SelectedIndexChanged += new System.EventHandler(this.suggestComboBox1_SelectedIndexChanged);
            this.suggestComboBox1.DataSourceChanged += new System.EventHandler(this.DataSourceChanged);
            // 
            // suggestComboBox2
            // 
            this.suggestComboBox2.FilterRule = null;
            this.suggestComboBox2.FormattingEnabled = true;
            this.suggestComboBox2.Location = new System.Drawing.Point(72, 63);
            this.suggestComboBox2.Name = "suggestComboBox2";
            this.suggestComboBox2.PropertySelector = null;
            this.suggestComboBox2.Size = new System.Drawing.Size(280, 23);
            this.suggestComboBox2.SuggestBoxHeight = 96;
            this.suggestComboBox2.SuggestListOrderRule = null;
            this.suggestComboBox2.TabIndex = 2;
            this.suggestComboBox2.SelectedIndexChanged += new System.EventHandler(this.suggestComboBox2_SelectedIndexChanged);
            this.suggestComboBox2.DataSourceChanged += new System.EventHandler(this.DataSourceChanged);
            // 
            // suggestComboBox3
            // 
            this.suggestComboBox3.FilterRule = null;
            this.suggestComboBox3.FormattingEnabled = true;
            this.suggestComboBox3.Location = new System.Drawing.Point(72, 92);
            this.suggestComboBox3.Name = "suggestComboBox3";
            this.suggestComboBox3.PropertySelector = null;
            this.suggestComboBox3.Size = new System.Drawing.Size(280, 23);
            this.suggestComboBox3.SuggestBoxHeight = 96;
            this.suggestComboBox3.SuggestListOrderRule = null;
            this.suggestComboBox3.TabIndex = 3;
            this.suggestComboBox3.SelectedIndexChanged += new System.EventHandler(this.suggestComboBox3_SelectedIndexChanged);
            this.suggestComboBox3.DataSourceChanged += new System.EventHandler(this.DataSourceChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(358, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Browse";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 466);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.suggestComboBox3);
            this.Controls.Add(this.suggestComboBox2);
            this.Controls.Add(this.suggestComboBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "That one pod viewer tool that Noire made that time V:0.1-lol browse";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private SuggestComboBox suggestComboBox1;
        private SuggestComboBox suggestComboBox2;
        private SuggestComboBox suggestComboBox3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

