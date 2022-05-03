namespace BookHub.Server.Protocol.Messages.Server
{
    using System.Net;

    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic;
    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Protocol.Enums;

    internal class GetCollegesResponse : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetCollegesResponse"/> class.
        /// </summary>
        public GetCollegesResponse(User user, HttpListenerContext context) : base(user, context)
        {
            this.Type = MessageType.GetCollegesResponse;
        }

        internal override void Encode()
        {
            Colleges.ForEach(college =>
            {
                this.Data.AddString(college.Identifier.ToString(), college.ToString());
            });
        }
    }
}
