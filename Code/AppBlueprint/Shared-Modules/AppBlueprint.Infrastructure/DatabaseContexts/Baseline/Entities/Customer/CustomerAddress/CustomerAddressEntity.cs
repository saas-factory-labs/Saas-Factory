using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using AppBlueprint.Infrastructure.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.CustomerAddress;

public class CustomerAddressEntity
{
    public CustomerAddressEntity()
    {
        Customer = new CustomerEntity();
        var addressEntity = new AddressEntity
        {
            Floor = "2",
            StreetNumber = "2"
        };
    }

    public int Id { get; set; }
    public CustomerEntity Customer { get; set; }
    public AddressEntity? Address { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsSecondary { get; set; } = false;

    public CustomerAddressType CustomerAddressTypeEnum { get; set; }
}
