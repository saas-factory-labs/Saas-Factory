// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace AppBlueprint.Presentation.Ui.Sdk.Client.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class ContactPersonEntity : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The addresses property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.AddressEntity>? Addresses { get; set; }
#nullable restore
#else
        public List<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.AddressEntity> Addresses { get; set; }
#endif
        /// <summary>The customer property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.CustomerEntity? Customer { get; set; }
#nullable restore
#else
        public global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.CustomerEntity Customer { get; set; }
#endif
        /// <summary>The emailAddresses property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.EmailAddressEntity>? EmailAddresses { get; set; }
#nullable restore
#else
        public List<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.EmailAddressEntity> EmailAddresses { get; set; }
#endif
        /// <summary>The firstName property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? FirstName { get; set; }
#nullable restore
#else
        public string FirstName { get; set; }
#endif
        /// <summary>The id property</summary>
        public int? Id { get; set; }
        /// <summary>The isActive property</summary>
        public bool? IsActive { get; set; }
        /// <summary>The isPrimary property</summary>
        public bool? IsPrimary { get; set; }
        /// <summary>The lastName property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? LastName { get; set; }
#nullable restore
#else
        public string LastName { get; set; }
#endif
        /// <summary>The phoneNumbers property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.PhoneNumberEntity>? PhoneNumbers { get; set; }
#nullable restore
#else
        public List<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.PhoneNumberEntity> PhoneNumbers { get; set; }
#endif
        /// <summary>The tenant property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.TenantEntity? Tenant { get; set; }
#nullable restore
#else
        public global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.TenantEntity Tenant { get; set; }
#endif
        /// <summary>The tenantId property</summary>
        public int? TenantId { get; set; }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.ContactPersonEntity"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.ContactPersonEntity CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.ContactPersonEntity();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "addresses", n => { Addresses = n.GetCollectionOfObjectValues<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.AddressEntity>(global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.AddressEntity.CreateFromDiscriminatorValue)?.AsList(); } },
                { "customer", n => { Customer = n.GetObjectValue<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.CustomerEntity>(global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.CustomerEntity.CreateFromDiscriminatorValue); } },
                { "emailAddresses", n => { EmailAddresses = n.GetCollectionOfObjectValues<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.EmailAddressEntity>(global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.EmailAddressEntity.CreateFromDiscriminatorValue)?.AsList(); } },
                { "firstName", n => { FirstName = n.GetStringValue(); } },
                { "id", n => { Id = n.GetIntValue(); } },
                { "isActive", n => { IsActive = n.GetBoolValue(); } },
                { "isPrimary", n => { IsPrimary = n.GetBoolValue(); } },
                { "lastName", n => { LastName = n.GetStringValue(); } },
                { "phoneNumbers", n => { PhoneNumbers = n.GetCollectionOfObjectValues<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.PhoneNumberEntity>(global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.PhoneNumberEntity.CreateFromDiscriminatorValue)?.AsList(); } },
                { "tenant", n => { Tenant = n.GetObjectValue<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.TenantEntity>(global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.TenantEntity.CreateFromDiscriminatorValue); } },
                { "tenantId", n => { TenantId = n.GetIntValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteCollectionOfObjectValues<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.AddressEntity>("addresses", Addresses);
            writer.WriteObjectValue<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.CustomerEntity>("customer", Customer);
            writer.WriteCollectionOfObjectValues<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.EmailAddressEntity>("emailAddresses", EmailAddresses);
            writer.WriteStringValue("firstName", FirstName);
            writer.WriteIntValue("id", Id);
            writer.WriteBoolValue("isActive", IsActive);
            writer.WriteBoolValue("isPrimary", IsPrimary);
            writer.WriteStringValue("lastName", LastName);
            writer.WriteCollectionOfObjectValues<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.PhoneNumberEntity>("phoneNumbers", PhoneNumbers);
            writer.WriteObjectValue<global::AppBlueprint.Presentation.Ui.Sdk.Client.Models.TenantEntity>("tenant", Tenant);
            writer.WriteIntValue("tenantId", TenantId);
        }
    }
}
#pragma warning restore CS0618
