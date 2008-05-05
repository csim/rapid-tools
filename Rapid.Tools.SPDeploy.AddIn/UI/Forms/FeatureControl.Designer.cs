namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    partial class FeatureControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.ddlScope = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtFeatureDescription = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFeatureTitle = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblFeature = new System.Windows.Forms.Label();
            this.rbNewFeature = new System.Windows.Forms.RadioButton();
            this.rbExistingFeature = new System.Windows.Forms.RadioButton();
            this.lblNew = new System.Windows.Forms.Label();
            this.cbFeatureTitle = new System.Windows.Forms.ComboBox();
            this.lblExisting = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtImageUrl = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cbActivateOnDefault = new System.Windows.Forms.CheckBox();
            this.cbHidden = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ddlScope);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.txtFeatureDescription);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtFeatureTitle);
            this.panel1.Location = new System.Drawing.Point(0, 114);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(426, 94);
            this.panel1.TabIndex = 119;
            // 
            // ddlScope
            // 
            this.ddlScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlScope.FormattingEnabled = true;
            this.ddlScope.Items.AddRange(new object[] {
            "Farm",
            "WebApplication",
            "Site",
            "Web"});
            this.ddlScope.Location = new System.Drawing.Point(113, 61);
            this.ddlScope.Name = "ddlScope";
            this.ddlScope.Size = new System.Drawing.Size(304, 21);
            this.ddlScope.TabIndex = 73;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(27, 64);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 83;
            this.label9.Text = "Scope";
            // 
            // txtFeatureDescription
            // 
            this.txtFeatureDescription.Location = new System.Drawing.Point(113, 34);
            this.txtFeatureDescription.Name = "txtFeatureDescription";
            this.txtFeatureDescription.Size = new System.Drawing.Size(304, 20);
            this.txtFeatureDescription.TabIndex = 72;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 80;
            this.label5.Text = "Description";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 79;
            this.label1.Text = "Title";
            // 
            // txtFeatureTitle
            // 
            this.txtFeatureTitle.Location = new System.Drawing.Point(113, 8);
            this.txtFeatureTitle.Name = "txtFeatureTitle";
            this.txtFeatureTitle.Size = new System.Drawing.Size(304, 20);
            this.txtFeatureTitle.TabIndex = 71;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(27, 60);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(27, 13);
            this.lblTitle.TabIndex = 82;
            this.lblTitle.Text = "Title";
            // 
            // lblFeature
            // 
            this.lblFeature.AutoSize = true;
            this.lblFeature.Location = new System.Drawing.Point(24, 11);
            this.lblFeature.Name = "lblFeature";
            this.lblFeature.Size = new System.Drawing.Size(43, 13);
            this.lblFeature.TabIndex = 81;
            this.lblFeature.Text = "Feature";
            // 
            // rbNewFeature
            // 
            this.rbNewFeature.AutoSize = true;
            this.rbNewFeature.Location = new System.Drawing.Point(7, 103);
            this.rbNewFeature.Name = "rbNewFeature";
            this.rbNewFeature.Size = new System.Drawing.Size(14, 13);
            this.rbNewFeature.TabIndex = 78;
            this.rbNewFeature.TabStop = true;
            this.rbNewFeature.UseVisualStyleBackColor = true;
            // 
            // rbExistingFeature
            // 
            this.rbExistingFeature.AutoSize = true;
            this.rbExistingFeature.Location = new System.Drawing.Point(7, 38);
            this.rbExistingFeature.Name = "rbExistingFeature";
            this.rbExistingFeature.Size = new System.Drawing.Size(14, 13);
            this.rbExistingFeature.TabIndex = 77;
            this.rbExistingFeature.TabStop = true;
            this.rbExistingFeature.UseVisualStyleBackColor = true;
            // 
            // lblNew
            // 
            this.lblNew.AutoSize = true;
            this.lblNew.Location = new System.Drawing.Point(24, 103);
            this.lblNew.Name = "lblNew";
            this.lblNew.Size = new System.Drawing.Size(68, 13);
            this.lblNew.TabIndex = 76;
            this.lblNew.Text = "New Feature";
            // 
            // cbFeatureTitle
            // 
            this.cbFeatureTitle.AllowDrop = true;
            this.cbFeatureTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFeatureTitle.FormattingEnabled = true;
            this.cbFeatureTitle.Location = new System.Drawing.Point(113, 57);
            this.cbFeatureTitle.Name = "cbFeatureTitle";
            this.cbFeatureTitle.Size = new System.Drawing.Size(304, 21);
            this.cbFeatureTitle.TabIndex = 75;
            // 
            // lblExisting
            // 
            this.lblExisting.AutoSize = true;
            this.lblExisting.Location = new System.Drawing.Point(24, 38);
            this.lblExisting.Name = "lblExisting";
            this.lblExisting.Size = new System.Drawing.Size(82, 13);
            this.lblExisting.TabIndex = 74;
            this.lblExisting.Text = "Existing Feature";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtImageUrl);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.cbActivateOnDefault);
            this.panel2.Controls.Add(this.cbHidden);
            this.panel2.Location = new System.Drawing.Point(0, 208);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(426, 85);
            this.panel2.TabIndex = 120;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Image Url";
            // 
            // txtImageUrl
            // 
            this.txtImageUrl.Location = new System.Drawing.Point(113, 10);
            this.txtImageUrl.Name = "txtImageUrl";
            this.txtImageUrl.Size = new System.Drawing.Size(304, 20);
            this.txtImageUrl.TabIndex = 22;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Activate on Default";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(58, 36);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "Hidden";
            // 
            // cbActivateOnDefault
            // 
            this.cbActivateOnDefault.AutoSize = true;
            this.cbActivateOnDefault.Location = new System.Drawing.Point(113, 56);
            this.cbActivateOnDefault.Name = "cbActivateOnDefault";
            this.cbActivateOnDefault.Size = new System.Drawing.Size(15, 14);
            this.cbActivateOnDefault.TabIndex = 20;
            this.cbActivateOnDefault.UseVisualStyleBackColor = true;
            // 
            // cbHidden
            // 
            this.cbHidden.AutoSize = true;
            this.cbHidden.Location = new System.Drawing.Point(113, 36);
            this.cbHidden.Name = "cbHidden";
            this.cbHidden.Size = new System.Drawing.Size(15, 14);
            this.cbHidden.TabIndex = 19;
            this.cbHidden.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblExisting);
            this.panel3.Controls.Add(this.rbExistingFeature);
            this.panel3.Controls.Add(this.lblTitle);
            this.panel3.Controls.Add(this.lblFeature);
            this.panel3.Controls.Add(this.cbFeatureTitle);
            this.panel3.Controls.Add(this.lblNew);
            this.panel3.Controls.Add(this.rbNewFeature);
            this.panel3.Location = new System.Drawing.Point(0, -5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(426, 123);
            this.panel3.TabIndex = 84;
            // 
            // FeatureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Name = "FeatureControl";
            this.Size = new System.Drawing.Size(429, 296);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox ddlScope;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtFeatureDescription;
        private System.Windows.Forms.Label lblFeature;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFeatureTitle;
        private System.Windows.Forms.RadioButton rbNewFeature;
        private System.Windows.Forms.RadioButton rbExistingFeature;
        private System.Windows.Forms.Label lblNew;
        private System.Windows.Forms.ComboBox cbFeatureTitle;
        private System.Windows.Forms.Label lblExisting;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtImageUrl;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbActivateOnDefault;
        private System.Windows.Forms.CheckBox cbHidden;
        private System.Windows.Forms.Panel panel3;
    }
}
