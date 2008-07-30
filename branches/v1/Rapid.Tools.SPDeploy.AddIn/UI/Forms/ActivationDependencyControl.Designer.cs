namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    partial class ActivationDependencyControl
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
            this.components = new System.ComponentModel.Container();
            this.ddlFeatureId = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFeatureId = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbInternal = new System.Windows.Forms.RadioButton();
            this.rbExternal = new System.Windows.Forms.RadioButton();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // ddlFeatureId
            // 
            this.ddlFeatureId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlFeatureId.FormattingEnabled = true;
            this.ddlFeatureId.Location = new System.Drawing.Point(125, 47);
            this.ddlFeatureId.Name = "ddlFeatureId";
            this.ddlFeatureId.Size = new System.Drawing.Size(279, 21);
            this.ddlFeatureId.TabIndex = 117;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 116;
            this.label3.Text = "Project Features";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 13);
            this.label2.TabIndex = 115;
            this.label2.Text = "Activation Dependency";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 114;
            this.label1.Text = "External Feature ID";
            // 
            // txtFeatureId
            // 
            this.txtFeatureId.Location = new System.Drawing.Point(125, 21);
            this.txtFeatureId.Name = "txtFeatureId";
            this.txtFeatureId.Size = new System.Drawing.Size(279, 20);
            this.txtFeatureId.TabIndex = 113;
            this.txtFeatureId.Validating += new System.ComponentModel.CancelEventHandler(this.txtTitle_Validating);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbInternal);
            this.panel1.Controls.Add(this.rbExternal);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtFeatureId);
            this.panel1.Controls.Add(this.ddlFeatureId);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(431, 83);
            this.panel1.TabIndex = 119;
            // 
            // rbInternal
            // 
            this.rbInternal.AutoSize = true;
            this.rbInternal.Location = new System.Drawing.Point(5, 50);
            this.rbInternal.Name = "rbInternal";
            this.rbInternal.Size = new System.Drawing.Size(14, 13);
            this.rbInternal.TabIndex = 119;
            this.rbInternal.TabStop = true;
            this.rbInternal.UseVisualStyleBackColor = true;
            // 
            // rbExternal
            // 
            this.rbExternal.AutoSize = true;
            this.rbExternal.Location = new System.Drawing.Point(5, 24);
            this.rbExternal.Name = "rbExternal";
            this.rbExternal.Size = new System.Drawing.Size(14, 13);
            this.rbExternal.TabIndex = 118;
            this.rbExternal.TabStop = true;
            this.rbExternal.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider1.ContainerControl = this;
            // 
            // ActivationDependencyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Name = "ActivationDependencyControl";
            this.Size = new System.Drawing.Size(434, 86);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ddlFeatureId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFeatureId;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbInternal;
        private System.Windows.Forms.RadioButton rbExternal;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}
