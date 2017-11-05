namespace CodeCommander
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.web = new System.Windows.Forms.WebBrowser();
            this.button1 = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnGenerate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyBtn = new System.Windows.Forms.ToolStripButton();
            this.cutBtn = new System.Windows.Forms.ToolStripButton();
            this.pasteBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnRedo = new System.Windows.Forms.ToolStripButton();
            this.btnAplus = new System.Windows.Forms.ToolStripButton();
            this.btnAmoins = new System.Windows.Forms.ToolStripButton();
            this.timView = new System.Windows.Forms.Timer(this.components);
            this.fichierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nouveauToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ouvrirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sauvegarderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImporterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.générerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.annulerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rétablirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.copierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.couperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.languesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rechercherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ajouterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modèlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.aProposDeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.didactitielVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.skeletonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ouvrirSkelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.syntaxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OuvrirSyntaxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.ofs = new System.Windows.Forms.OpenFileDialog();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.RightToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.webBrowser1);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.web);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.button1);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            this.toolStripContainer1.RightToolStripPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.TabStop = false;
            // 
            // webBrowser1
            // 
            this.webBrowser1.AllowNavigation = false;
            resources.ApplyResources(this.webBrowser1, "webBrowser1");
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.TabStop = false;
            this.webBrowser1.WebBrowserShortcutsEnabled = false;
            // 
            // web
            // 
            resources.ApplyResources(this.web, "web");
            this.web.IsWebBrowserContextMenuEnabled = false;
            this.web.Name = "web";
            this.web.TabStop = false;
            this.web.Url = new System.Uri("", System.UriKind.Relative);
            this.web.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.TabStop = false;
            this.button1.UseMnemonic = false;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpen,
            this.btnSave,
            this.btnGenerate,
            this.toolStripSeparator1,
            this.copyBtn,
            this.cutBtn,
            this.pasteBtn,
            this.toolStripSeparator2,
            this.btnUndo,
            this.btnRedo,
            this.btnAplus,
            this.btnAmoins});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnNew, "btnNew");
            this.btnNew.Name = "btnNew";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnOpen, "btnOpen");
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnGenerate, "btnGenerate");
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // copyBtn
            // 
            this.copyBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.copyBtn, "copyBtn");
            this.copyBtn.Name = "copyBtn";
            this.copyBtn.Click += new System.EventHandler(this.copyBtn_Click);
            // 
            // cutBtn
            // 
            this.cutBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.cutBtn, "cutBtn");
            this.cutBtn.Name = "cutBtn";
            this.cutBtn.Click += new System.EventHandler(this.cutBtn_Click);
            // 
            // pasteBtn
            // 
            this.pasteBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.pasteBtn, "pasteBtn");
            this.pasteBtn.Name = "pasteBtn";
            this.pasteBtn.Click += new System.EventHandler(this.pasteBtn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // btnUndo
            // 
            this.btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnUndo, "btnUndo");
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnRedo, "btnRedo");
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // btnAplus
            // 
            this.btnAplus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnAplus, "btnAplus");
            this.btnAplus.Name = "btnAplus";
            this.btnAplus.Click += new System.EventHandler(this.btnAplus_Click);
            // 
            // btnAmoins
            // 
            this.btnAmoins.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnAmoins, "btnAmoins");
            this.btnAmoins.Name = "btnAmoins";
            this.btnAmoins.Click += new System.EventHandler(this.btnAmoins_Click);
            // 
            // timView
            // 
            this.timView.Tick += new System.EventHandler(this.timView_Tick);
            // 
            // fichierToolStripMenuItem
            // 
            this.fichierToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nouveauToolStripMenuItem,
            this.ouvrirToolStripMenuItem,
            this.sauvegarderToolStripMenuItem,
            this.ImporterToolStripMenuItem,
            this.générerToolStripMenuItem,
            this.quitterToolStripMenuItem});
            this.fichierToolStripMenuItem.Name = "fichierToolStripMenuItem";
            resources.ApplyResources(this.fichierToolStripMenuItem, "fichierToolStripMenuItem");
            // 
            // nouveauToolStripMenuItem
            // 
            this.nouveauToolStripMenuItem.Name = "nouveauToolStripMenuItem";
            resources.ApplyResources(this.nouveauToolStripMenuItem, "nouveauToolStripMenuItem");
            this.nouveauToolStripMenuItem.Click += new System.EventHandler(this.nouveauToolStripMenuItem_Click);
            // 
            // ouvrirToolStripMenuItem
            // 
            this.ouvrirToolStripMenuItem.Name = "ouvrirToolStripMenuItem";
            resources.ApplyResources(this.ouvrirToolStripMenuItem, "ouvrirToolStripMenuItem");
            this.ouvrirToolStripMenuItem.Click += new System.EventHandler(this.ouvrirToolStripMenuItem_Click);
            // 
            // sauvegarderToolStripMenuItem
            // 
            resources.ApplyResources(this.sauvegarderToolStripMenuItem, "sauvegarderToolStripMenuItem");
            this.sauvegarderToolStripMenuItem.Name = "sauvegarderToolStripMenuItem";
            this.sauvegarderToolStripMenuItem.Click += new System.EventHandler(this.sauvegarderToolStripMenuItem_Click);
            // 
            // ImporterToolStripMenuItem
            // 
            this.ImporterToolStripMenuItem.Name = "ImporterToolStripMenuItem";
            resources.ApplyResources(this.ImporterToolStripMenuItem, "ImporterToolStripMenuItem");
            this.ImporterToolStripMenuItem.Click += new System.EventHandler(this.ImporterToolStripMenuItem_Click);
            // 
            // générerToolStripMenuItem
            // 
            resources.ApplyResources(this.générerToolStripMenuItem, "générerToolStripMenuItem");
            this.générerToolStripMenuItem.Name = "générerToolStripMenuItem";
            this.générerToolStripMenuItem.Click += new System.EventHandler(this.générerToolStripMenuItem_Click);
            // 
            // quitterToolStripMenuItem
            // 
            this.quitterToolStripMenuItem.Name = "quitterToolStripMenuItem";
            resources.ApplyResources(this.quitterToolStripMenuItem, "quitterToolStripMenuItem");
            this.quitterToolStripMenuItem.Click += new System.EventHandler(this.quitterToolStripMenuItem_Click);
            // 
            // editerToolStripMenuItem
            // 
            this.editerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.annulerToolStripMenuItem,
            this.rétablirToolStripMenuItem,
            this.toolStripSeparator3,
            this.copierToolStripMenuItem,
            this.couperToolStripMenuItem,
            this.collerToolStripMenuItem});
            this.editerToolStripMenuItem.Name = "editerToolStripMenuItem";
            resources.ApplyResources(this.editerToolStripMenuItem, "editerToolStripMenuItem");
            // 
            // annulerToolStripMenuItem
            // 
            this.annulerToolStripMenuItem.Name = "annulerToolStripMenuItem";
            resources.ApplyResources(this.annulerToolStripMenuItem, "annulerToolStripMenuItem");
            this.annulerToolStripMenuItem.Click += new System.EventHandler(this.annulerToolStripMenuItem_Click);
            // 
            // rétablirToolStripMenuItem
            // 
            this.rétablirToolStripMenuItem.Name = "rétablirToolStripMenuItem";
            resources.ApplyResources(this.rétablirToolStripMenuItem, "rétablirToolStripMenuItem");
            this.rétablirToolStripMenuItem.Click += new System.EventHandler(this.rétablirToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // copierToolStripMenuItem
            // 
            this.copierToolStripMenuItem.Name = "copierToolStripMenuItem";
            resources.ApplyResources(this.copierToolStripMenuItem, "copierToolStripMenuItem");
            this.copierToolStripMenuItem.Click += new System.EventHandler(this.copierToolStripMenuItem_Click);
            // 
            // couperToolStripMenuItem
            // 
            this.couperToolStripMenuItem.Name = "couperToolStripMenuItem";
            resources.ApplyResources(this.couperToolStripMenuItem, "couperToolStripMenuItem");
            this.couperToolStripMenuItem.Click += new System.EventHandler(this.couperToolStripMenuItem_Click);
            // 
            // collerToolStripMenuItem
            // 
            this.collerToolStripMenuItem.Name = "collerToolStripMenuItem";
            resources.ApplyResources(this.collerToolStripMenuItem, "collerToolStripMenuItem");
            this.collerToolStripMenuItem.Click += new System.EventHandler(this.collerToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languesToolStripMenuItem1,
            this.rechercherToolStripMenuItem,
            this.ajouterToolStripMenuItem});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // languesToolStripMenuItem1
            // 
            this.languesToolStripMenuItem1.Name = "languesToolStripMenuItem1";
            resources.ApplyResources(this.languesToolStripMenuItem1, "languesToolStripMenuItem1");
            this.languesToolStripMenuItem1.Click += new System.EventHandler(this.languesToolStripMenuItem1_Click);
            // 
            // rechercherToolStripMenuItem
            // 
            this.rechercherToolStripMenuItem.Name = "rechercherToolStripMenuItem";
            resources.ApplyResources(this.rechercherToolStripMenuItem, "rechercherToolStripMenuItem");
            this.rechercherToolStripMenuItem.Click += new System.EventHandler(this.rechercherToolStripMenuItem_Click);
            // 
            // ajouterToolStripMenuItem
            // 
            this.ajouterToolStripMenuItem.Name = "ajouterToolStripMenuItem";
            resources.ApplyResources(this.ajouterToolStripMenuItem, "ajouterToolStripMenuItem");
            this.ajouterToolStripMenuItem.Click += new System.EventHandler(this.ajouterToolStripMenuItem_Click);
            // 
            // modèlesToolStripMenuItem
            // 
            this.modèlesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripSeparator4});
            this.modèlesToolStripMenuItem.Name = "modèlesToolStripMenuItem";
            resources.ApplyResources(this.modèlesToolStripMenuItem, "modèlesToolStripMenuItem");
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aProposDeToolStripMenuItem,
            this.didactitielVideoToolStripMenuItem,
            this.projetsToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // aProposDeToolStripMenuItem
            // 
            this.aProposDeToolStripMenuItem.Name = "aProposDeToolStripMenuItem";
            resources.ApplyResources(this.aProposDeToolStripMenuItem, "aProposDeToolStripMenuItem");
            this.aProposDeToolStripMenuItem.Click += new System.EventHandler(this.aProposDeToolStripMenuItem_Click);
            // 
            // didactitielVideoToolStripMenuItem
            // 
            this.didactitielVideoToolStripMenuItem.Name = "didactitielVideoToolStripMenuItem";
            resources.ApplyResources(this.didactitielVideoToolStripMenuItem, "didactitielVideoToolStripMenuItem");
            this.didactitielVideoToolStripMenuItem.Click += new System.EventHandler(this.didactitielVideoToolStripMenuItem_Click);
            // 
            // projetsToolStripMenuItem
            // 
            this.projetsToolStripMenuItem.Name = "projetsToolStripMenuItem";
            resources.ApplyResources(this.projetsToolStripMenuItem, "projetsToolStripMenuItem");
            this.projetsToolStripMenuItem.Click += new System.EventHandler(this.projetsToolStripMenuItem_Click);
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fichierToolStripMenuItem,
            this.editerToolStripMenuItem,
            this.modèlesToolStripMenuItem,
            this.skeletonsToolStripMenuItem,
            this.syntaxesToolStripMenuItem,
            this.toolStripMenuItem3,
            this.toolStripMenuItem2});
            resources.ApplyResources(this.menu, "menu");
            this.menu.Name = "menu";
            // 
            // skeletonsToolStripMenuItem
            // 
            this.skeletonsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ouvrirSkelToolStripMenuItem,
            this.toolStripSeparator5});
            this.skeletonsToolStripMenuItem.Name = "skeletonsToolStripMenuItem";
            resources.ApplyResources(this.skeletonsToolStripMenuItem, "skeletonsToolStripMenuItem");
            // 
            // ouvrirSkelToolStripMenuItem
            // 
            this.ouvrirSkelToolStripMenuItem.Name = "ouvrirSkelToolStripMenuItem";
            resources.ApplyResources(this.ouvrirSkelToolStripMenuItem, "ouvrirSkelToolStripMenuItem");
            this.ouvrirSkelToolStripMenuItem.Click += new System.EventHandler(this.ouvrirSkelToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // syntaxesToolStripMenuItem
            // 
            this.syntaxesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OuvrirSyntaxesToolStripMenuItem,
            this.toolStripSeparator6});
            this.syntaxesToolStripMenuItem.Name = "syntaxesToolStripMenuItem";
            resources.ApplyResources(this.syntaxesToolStripMenuItem, "syntaxesToolStripMenuItem");
            // 
            // OuvrirSyntaxesToolStripMenuItem
            // 
            this.OuvrirSyntaxesToolStripMenuItem.Name = "OuvrirSyntaxesToolStripMenuItem";
            resources.ApplyResources(this.OuvrirSyntaxesToolStripMenuItem, "OuvrirSyntaxesToolStripMenuItem");
            this.OuvrirSyntaxesToolStripMenuItem.Click += new System.EventHandler(this.OuvrirSyntaxesToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.menu);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menu;
            this.Name = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.RightToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.RightToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser web;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton copyBtn;
        private System.Windows.Forms.ToolStripButton cutBtn;
        private System.Windows.Forms.ToolStripButton pasteBtn;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripButton btnRedo;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnGenerate;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Timer timView;
        private System.Windows.Forms.ToolStripMenuItem fichierToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nouveauToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ouvrirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sauvegarderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem générerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem annulerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rétablirToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem copierToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem couperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem modèlesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem aProposDeToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem languesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem rechercherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ajouterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem didactitielVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skeletonsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ouvrirSkelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem syntaxesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OuvrirSyntaxesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem ImporterToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog ofs;
        private System.Windows.Forms.ToolStripMenuItem projetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnAplus;
        private System.Windows.Forms.ToolStripButton btnAmoins;
    }
}

