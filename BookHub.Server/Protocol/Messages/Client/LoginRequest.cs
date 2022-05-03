namespace BookHub.Server.Protocol.Messages.Client
{
    using Newtonsoft.Json.Linq;
    
    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Network;
    using BookHub.Server.Protocol.Messages.Server;

    internal class LoginRequest : Message
    {
        private long Identifier;
        private string Email;
        private string Password;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginRequest"/> class.
        /// </summary>
        public LoginRequest(JObject json) : base(json)
        {
            // LoginRequest.
        }

        internal override void Decode()
        {
            this.Identifier = this.JSON.GetLong("id");
            this.Email      = this.JSON.GetString("email").ToLower();
            this.Password   = this.JSON.GetString("password");
        }

        internal override void Handle()
        {
            int error;

            this.User = Users.Get(this.Email);

            if (this.User != null)
            {
                if (this.User.Email.Equals(this.Email) && this.User.Password.Equals(this.Password))
                {
                    error = 0; // Success
                }
                else
                {
                    error = 2; // Invalid login
                }
            }
            else
            {
                error = 1; // Does Not Exist
                this.User = Users.Create(this.Email, this.Password);
            }

            new LoginResponse(this.User, this.Context)
            {
                Error = error
            }.Send();
        }
    }
}
