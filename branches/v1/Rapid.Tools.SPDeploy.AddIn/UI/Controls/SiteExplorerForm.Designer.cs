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
            this._tree = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.refreshMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.deploymentMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tree
            // 
            this._tree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tree.Location = new System.Drawing.Point(0, 24);
            this._tree.Name = "_tree";
            this._tree.Size = new System.Drawing.Size(287, 390);
            this._tree.TabIndex = 0;
            this._tree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.NodeDoubleClick);
            this._tree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.NodeBeforeExpand);
            this._tree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.NodeClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshMenu,
            this.deploymentMenu,
            this.openMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(287, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // refreshMenu
            // 
            this.refreshMenu.BackColor = System.Drawing.SystemColors.ControlLight;
            this.refreshMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshMenu.Image = ((System.Drawing.Image)(resources.GetObject("refreshMenu.Image")));
            this.refreshMenu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.refreshMenu.Name = "refreshMenu";
            this.refreshMenu.Size = new System.Drawing.Size(28, 20);
            this.refreshMenu.Tag = "Refresh";
            this.refreshMenu.Visible = false;
            this.refreshMenu.Click += new System.EventHandler(this.RefreshMenuClick);
            // 
            // deploymentMenu
            // 
            this.deploymentMenu.Image = ((System.Drawing.Image)(resources.GetObject("deploymentMenu.Image")));
            this.deploymentMenu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.deploymentMenu.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.deploymentMenu.Name = "deploymentMenu";
            this.deploymentMenu.Size = new System.Drawing.Size(88, 20);
            this.deploymentMenu.Text = "Deployment";
            // 
            // openMenu
            // 
            this.openMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openMenu.Image = ((System.Drawing.Image)(resources.GetObject("openMenu.Image")));
            this.openMenu.Name = "openMenu";
            this.openMenu.Size = new System.Drawing.Size(28, 20);
            this.openMenu.Text = "Open ...";
            // 
            // SiteExplorerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tree);
            this.Controls.Add(this.menuStrip1);
            this.Name = "SiteExplorerForm";
            this.Size = new System.Drawing.Size(287, 414);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView _tree;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem refreshMenu;
		private System.Windows.Forms.ToolStripMenuItem deploymentMenu;
        private System.Windows.Forms.ToolStripMenuItem openMenu;
    }
}
