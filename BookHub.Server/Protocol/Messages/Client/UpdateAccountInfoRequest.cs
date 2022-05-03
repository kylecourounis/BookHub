namespace BookHub.Server.Protocol.Messages.Client
{
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;

    using Newtonsoft.Json.Linq;
    
    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Network;
    using BookHub.Server.Protocol.Messages.Server;

    internal class UpdateAccountInfoRequest : Message
    {
        private string Email;
        private string Password;
        private string FullName;
        private long College;

        private bool FirstLogin;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAccountInfoRequest"/> class.
        /// </summary>
        public UpdateAccountInfoRequest(JObject json) : base(json)
        {
            // UpdateAccountInfoRequest.
        }

        internal override void Decode()
        {
            this.Email      = this.JSON.GetString("email");
            this.Password   = this.JSON.GetString("password");

            this.FullName   = this.JSON.GetString("name");
            this.College    = this.JSON.GetLong("college");

            this.FirstLogin = this.JSON.GetBoolean("firstLogin");
        }

        internal override void Handle()
        {
            if (this.User.Email.Equals(this.Email) && this.User.Password.Equals(this.Password))
            {
                if (!string.IsNullOrEmpty(this.FullName))
                {
                    this.User.Name = this.FullName;
                }

                if (this.College > 0)
                {
                    this.User.College = Colleges.Get(this.College);
                }

                if (this.FirstLogin)
                {
                    this.SendConfirmationEmail();
                }

                this.User.Save();
            }

            new UpdateAccountInfoResponse(this.User, this.Context).Send();
        }

        /// <summary>
        /// Sends the confirmation email to the new user.
        /// </summary>
        private void SendConfirmationEmail()
        {
            try
            {
                const string DISPLAY_NAME = "BookHub";

                const string FROM_ADDRESS = "";
                const string PASSWORD     = "";

                SmtpClient mailClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(FROM_ADDRESS, PASSWORD),
                    EnableSsl   = true
                };

                // The next three lines make it so that we can embed the logo but it won't show as an attachment
                AlternateView htmlView    = AlternateView.CreateAlternateViewFromString(this.EmailBody, Encoding.UTF8, "text/html");
                htmlView.TransferEncoding = TransferEncoding.QuotedPrintable;
                htmlView.LinkedResources.Add(new LinkedResource("Resources/logo.png", "image/png")
                {
                    ContentId = "logo"
                });

                mailClient.Send(new MailMessage
                {
                    From           = new MailAddress(FROM_ADDRESS, DISPLAY_NAME),
                    Sender         = new MailAddress(FROM_ADDRESS, DISPLAY_NAME),
                    To             = { new MailAddress(this.Email) },
                    Subject        = "Thank you for joining BookHub!",
                    Body           = this.EmailBody,
                    IsBodyHtml     = true,
                    AlternateViews = { htmlView },
                });
            }
            catch (SmtpException)
            {
                // Either the user signed up with an invalid email address or couldn't login to our email...
            }
        }

        /// <summary>
        /// Gets the email body HTML and replaces the parameters.
        /// </summary>
        private string EmailBody
        {
            get
            {
                StringBuilder contents = new StringBuilder(File.ReadAllText("Resources/confirmation-email.html"));

                contents.Replace("%NAME%", this.User.Name);
                contents.Replace("%COLLEGE%", this.User.College.Name);

                return contents.ToString();
            }
        }
    }
}
