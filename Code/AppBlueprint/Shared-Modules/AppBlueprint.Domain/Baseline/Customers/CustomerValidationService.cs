namespace AppBlueprint.Domain.Baseline.Customers;

public static class CustomerValidationService
{
    public static Task<bool> ValidateCustomerDataAsync(object customerData)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task<bool> IsEmailUniqueAsync(string email, Guid? excludeCustomerId = null)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber, Guid? excludeCustomerId = null)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task<IEnumerable<string>> ValidateCustomerBusinessRulesAsync(object customerData)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
}
