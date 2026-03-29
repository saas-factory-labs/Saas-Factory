using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace AppBlueprint.Api.Client.Sdk.Models
{
    #pragma warning disable CS1591
    public partial class SearchRequestDto : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The queryText property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? QueryText { get; set; }
#nullable restore
#else
        public string QueryText { get; set; }
#endif
        /// <summary>The pageSize property</summary>
        public int? PageSize { get; set; }
        /// <summary>The pageNumber property</summary>
        public int? PageNumber { get; set; }
        /// <summary>The minRelevanceScore property</summary>
        public float? MinRelevanceScore { get; set; }
        /// <summary>The sortBy property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? SortBy { get; set; }
#nullable restore
#else
        public string SortBy { get; set; }
#endif
        /// <summary>The sortDirection property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? SortDirection { get; set; }
#nullable restore
#else
        public string SortDirection { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::AppBlueprint.Api.Client.Sdk.Models.SearchRequestDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::AppBlueprint.Api.Client.Sdk.Models.SearchRequestDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::AppBlueprint.Api.Client.Sdk.Models.SearchRequestDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "queryText", n => { QueryText = n.GetStringValue(); } },
                { "pageSize", n => { PageSize = n.GetIntValue(); } },
                { "pageNumber", n => { PageNumber = n.GetIntValue(); } },
                { "minRelevanceScore", n => { MinRelevanceScore = n.GetFloatValue(); } },
                { "sortBy", n => { SortBy = n.GetStringValue(); } },
                { "sortDirection", n => { SortDirection = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("queryText", QueryText);
            writer.WriteIntValue("pageSize", PageSize);
            writer.WriteIntValue("pageNumber", PageNumber);
            writer.WriteFloatValue("minRelevanceScore", MinRelevanceScore);
            writer.WriteStringValue("sortBy", SortBy);
            writer.WriteStringValue("sortDirection", SortDirection);
        }
    }
}
