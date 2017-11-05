namespace CodeCommander
{
    partial class Save
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
            this.lvFiles = new System.Windows.Forms.ListView();
            this.colTitle = new System.Windows.Forms.ColumnHeader();
            this.colDateC = new System.Windows.Forms.ColumnHeader();
            this.colDateM = new System.Windows.Forms.ColumnHeader();
            this.colRev = new System.Windows.Forms.ColumnHeader();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSuppr = new System.Windows.Forms.Button();
            this.btnErase = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvFiles
            // 
            this.lvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFiles.BackColor = System.Drawing.Color.Snow;
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTitle,
            this.colDateC,
            this.colDateM,
            this.colRev});
            this.lvFiles.GridLines = true;
            this.lvFiles.Location = new System.Drawing.Point(12, 59);
            this.lvFiles.MultiSelect = false;
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(518, 201);
            this.lvFiles.TabIndex = 0;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.SelectedIndexChanged += new System.EventHandler(this.lvFiles_SelectedIndexChanged);
            // 
            // colTitle
            // 
            this.colTitle.Text = "Titre";
            this.colTitle.Width = 180;
            // 
            // colDateC
            // 
            this.colDateC.Text = "Date création";
            this.colDateC.Width = 120;
            // 
            // colDateM
            // 
            this.colDateM.Text = "Last modified";
            this.colDateM.Width = 120;
            // 
            // colRev
            // 
            this.colRev.Text = "Révision";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(455, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Annuler";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Actualiser";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSuppr
            // 
            this.btnSuppr.Enabled = false;
            this.btnSuppr.Location = new System.Drawing.Point(105, 12);
            this.btnSuppr.Name = "btnSuppr";
            this.btnSuppr.Size = new System.Drawing.Size(75, 23);
            this.btnSuppr.TabIndex = 3;
            this.btnSuppr.Text = "Supprimer";
            this.btnSuppr.UseVisualStyleBackColor = true;
            this.btnSuppr.Click += new System.EventHandler(this.btnSuppr_Click);
            // 
            // btnErase
            // 
            this.btnErase.Enabled = false;
            this.btnErase.Location = new System.Drawing.Point(346, 12);
            this.btnErase.Name = "btnErase";
            this.btnErase.Size = new System.Drawing.Size(91, 23);
            this.btnErase.TabIndex = 4;
            this.btnErase.Text = "Ecraser";
            this.btnErase.UseVisualStyleBackColor = true;
            this.btnErase.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(240, 12);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 5;
            this.btnNew.Text = "Nouveau";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // Save
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 272);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnErase);
            this.Controls.Add(this.btnSuppr);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lvFiles);
            this.Name = "Save";
            this.Text = "Sauvegarder";
            this.Load += new System.EventHandler(this.Open_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSuppr;
        private System.Windows.Forms.ColumnHeader colTitle;
        private System.Windows.Forms.ColumnHeader colDateC;
        private System.Windows.Forms.ColumnHeader colDateM;
        private System.Windows.Forms.ColumnHeader colRev;
        private System.Windows.Forms.Button btnErase;
        private System.Windows.Forms.Button btnNew;
    }
}