using AppBlueprint.Api.Client.Sdk.Models;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace AppBlueprint.Api.Client.Sdk.Api.V1.Search.Tenants
{
    /// <summary>
    /// Builds and executes requests for operations under \api\v1\search\tenants
    /// </summary>
    public partial class TenantsRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.Search.Tenants.TenantsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public TenantsRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/v1/search/tenants", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.Search.Tenants.TenantsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public TenantsRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/v1/search/tenants", rawUrl)
        {
        }
        /// <summary>
        /// Performs a full-text search across tenants.
        /// </summary>
        /// <returns>A <see cref="global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchResponseDto"/></returns>
        /// <param name="body">The search request body</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::AppBlueprint.Api.Client.Sdk.Models.ProblemDetails">When receiving a 400 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchResponseDto?> PostAsync(global::AppBlueprint.Api.Client.Sdk.Models.SearchRequestDto body, Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchResponseDto> PostAsync(global::AppBlueprint.Api.Client.Sdk.Models.SearchRequestDto body, Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = ToPostRequestInformation(body, requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "400", global::AppBlueprint.Api.Client.Sdk.Models.ProblemDetails.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchResponseDto>(requestInfo, global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchResponseDto.CreateFromDiscriminatorValue, errorMapping, cancellationToken);
        }
        /// <summary>
        /// Builds the POST request information for tenant search.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="body">The search request body</param>
        /// <param name="requestConfiguration">Configuration for the request</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToPostRequestInformation(global::AppBlueprint.Api.Client.Sdk.Models.SearchRequestDto body, Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToPostRequestInformation(global::AppBlueprint.Api.Client.Sdk.Models.SearchRequestDto body, Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default)
        {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = new RequestInformation(Method.POST, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            requestInfo.SetContentFromParsable(RequestAdapter, "application/json", body);
            return requestInfo;
        }
    }
}
