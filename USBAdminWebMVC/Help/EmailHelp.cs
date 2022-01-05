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
        public async Task SendUsbRegisterRequestNotify(Tbl_UsbRegRequest usb)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Usb Detail:")
                .AppendLine(usb.ToString())
                .AppendLine("User Email: "+ usb.UserEmail)
                .AppendLine();

            // send to user
            await SendEmail(usb.UserEmail, "Usb Register Request Notify", sb.ToString());

            // send to it
            sb.AppendLine("Usb Register Request Url: " + _usbRegisterRequestUrl + "/" + usb.Id);
            await SendEmail(_from, "Usb Register Request Notify", sb.ToString());            
        }
        #endregion
    }
}
