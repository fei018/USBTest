﻿
namespace USBNotifyDesktop
{
    partial class USBNotifyDesktopForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(USBNotifyDesktopForm));
            this.UsbNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripUsb = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.updateUsbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripUsb.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsbNotifyIcon
            // 
            this.UsbNotifyIcon.ContextMenuStrip = this.contextMenuStripUsb;
            this.UsbNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("UsbNotifyIcon.Icon")));
            this.UsbNotifyIcon.Text = "Usb Notify";
            this.UsbNotifyIcon.Visible = true;
            // 
            // contextMenuStripUsb
            // 
            this.contextMenuStripUsb.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateUsbToolStripMenuItem,
            this.toolStripMenuItem2});
            this.contextMenuStripUsb.Name = "contextMenuStripUsb";
            this.contextMenuStripUsb.Size = new System.Drawing.Size(143, 48);
            // 
            // updateUsbToolStripMenuItem
            // 
            this.updateUsbToolStripMenuItem.Name = "updateUsbToolStripMenuItem";
            this.updateUsbToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.updateUsbToolStripMenuItem.Text = "Update Usb";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(142, 22);
            this.toolStripMenuItem2.Text = "?";
            // 
            // USBNotifyDesktopForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 82);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "USBNotifyDesktopForm";
            this.Text = "USBNotify Desktop";
            this.contextMenuStripUsb.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon UsbNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripUsb;
        private System.Windows.Forms.ToolStripMenuItem updateUsbToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    }
}

