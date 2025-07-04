namespace AppBlueprint.Contracts.Baseline.Address.Requests;

public class CreateAddressRequest
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}
