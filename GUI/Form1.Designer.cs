namespace CodesysGUIBrowser
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lbInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_OnlineGetVars = new System.Windows.Forms.Button();
            this.btn_LoadXml = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tbUser = new System.Windows.Forms.TextBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbIpAddress = new System.Windows.Forms.TextBox();
            this.tbSymbol = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbSymbolicDatatype = new System.Windows.Forms.TextBox();
            this.btnRead = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbInfo
            // 
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(70, 17);
            this.lbInfo.Text = "PLC type：";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Location = new System.Drawing.Point(12, 140);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(776, 279);
            this.treeView1.TabIndex = 9;
            this.treeView1.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterExpand);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_OnlineGetVars);
            this.groupBox1.Controls.Add(this.btn_LoadXml);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.tbUser);
            this.groupBox1.Controls.Add(this.lbStatus);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnDisconnect);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbPassword);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbIpAddress);
            this.groupBox1.Location = new System.Drawing.Point(12, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 131);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // btn_OnlineGetVars
            // 
            this.btn_OnlineGetVars.Location = new System.Drawing.Point(165, 90);
            this.btn_OnlineGetVars.Name = "btn_OnlineGetVars";
            this.btn_OnlineGetVars.Size = new System.Drawing.Size(87, 21);
            this.btn_OnlineGetVars.TabIndex = 11;
            this.btn_OnlineGetVars.Text = "在线读取标签";
            this.btn_OnlineGetVars.UseVisualStyleBackColor = true;
            this.btn_OnlineGetVars.Click += new System.EventHandler(this.btn_OnlineGetVars_Click);
            // 
            // btn_LoadXml
            // 
            this.btn_LoadXml.Location = new System.Drawing.Point(254, 90);
            this.btn_LoadXml.Name = "btn_LoadXml";
            this.btn_LoadXml.Size = new System.Drawing.Size(58, 21);
            this.btn_LoadXml.TabIndex = 10;
            this.btn_LoadXml.Text = "加载XML";
            this.btn_LoadXml.UseVisualStyleBackColor = true;
            this.btn_LoadXml.Click += new System.EventHandler(this.btn_LoadXml_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 51);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 12);
            this.label7.TabIndex = 9;
            this.label7.Text = "User:";
            // 
            // tbUser
            // 
            this.tbUser.Location = new System.Drawing.Point(10, 66);
            this.tbUser.Name = "tbUser";
            this.tbUser.Size = new System.Drawing.Size(122, 21);
            this.tbUser.TabIndex = 8;
            // 
            // lbStatus
            // 
            this.lbStatus.Location = new System.Drawing.Point(44, 114);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(116, 15);
            this.lbStatus.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "Status:";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(87, 90);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 21);
            this.btnDisconnect.TabIndex = 5;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(9, 90);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 21);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(193, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password:";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(195, 66);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(117, 21);
            this.tbPassword.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP-Address:";
            // 
            // tbIpAddress
            // 
            this.tbIpAddress.Location = new System.Drawing.Point(9, 30);
            this.tbIpAddress.Name = "tbIpAddress";
            this.tbIpAddress.Size = new System.Drawing.Size(303, 21);
            this.tbIpAddress.TabIndex = 0;
            this.tbIpAddress.Text = "127.0.0.1";
            // 
            // tbSymbol
            // 
            this.tbSymbol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSymbol.Location = new System.Drawing.Point(6, 30);
            this.tbSymbol.Name = "tbSymbol";
            this.tbSymbol.ReadOnly = true;
            this.tbSymbol.Size = new System.Drawing.Size(357, 21);
            this.tbSymbol.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Symbol:";
            // 
            // tbValue
            // 
            this.tbValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbValue.Location = new System.Drawing.Point(6, 102);
            this.tbValue.Name = "tbValue";
            this.tbValue.ReadOnly = true;
            this.tbValue.Size = new System.Drawing.Size(438, 21);
            this.tbValue.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "Value:";
            // 
            // tbSymbolicDatatype
            // 
            this.tbSymbolicDatatype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSymbolicDatatype.Location = new System.Drawing.Point(6, 66);
            this.tbSymbolicDatatype.Name = "tbSymbolicDatatype";
            this.tbSymbolicDatatype.ReadOnly = true;
            this.tbSymbolicDatatype.Size = new System.Drawing.Size(438, 21);
            this.tbSymbolicDatatype.TabIndex = 8;
            // 
            // btnRead
            // 
            this.btnRead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRead.Location = new System.Drawing.Point(369, 29);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(75, 22);
            this.btnRead.TabIndex = 11;
            this.btnRead.Text = "Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "Symbolic data type:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnRead);
            this.groupBox2.Controls.Add(this.tbValue);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tbSymbolicDatatype);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbSymbol);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(338, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(450, 131);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripStatusLabel lbInfo;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbIpAddress;
        private System.Windows.Forms.TextBox tbSymbol;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbSymbolicDatatype;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbUser;
        private System.Windows.Forms.Button btn_LoadXml;
        private System.Windows.Forms.Button btn_OnlineGetVars;
    }
}

