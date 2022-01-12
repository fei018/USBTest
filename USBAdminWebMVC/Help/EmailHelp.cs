using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBModel;

namespace USBAdminWebMVC
{
    public class EmailHelp
    {
        private readonly string _smtp;
        private readonly int _port;
        private readonly string _from;
        private readonly string _usbRegisterRequestUrl;

        public EmailHelp(IConfiguration configuration)
        {
            _smtp = configuration.GetSection("Email").GetSection("Smtp").Value;
            _port= Convert.ToInt32(configuration.GetSection("Email").GetSection("Port").Value);
            _from = configuration.GetSection("Email").GetSection("From").Value;
            _usbRegisterRequestUrl = configuration.GetSection("Email").GetSection("UsbRequestRegUrl").Value;
        }

        #region public async Task SendEmail(string subject, string content)
        public async Task SendEmail(string toAddress,string subject, string content)
        {
            try
            {
                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("USBAdmin", _from);
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress(toAddress, toAddress);
                message.To.Add(to);

                message.Subject = subject;
                BodyBuilder bodyBuilder = new BodyBuilder { TextBody = content };
                message.Body = bodyBuilder.ToMessageBody();

                using SmtpClient client = new SmtpClient();
                await client.ConnectAsync(_smtp, _port, false);
                //client.Authenticate("", "");

                client.Send(message);
                client.Disconnect(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task SendUsbRegisterRequestNotify(Tbl_UsbRegisterRequest usb, string userEmailAddress)
        public async Task Send_UsbRequest_Notify_Submit_ToUser(Tbl_UsbRequest usb, Tbl_PerComputer com)
        {
            try
            {
                var subject = "USB registration request notification";

                StringBuilder body = new StringBuilder();
                body.AppendLine("USB Detail:")
                    .AppendLine("Manufacturer: " + usb.Manufacturer)
                    .AppendLine("Product: " + usb.Product)
                    .AppendLine("SerialNumber: " + usb.SerialNumber)
                    .AppendLine("IP: " + com.IPAddress)
                    .AppendLine("ComputerName: " + com.HostName)
                    .AppendLine()
                    .AppendLine("Request reason:")
                    .AppendLine(usb.RequestReason)
                    .AppendLine();

                // send to user
                await SendEmail(usb.RequestUserEmail, subject, body.ToString());

                // send to it
                body.AppendLine("Approve or Reject Url: " + _usbRegisterRequestUrl + "/" + usb.Id);
                await SendEmail(_from, subject, body.ToString());
            }
            catch (Exception)
            {
                throw;
            }    
        }
        #endregion

        #region + public async Task Send_UsbReuqest_Notify_Result_ToUser(Tbl_UsbRequest usb)
        public async Task Send_UsbReuqest_Notify_Result_ToUser(Tbl_UsbRequest usb)
        {
            try
            {
                var subject = "USB registration request result";

                StringBuilder body = new StringBuilder();
                body.AppendLine("USB Detail:")
                    .AppendLine("Manufacturer: " + usb.Manufacturer)
                    .AppendLine("Product: " + usb.Product)
                    .AppendLine("SerialNumber: " + usb.SerialNumber)
                    .AppendLine()
                    .AppendLine("Request reason:")
                    .AppendLine(usb.RequestReason)
                    .AppendLine()
                    .AppendLine("-----------")
                    .AppendLine("Request result:");

                if (true)
                {
                    body.AppendLine("Your request is approved. This USB device can be used after 5 mins. You can also right click \"USB Control\" icon in system tray, select \"Update USB Whitelist\" to take immediate effect.");
                }

                // send to user
                await SendEmail(usb.RequestUserEmail, subject, body.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
