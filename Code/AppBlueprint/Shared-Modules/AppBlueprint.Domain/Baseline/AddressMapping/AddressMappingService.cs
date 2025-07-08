namespace AppBlueprint.Domain.Baseline.AddressMapping;

public sealed class AddressMappingService
{
    private AddressMappingService() { }
    
    
    public static Task<(double Latitude, double Longitude)> GeocodeAddressAsync(string address)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
    
    public static Task<string> ReverseGeocodeAsync(double latitude, double longitude)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
    
    public static Task<bool> ValidateAddressAsync(string address)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
    
    public static Task<string> NormalizeAddressAsync(string address)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
}
