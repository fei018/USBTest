
namespace NotifyUSBFormDesktop
{
    partial class NUSBForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NUSBForm));
            this.notifyIconUsb = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.submitUsbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateInfomationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIconUsb
            // 
            this.notifyIconUsb.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIconUsb.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconUsb.Icon")));
            this.notifyIconUsb.Text = "Usb Monitor";
            this.notifyIconUsb.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.submitUsbToolStripMenuItem,
            this.updateInfomationToolStripMenuItem,
            this.toolStripMenuItem2});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(183, 70);
            // 
            // submitUsbToolStripMenuItem
            // 
            this.submitUsbToolStripMenuItem.Name = "submitUsbToolStripMenuItem";
            this.submitUsbToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.submitUsbToolStripMenuItem.Text = "Submit Usb";
            // 
            // updateInfomationToolStripMenuItem
            // 
            this.updateInfomationToolStripMenuItem.Name = "updateInfomationToolStripMenuItem";
            this.updateInfomationToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.updateInfomationToolStripMenuItem.Text = "Update Infomation";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(182, 22);
            this.toolStripMenuItem2.Text = "?";
            // 
            // NUSBForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 78);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NUSBForm";
            this.Text = "NUSB Desktop";
            this.Shown += new System.EventHandler(this.NUSBForm_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIconUsb;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem submitUsbToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateInfomationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    }
}

