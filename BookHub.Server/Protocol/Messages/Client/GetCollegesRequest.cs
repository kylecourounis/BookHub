namespace BookHub.Server.Protocol.Messages.Client
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;
    
    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic;
    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Network;
    using BookHub.Server.Protocol.Messages.Server;

    internal class GetCollegesRequest : Message
    {
        private string SearchQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCollegesRequest"/> class.
        /// </summary>
        public GetCollegesRequest(JObject json) : base(json)
        {
            // GetCollegesRequest.
        }
        
        internal override void Handle()
        {
            new GetCollegesResponse(this.User, this.Context).Send();
        }
    }
}