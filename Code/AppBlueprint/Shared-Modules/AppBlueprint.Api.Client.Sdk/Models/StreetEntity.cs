// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace AppBlueprint.Api.Client.Sdk.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class StreetEntity : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The city property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::AppBlueprint.Api.Client.Sdk.Models.CityEntity? City { get; set; }
#nullable restore
#else
        public global::AppBlueprint.Api.Client.Sdk.Models.CityEntity City { get; set; }
#endif
        /// <summary>The cityId property</summary>
        public int? CityId { get; set; }
        /// <summary>The country property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::AppBlueprint.Api.Client.Sdk.Models.CityEntity? Country { get; set; }
#nullable restore
#else
        public global::AppBlueprint.Api.Client.Sdk.Models.CityEntity Country { get; set; }
#endif
        /// <summary>The countryId property</summary>
        public int? CountryId { get; set; }
        /// <summary>The id property</summary>
        public int? Id { get; set; }
        /// <summary>The name property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Name { get; set; }
#nullable restore
#else
        public string Name { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::AppBlueprint.Api.Client.Sdk.Models.StreetEntity"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::AppBlueprint.Api.Client.Sdk.Models.StreetEntity CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::AppBlueprint.Api.Client.Sdk.Models.StreetEntity();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "city", n => { City = n.GetObjectValue<global::AppBlueprint.Api.Client.Sdk.Models.CityEntity>(global::AppBlueprint.Api.Client.Sdk.Models.CityEntity.CreateFromDiscriminatorValue); } },
                { "cityId", n => { CityId = n.GetIntValue(); } },
                { "country", n => { Country = n.GetObjectValue<global::AppBlueprint.Api.Client.Sdk.Models.CityEntity>(global::AppBlueprint.Api.Client.Sdk.Models.CityEntity.CreateFromDiscriminatorValue); } },
                { "countryId", n => { CountryId = n.GetIntValue(); } },
                { "id", n => { Id = n.GetIntValue(); } },
                { "name", n => { Name = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<global::AppBlueprint.Api.Client.Sdk.Models.CityEntity>("city", City);
            writer.WriteIntValue("cityId", CityId);
            writer.WriteObjectValue<global::AppBlueprint.Api.Client.Sdk.Models.CityEntity>("country", Country);
            writer.WriteIntValue("countryId", CountryId);
            writer.WriteIntValue("id", Id);
            writer.WriteStringValue("name", Name);
        }
    }
}
#pragma warning restore CS0618
