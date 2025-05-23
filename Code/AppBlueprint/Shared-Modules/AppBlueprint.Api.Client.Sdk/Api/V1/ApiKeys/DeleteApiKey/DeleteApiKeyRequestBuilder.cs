// <auto-generated/>
#pragma warning disable CS0618
using AppBlueprint.Api.Client.Sdk.Api.V1.ApiKeys.DeleteApiKey.Item;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace AppBlueprint.Api.Client.Sdk.Api.V1.ApiKeys.DeleteApiKey
{
    /// <summary>
    /// Builds and executes requests for operations under \api\v1\api-keys\DeleteApiKey
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class DeleteApiKeyRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the AppBlueprint.Api.Client.Sdk.api.v1.apiKeys.DeleteApiKey.item collection</summary>
        /// <param name="position">API key ID.</param>
        /// <returns>A <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.ApiKeys.DeleteApiKey.Item.DeleteApiKeyItemRequestBuilder"/></returns>
        public global::AppBlueprint.Api.Client.Sdk.Api.V1.ApiKeys.DeleteApiKey.Item.DeleteApiKeyItemRequestBuilder this[int position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("id", position);
                return new global::AppBlueprint.Api.Client.Sdk.Api.V1.ApiKeys.DeleteApiKey.Item.DeleteApiKeyItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>Gets an item from the AppBlueprint.Api.Client.Sdk.api.v1.apiKeys.DeleteApiKey.item collection</summary>
        /// <param name="position">API key ID.</param>
        /// <returns>A <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.ApiKeys.DeleteApiKey.Item.DeleteApiKeyItemRequestBuilder"/></returns>
        [Obsolete("This indexer is deprecated and will be removed in the next major version. Use the one with the typed parameter instead.")]
        public global::AppBlueprint.Api.Client.Sdk.Api.V1.ApiKeys.DeleteApiKey.Item.DeleteApiKeyItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                if (!string.IsNullOrWhiteSpace(position)) urlTplParams.Add("id", position);
                return new global::AppBlueprint.Api.Client.Sdk.Api.V1.ApiKeys.DeleteApiKey.Item.DeleteApiKeyItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.ApiKeys.DeleteApiKey.DeleteApiKeyRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DeleteApiKeyRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/v1/api-keys/DeleteApiKey", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.ApiKeys.DeleteApiKey.DeleteApiKeyRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DeleteApiKeyRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/v1/api-keys/DeleteApiKey", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
