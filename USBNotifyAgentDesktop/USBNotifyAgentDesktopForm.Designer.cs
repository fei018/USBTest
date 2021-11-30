
namespace USBNotifyAgentDesktop
{
    partial class USBNotifyAgentDesktopForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(USBNotifyAgentDesktopForm));
            this.UsbNotifyTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripUsb = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.NotifyItem_About = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripUsb.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsbNotifyTray
            // 
            this.UsbNotifyTray.ContextMenuStrip = this.contextMenuStripUsb;
            this.UsbNotifyTray.Icon = ((System.Drawing.Icon)(resources.GetObject("UsbNotifyTray.Icon")));
            this.UsbNotifyTray.Text = "Usb Notify";
            this.UsbNotifyTray.Visible = true;
            // 
            // contextMenuStripUsb
            // 
            this.contextMenuStripUsb.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripUsb.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NotifyItem_About});
            this.contextMenuStripUsb.Name = "contextMenuStripUsb";
            this.contextMenuStripUsb.Size = new System.Drawing.Size(110, 26);
            // 
            // NotifyItem_About
            // 
            this.NotifyItem_About.Name = "NotifyItem_About";
            this.NotifyItem_About.Size = new System.Drawing.Size(109, 22);
            this.NotifyItem_About.Text = "About";
            this.NotifyItem_About.Click += new System.EventHandler(this.NotifyItem_About_Click);
            // 
            // USBNotifyAgentDesktopForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 82);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "USBNotifyAgentDesktopForm";
            this.Text = "USBNotifyAgentD";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.USBNotifyAgentDesktopForm_FormClosed);
            this.Shown += new System.EventHandler(this.USBNotifyAgentDesktopForm_Shown);
            this.contextMenuStripUsb.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon UsbNotifyTray;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripUsb;
        private System.Windows.Forms.ToolStripMenuItem NotifyItem_About;
    }
}

