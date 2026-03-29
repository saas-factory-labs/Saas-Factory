using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace AppBlueprint.Api.Client.Sdk.Models
{
    #pragma warning disable CS1591
    public partial class TenantSearchItemDto : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The id property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Id { get; set; }
#nullable restore
#else
        public string Id { get; set; }
#endif
        /// <summary>The name property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Name { get; set; }
#nullable restore
#else
        public string Name { get; set; }
#endif
        /// <summary>The description property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Description { get; set; }
#nullable restore
#else
        public string Description { get; set; }
#endif
        /// <summary>The relevanceScore property</summary>
        public float? RelevanceScore { get; set; }
        /// <summary>The headline property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Headline { get; set; }
#nullable restore
#else
        public string Headline { get; set; }
#endif
        /// <summary>The matchedTerms property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<string>? MatchedTerms { get; set; }
#nullable restore
#else
        public List<string> MatchedTerms { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchItemDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchItemDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::AppBlueprint.Api.Client.Sdk.Models.TenantSearchItemDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "id", n => { Id = n.GetStringValue(); } },
                { "name", n => { Name = n.GetStringValue(); } },
                { "description", n => { Description = n.GetStringValue(); } },
                { "relevanceScore", n => { RelevanceScore = n.GetFloatValue(); } },
                { "headline", n => { Headline = n.GetStringValue(); } },
                { "matchedTerms", n => { MatchedTerms = n.GetCollectionOfPrimitiveValues<string>()?.AsList(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("id", Id);
            writer.WriteStringValue("name", Name);
            writer.WriteStringValue("description", Description);
            writer.WriteFloatValue("relevanceScore", RelevanceScore);
            writer.WriteStringValue("headline", Headline);
            writer.WriteCollectionOfPrimitiveValues("matchedTerms", MatchedTerms);
        }
    }
}
