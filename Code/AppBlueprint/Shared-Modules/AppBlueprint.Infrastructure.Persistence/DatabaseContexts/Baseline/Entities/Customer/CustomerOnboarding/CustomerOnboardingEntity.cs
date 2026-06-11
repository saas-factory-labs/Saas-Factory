namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.CustomerOnboarding;

public class CustomerOnboardingEntity
{
    public CustomerOnboardingEntity()
    {
        CustomerOnboarding = new CustomerOnboardingEntity();
    }

    public int Id { get; set; }

    public CustomerOnboardingEntity CustomerOnboarding { get; set; }


    // current onboarding step could also be in the user model


    // https://cruip.com/demos/mosaic/ - onboarding flow => onboarding pane

    // Track which step the user is on in the onboarding process and what they have completed on each step in the customer portal

    // step 1: user persona (Tell us what’s your situation ✨) - company, freelancer, individual, etc
    // step 2: indivial or company
    // step 3: company details (Tell us about your company 🏢) - company name, company type, company size, etc
    // step 4: onboarding done


    // flow of onboarding
    // 1. create user
    // 2. create tenant
    // 3. create role
    // 4. create permission
    // 5. create user role
    // 6. create address
    // 7. create email
    // 8. create phone number
    // 9. create contact person
    // 10. create customer
}
