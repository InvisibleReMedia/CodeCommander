namespace CodeCommander
{
    partial class VideosHelp
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideosHelp));
            this.videoWeb = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // videoWeb
            // 
            this.videoWeb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoWeb.Location = new System.Drawing.Point(0, 0);
            this.videoWeb.MinimumSize = new System.Drawing.Size(20, 20);
            this.videoWeb.Name = "videoWeb";
            this.videoWeb.Size = new System.Drawing.Size(559, 409);
            this.videoWeb.TabIndex = 0;
            // 
            // VideosHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 409);
            this.Controls.Add(this.videoWeb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VideosHelp";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Aide";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.VideosHelp_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser videoWeb;
    }
}