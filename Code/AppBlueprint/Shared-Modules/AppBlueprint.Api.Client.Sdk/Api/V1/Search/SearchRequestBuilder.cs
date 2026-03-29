using AppBlueprint.Api.Client.Sdk.Models;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace AppBlueprint.Api.Client.Sdk.Api.V1.Search
{
    /// <summary>
    /// Builds and executes requests for operations under \api\v1\search
    /// </summary>
    public partial class SearchRequestBuilder : BaseRequestBuilder
    {
        /// <summary>The tenants search endpoint</summary>
        public global::AppBlueprint.Api.Client.Sdk.Api.V1.Search.Tenants.TenantsRequestBuilder Tenants
        {
            get => new global::AppBlueprint.Api.Client.Sdk.Api.V1.Search.Tenants.TenantsRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The users search endpoint</summary>
        public global::AppBlueprint.Api.Client.Sdk.Api.V1.Search.Users.UsersRequestBuilder Users
        {
            get => new global::AppBlueprint.Api.Client.Sdk.Api.V1.Search.Users.UsersRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.Search.SearchRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public SearchRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/v1/search", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.Search.SearchRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public SearchRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/v1/search", rawUrl)
        {
        }
    }
}
