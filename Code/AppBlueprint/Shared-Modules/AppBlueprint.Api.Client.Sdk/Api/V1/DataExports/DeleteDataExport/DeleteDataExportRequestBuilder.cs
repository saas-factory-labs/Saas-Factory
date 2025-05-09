// <auto-generated/>
#pragma warning disable CS0618
using AppBlueprint.Api.Client.Sdk.Api.V1.DataExports.DeleteDataExport.Item;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace AppBlueprint.Api.Client.Sdk.Api.V1.DataExports.DeleteDataExport
{
    /// <summary>
    /// Builds and executes requests for operations under \api\v1\data-exports\DeleteDataExport
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class DeleteDataExportRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the AppBlueprint.Api.Client.Sdk.api.v1.dataExports.DeleteDataExport.item collection</summary>
        /// <param name="position">Data export ID.</param>
        /// <returns>A <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.DataExports.DeleteDataExport.Item.DeleteDataExportItemRequestBuilder"/></returns>
        public global::AppBlueprint.Api.Client.Sdk.Api.V1.DataExports.DeleteDataExport.Item.DeleteDataExportItemRequestBuilder this[int position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("id", position);
                return new global::AppBlueprint.Api.Client.Sdk.Api.V1.DataExports.DeleteDataExport.Item.DeleteDataExportItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>Gets an item from the AppBlueprint.Api.Client.Sdk.api.v1.dataExports.DeleteDataExport.item collection</summary>
        /// <param name="position">Data export ID.</param>
        /// <returns>A <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.DataExports.DeleteDataExport.Item.DeleteDataExportItemRequestBuilder"/></returns>
        [Obsolete("This indexer is deprecated and will be removed in the next major version. Use the one with the typed parameter instead.")]
        public global::AppBlueprint.Api.Client.Sdk.Api.V1.DataExports.DeleteDataExport.Item.DeleteDataExportItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                if (!string.IsNullOrWhiteSpace(position)) urlTplParams.Add("id", position);
                return new global::AppBlueprint.Api.Client.Sdk.Api.V1.DataExports.DeleteDataExport.Item.DeleteDataExportItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.DataExports.DeleteDataExport.DeleteDataExportRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DeleteDataExportRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/v1/data-exports/DeleteDataExport", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::AppBlueprint.Api.Client.Sdk.Api.V1.DataExports.DeleteDataExport.DeleteDataExportRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DeleteDataExportRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/v1/data-exports/DeleteDataExport", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
