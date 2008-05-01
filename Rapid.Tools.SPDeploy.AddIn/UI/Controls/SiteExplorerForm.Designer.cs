namespace Rapid.Tools.SPDeploy.AddIn.UI.Controls
{
    partial class SiteExplorerForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SiteExplorerForm));
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.deploymentToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(0, 24);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(287, 390);
			this.treeView1.TabIndex = 0;
			this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
			this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
			// 
			// menuStrip1
			// 
			this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.deploymentToolStripMenu,
            this.openToolStripMenu});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(287, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.toolStripMenuItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem1.Image")));
			this.toolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(28, 20);
			this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
			// 
			// solutionToolStripMenuItem
			// 
			this.deploymentToolStripMenu.Image = ((System.Drawing.Image)(resources.GetObject("solutionToolStripMenuItem.Image")));
			this.deploymentToolStripMenu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.deploymentToolStripMenu.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.deploymentToolStripMenu.Name = "solutionToolStripMenuItem";
			this.deploymentToolStripMenu.Size = new System.Drawing.Size(96, 20);
			this.deploymentToolStripMenu.Text = "Deployment";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenu.Name = "openToolStripMenuItem";
			this.openToolStripMenu.Size = new System.Drawing.Size(60, 20);
			this.openToolStripMenu.Text = "Open ...";
			// 
			// SiteExplorerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.menuStrip1);
			this.Name = "SiteExplorerForm";
			this.Size = new System.Drawing.Size(287, 414);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem deploymentToolStripMenu;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenu;
    }
}
