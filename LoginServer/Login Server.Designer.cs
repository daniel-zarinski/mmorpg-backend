namespace LoginServer
{
    partial class frmLoginServer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLoginServer));
            this.lStatus = new System.Windows.Forms.Label();
            this.lClientsConnected = new System.Windows.Forms.Label();
            this.lbClients = new System.Windows.Forms.ListBox();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.bSendToSelected = new System.Windows.Forms.Button();
            this.bBroadcast = new System.Windows.Forms.Button();
            this.tLog = new System.Windows.Forms.Timer(this.components);
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.lbTime = new System.Windows.Forms.Label();
            this.lbCharacters = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.itemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skillsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nPCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(709, 450);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(40, 13);
            this.lStatus.TabIndex = 0;
            this.lStatus.Text = "Status:";
            // 
            // lClientsConnected
            // 
            this.lClientsConnected.AutoSize = true;
            this.lClientsConnected.Location = new System.Drawing.Point(13, 30);
            this.lClientsConnected.Name = "lClientsConnected";
            this.lClientsConnected.Size = new System.Drawing.Size(105, 13);
            this.lClientsConnected.TabIndex = 1;
            this.lClientsConnected.Text = "Clients Connected: 0";
            // 
            // lbClients
            // 
            this.lbClients.FormattingEnabled = true;
            this.lbClients.Location = new System.Drawing.Point(16, 47);
            this.lbClients.Name = "lbClients";
            this.lbClients.Size = new System.Drawing.Size(245, 199);
            this.lbClients.TabIndex = 2;
            // 
            // tbLog
            // 
            this.tbLog.BackColor = System.Drawing.SystemColors.Control;
            this.tbLog.ForeColor = System.Drawing.Color.Black;
            this.tbLog.Location = new System.Drawing.Point(267, 48);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.Size = new System.Drawing.Size(436, 111);
            this.tbLog.TabIndex = 3;
            this.tbLog.WordWrap = false;
            // 
            // tbInput
            // 
            this.tbInput.Location = new System.Drawing.Point(709, 47);
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(197, 20);
            this.tbInput.TabIndex = 4;
            // 
            // bSendToSelected
            // 
            this.bSendToSelected.Location = new System.Drawing.Point(709, 73);
            this.bSendToSelected.Name = "bSendToSelected";
            this.bSendToSelected.Size = new System.Drawing.Size(197, 23);
            this.bSendToSelected.TabIndex = 5;
            this.bSendToSelected.Text = "Kick";
            this.bSendToSelected.UseVisualStyleBackColor = true;
            this.bSendToSelected.Click += new System.EventHandler(this.bSendToSelected_Click);
            // 
            // bBroadcast
            // 
            this.bBroadcast.Location = new System.Drawing.Point(709, 102);
            this.bBroadcast.Name = "bBroadcast";
            this.bBroadcast.Size = new System.Drawing.Size(197, 23);
            this.bBroadcast.TabIndex = 6;
            this.bBroadcast.Text = "Broadcast";
            this.bBroadcast.UseVisualStyleBackColor = true;
            this.bBroadcast.Click += new System.EventHandler(this.bBroadcast_Click);
            // 
            // tLog
            // 
            this.tLog.Interval = 100000;
            this.tLog.Tick += new System.EventHandler(this.tLog_Tick);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(267, 48);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(436, 415);
            this.txtLog.TabIndex = 7;
            this.txtLog.Text = "";
            // 
            // lbTime
            // 
            this.lbTime.AutoSize = true;
            this.lbTime.Location = new System.Drawing.Point(803, 13);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(33, 13);
            this.lbTime.TabIndex = 8;
            this.lbTime.Text = "TIME";
            // 
            // lbCharacters
            // 
            this.lbCharacters.FormattingEnabled = true;
            this.lbCharacters.Location = new System.Drawing.Point(16, 264);
            this.lbCharacters.Name = "lbCharacters";
            this.lbCharacters.Size = new System.Drawing.Size(245, 199);
            this.lbCharacters.TabIndex = 9;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(918, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemsToolStripMenuItem,
            this.skillsToolStripMenuItem,
            this.nPCsToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(55, 20);
            this.toolStripMenuItem1.Text = "Import";
            // 
            // itemsToolStripMenuItem
            // 
            this.itemsToolStripMenuItem.Name = "itemsToolStripMenuItem";
            this.itemsToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.itemsToolStripMenuItem.Text = "Items";
            this.itemsToolStripMenuItem.Click += new System.EventHandler(this.itemsToolStripMenuItem_Click);
            // 
            // skillsToolStripMenuItem
            // 
            this.skillsToolStripMenuItem.Name = "skillsToolStripMenuItem";
            this.skillsToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.skillsToolStripMenuItem.Text = "Skills";
            this.skillsToolStripMenuItem.Click += new System.EventHandler(this.skillsToolStripMenuItem_Click);
            // 
            // nPCsToolStripMenuItem
            // 
            this.nPCsToolStripMenuItem.Name = "nPCsToolStripMenuItem";
            this.nPCsToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.nPCsToolStripMenuItem.Text = "NPCs";
            this.nPCsToolStripMenuItem.Click += new System.EventHandler(this.nPCsToolStripMenuItem_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(712, 414);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(194, 23);
            this.btnClearLog.TabIndex = 11;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // frmLoginServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 475);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.lbCharacters);
            this.Controls.Add(this.lbTime);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.bBroadcast);
            this.Controls.Add(this.bSendToSelected);
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.lbClients);
            this.Controls.Add(this.lClientsConnected);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "frmLoginServer";
            this.Text = "Login Server";
            this.Load += new System.EventHandler(this.frmLoginServer_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lStatus;
        private System.Windows.Forms.Label lClientsConnected;
        private System.Windows.Forms.ListBox lbClients;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.Button bSendToSelected;
        private System.Windows.Forms.Button bBroadcast;
        private System.Windows.Forms.Timer tLog;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.ListBox lbCharacters;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem itemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skillsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nPCsToolStripMenuItem;
        private System.Windows.Forms.Button btnClearLog;
    }
}

