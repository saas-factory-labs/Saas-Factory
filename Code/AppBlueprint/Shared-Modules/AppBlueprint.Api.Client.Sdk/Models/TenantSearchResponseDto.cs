using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace AppBlueprint.Api.Client.Sdk.Models
{
    #pragma warning disable CS1591
    public partial class TenantSearchResponseDto : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The items property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchItemDto>? Items { get; set; }
#nullable restore
#else
        public List<global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchItemDto> Items { get; set; }
#endif
        /// <summary>The totalCount property</summary>
        public int? TotalCount { get; set; }
        /// <summary>The pageNumber property</summary>
        public int? PageNumber { get; set; }
        /// <summary>The pageSize property</summary>
        public int? PageSize { get; set; }
        /// <summary>The executionTimeMs property</summary>
        public long? ExecutionTimeMs { get; set; }
        /// <summary>The query property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Query { get; set; }
#nullable restore
#else
        public string Query { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchResponseDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchResponseDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchResponseDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "items", n => { Items = n.GetCollectionOfObjectValues<global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchItemDto>(global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchItemDto.CreateFromDiscriminatorValue)?.AsList(); } },
                { "totalCount", n => { TotalCount = n.GetIntValue(); } },
                { "pageNumber", n => { PageNumber = n.GetIntValue(); } },
                { "pageSize", n => { PageSize = n.GetIntValue(); } },
                { "executionTimeMs", n => { ExecutionTimeMs = n.GetLongValue(); } },
                { "query", n => { Query = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteCollectionOfObjectValues("items", Items);
            writer.WriteIntValue("totalCount", TotalCount);
            writer.WriteIntValue("pageNumber", PageNumber);
            writer.WriteIntValue("pageSize", PageSize);
            writer.WriteLongValue("executionTimeMs", ExecutionTimeMs);
            writer.WriteStringValue("query", Query);
        }
    }
}
