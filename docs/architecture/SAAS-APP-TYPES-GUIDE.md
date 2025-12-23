# SaaS Application Types - Architecture Guide

**Last Updated:** December 23, 2025  
**Architecture Version:** Baseline Tenant Model with B2C/B2B Support

## Overview

This document explains how different types of SaaS applications work with our unified tenant architecture. The architecture supports both B2C (Business-to-Consumer) and B2B (Business-to-Business) models using a single `TenantEntity` with a type discriminator.

**Architecture Reference:** [B2C-B2B-TENANT-REFACTORING.md](./B2C-B2B-TENANT-REFACTORING.md)

---

## Table of Contents

1. [B2C Applications](#b2c-applications)
   - [Dating Apps](#1-dating-apps)
   - [Property Rental Platforms](#2-property-rental-platforms)
   - [Social Media Platforms](#3-social-media-platforms)
   - [Personal Finance Apps](#4-personal-finance-apps)
   - [Media Streaming Services](#5-media-streaming-services)
2. [B2B Applications](#b2b-applications)
   - [CRM Systems](#1-crm-systems)
   - [Project Management Tools](#2-project-management-tools)
   - [HR Management Systems](#3-hr-management-systems)
   - [Accounting Software](#4-accounting-software)
   - [Team Communication Platforms](#5-team-communication-platforms)
3. [Hybrid Applications](#hybrid-applications)
4. [Architecture Patterns](#architecture-patterns)

---

## B2C Applications

### 1. Dating Apps

**Examples:** Tinder, Bumble, Match.com

#### Tenant Model
- **Type:** `TenantType.Personal`
- **Users per Tenant:** 1 (individual profile)
- **Factory Method:** `TenantFactory.CreatePersonalTenant(user)`

#### Implementation

```csharp
// User Registration
public async Task<TenantEntity> RegisterNewUserAsync(UserRegistrationDto dto)
{
    // Create user
    var user = new UserEntity
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        UserName = dto.Username
    };
    
    // Auto-create Personal tenant for user's profile
    var profileTenant = TenantFactory.CreatePersonalTenant(user);
    profileTenant.AddUser(user);
    
    await _dbContext.Tenants.AddAsync(profileTenant);
    await _dbContext.Users.AddAsync(user);
    await _dbContext.SaveChangesAsync();
    
    return profileTenant;
}

// User Profile Data (scoped to their tenant)
public class UserProfile : ITenantScoped
{
    public string TenantId { get; set; }  // Their Personal tenant
    public string Bio { get; set; }
    public List<string> Photos { get; set; }
    public int Age { get; set; }
    public string Location { get; set; }
    public List<string> Interests { get; set; }
}

// Matches happen ACROSS tenants
public class Match
{
    public string User1TenantId { get; set; }  // First user's tenant
    public string User2TenantId { get; set; }  // Second user's tenant
    public DateTime MatchedAt { get; set; }
}
```

#### Database Structure

```sql
-- Each user gets their own Personal tenant
Tenants:
  Id: "tenant_01ABCD..."
  TenantType: 0 (Personal)
  Name: "Sarah Johnson"
  Email: "sarah@example.com"
  VatNumber: NULL
  Country: NULL

-- User profiles are tenant-scoped
UserProfiles:
  TenantId: "tenant_01ABCD..."  -- Sarah's tenant
  Bio: "Love hiking and coffee..."
  Age: 28
  
-- Matches connect ACROSS tenants
Matches:
  User1TenantId: "tenant_01ABCD..."  -- Sarah
  User2TenantId: "tenant_01WXYZ..."  -- John
  MatchedAt: 2025-12-23
```

#### Key Features
- ✅ **Data Isolation:** Each user's profile data isolated by tenant
- ✅ **Privacy:** RLS policies prevent cross-tenant data leaks
- ✅ **Scalability:** Millions of Personal tenants in shared database
- ✅ **Premium Tier:** VIP users could get dedicated database
- ✅ **Cross-Tenant Queries:** Matching algorithm queries across tenants

#### Deployment Model
- **Free/Basic Users:** Shared database (10M+ Personal tenants)
- **Premium Users:** Priority in elastic pool
- **VIP Users:** Optional dedicated database

---

### 2. Property Rental Platforms

**Examples:** Airbnb, Booking.com, Zillow Rentals

#### Tenant Model
- **Type:** Mixed - `TenantType.Personal` + `TenantType.Organization`
- **Individual Users:** Personal tenants (landlords, renters)
- **Property Management Companies:** Organization tenants

#### Implementation

```csharp
// Individual Landlord Registration
public async Task<TenantEntity> RegisterLandlordAsync(UserEntity landlord)
{
    // Personal tenant for individual landlord
    var landlordTenant = TenantFactory.CreatePersonalTenant(landlord);
    landlordTenant.AddUser(landlord);
    
    return landlordTenant;
}

// Property Management Company Registration
public async Task<TenantEntity> RegisterPMCAsync(OrganizationDto dto)
{
    // Organization tenant for PMC
    var pmcTenant = TenantFactory.CreateOrganizationTenant(
        organizationName: dto.CompanyName,
        organizationEmail: dto.Email,
        vatNumber: dto.VatNumber,
        country: dto.Country
    );
    
    // Add company employees as users
    foreach (var employee in dto.Employees)
    {
        var user = new UserEntity { ... };
        pmcTenant.AddUser(user);
    }
    
    return pmcTenant;
}

// Property owned by tenant (Personal or Organization)
public class Property : ITenantScoped
{
    public string TenantId { get; set; }  // Owner's tenant
    public string Address { get; set; }
    public decimal PricePerNight { get; set; }
    public int Bedrooms { get; set; }
    public List<string> Photos { get; set; }
}

// Bookings connect tenants (renter + property owner)
public class Booking
{
    public string PropertyId { get; set; }
    public string PropertyOwnerTenantId { get; set; }  // Landlord/PMC tenant
    public string RenterTenantId { get; set; }          // Renter's tenant
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}
```

#### Database Structure

```sql
-- Individual landlord
Tenants:
  Id: "tenant_landlord_123"
  TenantType: 0 (Personal)
  Name: "John Smith"
  VatNumber: NULL

-- Property management company
Tenants:
  Id: "tenant_pmc_456"
  TenantType: 1 (Organization)
  Name: "City Properties LLC"
  VatNumber: "US123456789"
  Country: "USA"

-- Properties belong to owners
Properties:
  Id: "prop_789"
  TenantId: "tenant_landlord_123"  -- John's property
  Address: "123 Main St"

Properties:
  Id: "prop_890"
  TenantId: "tenant_pmc_456"  -- PMC's property
  Address: "456 Oak Ave"

-- Bookings cross tenants
Bookings:
  PropertyOwnerTenantId: "tenant_landlord_123"
  RenterTenantId: "tenant_renter_999"
  CheckIn: 2025-12-25
```

#### Key Features
- ✅ **Flexible Model:** Supports individuals AND companies
- ✅ **B2B Features:** PMCs get teams, reporting, bulk operations
- ✅ **Cross-Tenant:** Bookings connect different tenant types
- ✅ **Scalability:** Shared DB for individuals, dedicated for large PMCs

#### Deployment Model
- **Individual Landlords/Renters:** Shared database
- **Small PMCs (<100 properties):** Shared database
- **Large PMCs (>100 properties):** Dedicated database
- **Enterprise PMCs:** Dedicated database + compute

---

### 3. Social Media Platforms

**Examples:** Instagram, Twitter/X, TikTok

#### Tenant Model
- **Type:** `TenantType.Personal`
- **Users per Tenant:** 1 (user account)
- **Special:** Relationships exist ACROSS tenants (followers, likes, etc.)

#### Implementation

```csharp
// User account = Personal tenant
var userTenant = TenantFactory.CreatePersonalTenant(user);

// User's content is tenant-scoped
public class Post : ITenantScoped
{
    public string TenantId { get; set; }  // Author's tenant
    public string Content { get; set; }
    public List<string> MediaUrls { get; set; }
    public DateTime PostedAt { get; set; }
}

// Social connections are CROSS-TENANT
public class Follower
{
    public string FollowerTenantId { get; set; }  // Person following
    public string FollowedTenantId { get; set; }  // Person being followed
    public DateTime FollowedAt { get; set; }
}

public class Like
{
    public string PostId { get; set; }
    public string LikerTenantId { get; set; }  // Who liked it
    public string PostOwnerTenantId { get; set; }  // Post author
}
```

#### Key Features
- ✅ **User Isolation:** Each user's data in their Personal tenant
- ✅ **Social Graph:** Cross-tenant relationships (followers, likes)
- ✅ **Feed Generation:** Query across multiple tenants
- ✅ **Privacy Controls:** RLS for private content

---

### 4. Personal Finance Apps

**Examples:** Mint, YNAB, Personal Capital

#### Tenant Model
- **Type:** `TenantType.Personal` (or Family)
- **Users per Tenant:** 1 user (or 2-4 for family accounts)

#### Implementation

```csharp
// Individual user
var personalTenant = TenantFactory.CreatePersonalTenant(user);

// Family account
var familyTenant = TenantFactory.CreateFamilyTenant(
    familyName: "Johnson Family",
    primaryUserEmail: "dad@johnson.com"
);
familyTenant.AddUser(dad);
familyTenant.AddUser(mom);

// Financial data is tenant-scoped
public class BankAccount : ITenantScoped
{
    public string TenantId { get; set; }
    public string AccountName { get; set; }
    public decimal Balance { get; set; }
}

public class Transaction : ITenantScoped
{
    public string TenantId { get; set; }
    public string AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; }
}
```

#### Key Features
- ✅ **Privacy:** Financial data isolated per tenant
- ✅ **Family Sharing:** Multiple users can access same tenant
- ✅ **No Cross-Tenant:** No queries across users (privacy)

---

### 5. Media Streaming Services

**Examples:** Spotify, Netflix, YouTube Premium

#### Tenant Model
- **Type:** `TenantType.Personal` (individual or family plan)
- **Individual Plan:** 1 user per tenant
- **Family Plan:** 2-6 users per tenant

#### Implementation

```csharp
// Individual subscription
var individualTenant = TenantFactory.CreatePersonalTenant(user);

// Family subscription
var familyTenant = TenantFactory.CreateFamilyTenant(
    familyName: "Smith Family Plan",
    primaryUserEmail: "parent@smith.com"
);
// Can add up to 6 family members
familyTenant.AddUser(parent);
familyTenant.AddUser(child1);
familyTenant.AddUser(child2);

// Subscription tied to tenant
public class Subscription : ITenantScoped
{
    public string TenantId { get; set; }
    public SubscriptionTier Tier { get; set; }  // Individual or Family
    public int MaxUsers { get; set; }  // 1 for Individual, 6 for Family
    public DateTime ExpiresAt { get; set; }
}

// Playback history per user within tenant
public class PlaybackHistory
{
    public string TenantId { get; set; }
    public string UserId { get; set; }  // Specific family member
    public string ContentId { get; set; }
    public int ProgressSeconds { get; set; }
}
```

#### Key Features
- ✅ **Flexible Plans:** Individual = 1 user, Family = multiple users
- ✅ **Shared Subscription:** One payment for whole tenant
- ✅ **Personal Preferences:** Each user has own data within tenant

---

## B2B Applications

### 1. CRM Systems

**Examples:** Salesforce, HubSpot, Pipedrive

#### Tenant Model
- **Type:** `TenantType.Organization`
- **Users per Tenant:** 5-500+ (sales teams)
- **Factory Method:** `TenantFactory.CreateOrganizationTenant(...)`

#### Implementation

```csharp
// Client company registration
public async Task<TenantEntity> RegisterClientCompanyAsync(CompanyDto dto)
{
    var orgTenant = TenantFactory.CreateOrganizationTenant(
        organizationName: dto.CompanyName,
        organizationEmail: dto.Email,
        vatNumber: dto.VatNumber,
        country: dto.Country
    );
    
    // Add initial admin user
    var admin = new UserEntity
    {
        FirstName = dto.AdminFirstName,
        LastName = dto.AdminLastName,
        Email = dto.AdminEmail
    };
    orgTenant.AddUser(admin);
    
    // Add contact persons for billing
    var billingContact = new ContactPersonEntity
    {
        FirstName = dto.BillingContactName,
        Email = dto.BillingEmail,
        Role = "Billing Contact"
    };
    orgTenant.AddContactPerson(billingContact);
    
    return orgTenant;
}

// Teams within organization
public async Task CreateSalesTeamAsync(string tenantId)
{
    var salesTeam = new TeamEntity
    {
        Name = "Sales Team",
        TenantId = tenantId,
        Description = "West Coast Sales"
    };
    
    await _dbContext.Teams.AddAsync(salesTeam);
}

// CRM data scoped to organization
public class Contact : ITenantScoped
{
    public string TenantId { get; set; }  // Client company
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Company { get; set; }
    public string Email { get; set; }
}

public class Deal : ITenantScoped
{
    public string TenantId { get; set; }
    public string ContactId { get; set; }
    public decimal Amount { get; set; }
    public string Stage { get; set; }  // "Prospecting", "Negotiation", "Closed"
    public string AssignedToUserId { get; set; }
}
```

#### Database Structure

```sql
-- Client company tenant
Tenants:
  Id: "tenant_acme_corp"
  TenantType: 1 (Organization)
  Name: "Acme Corporation"
  VatNumber: "DE123456789"
  Country: "Germany"
  Email: "admin@acme.com"

-- Users (employees)
Users:
  Id: "user_john"
  TenantId: "tenant_acme_corp"
  FirstName: "John"
  Role: "Sales Manager"

Users:
  Id: "user_sarah"
  TenantId: "tenant_acme_corp"
  FirstName: "Sarah"
  Role: "Sales Rep"

-- Teams
Teams:
  Id: "team_sales"
  TenantId: "tenant_acme_corp"
  Name: "Sales Team"

TeamMembers:
  TeamId: "team_sales"
  UserId: "user_john"
  UserId: "user_sarah"

-- CRM data (contacts, deals, etc.)
Contacts:
  TenantId: "tenant_acme_corp"
  Name: "Potential Customer"

Deals:
  TenantId: "tenant_acme_corp"
  ContactId: "contact_123"
  AssignedTo: "user_sarah"
  Amount: 50000
```

#### Key Features
- ✅ **Multi-User:** 5-500+ users per organization
- ✅ **Teams:** Sales, Support, Management teams
- ✅ **Roles:** Admin, Manager, Sales Rep, Read-Only
- ✅ **Billing:** VATNumber for invoicing
- ✅ **Enterprise:** Large orgs get dedicated database

#### Deployment Model
- **Small Companies (<10 users):** Shared database
- **Medium Companies (10-100 users):** Elastic pool
- **Enterprise (>100 users):** Dedicated database
- **Global Enterprise:** Geographic sharding

---

### 2. Project Management Tools

**Examples:** Asana, Monday.com, Jira

#### Tenant Model
- **Type:** `TenantType.Organization`
- **Users per Tenant:** 10-1000+ (whole company)
- **Teams:** Multiple teams per organization

#### Implementation

```csharp
// Company workspace
var companyTenant = TenantFactory.CreateOrganizationTenant(
    organizationName: "TechStart Inc",
    organizationEmail: "team@techstart.io"
);

// Multiple teams
var engineeringTeam = new TeamEntity 
{ 
    Name = "Engineering", 
    TenantId = companyTenant.Id 
};

var designTeam = new TeamEntity 
{ 
    Name = "Design", 
    TenantId = companyTenant.Id 
};

// Projects scoped to organization
public class Project : ITenantScoped
{
    public string TenantId { get; set; }
    public string Name { get; set; }
    public string TeamId { get; set; }  // Which team owns it
    public DateTime StartDate { get; set; }
    public DateTime? DueDate { get; set; }
}

// Tasks within projects
public class Task : ITenantScoped
{
    public string TenantId { get; set; }
    public string ProjectId { get; set; }
    public string Title { get; set; }
    public string AssignedToUserId { get; set; }
    public string Status { get; set; }  // "To Do", "In Progress", "Done"
}
```

#### Key Features
- ✅ **Workspace:** All company projects in one tenant
- ✅ **Cross-Team:** Projects can span multiple teams
- ✅ **Permissions:** Team-based access control
- ✅ **Scalability:** Supports large organizations

---

### 3. HR Management Systems

**Examples:** BambooHR, Workday, ADP

#### Tenant Model
- **Type:** `TenantType.Organization`
- **One Company = One Tenant**
- **All Employees = Users within tenant**

#### Implementation

```csharp
// Company HR tenant
var companyTenant = TenantFactory.CreateOrganizationTenant(
    organizationName: "Global Corp",
    organizationEmail: "hr@globalcorp.com",
    vatNumber: "US987654321",
    country: "USA"
);

// Employee data scoped to company
public class Employee : ITenantScoped
{
    public string TenantId { get; set; }
    public string UserId { get; set; }
    public string JobTitle { get; set; }
    public string Department { get; set; }
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
}

// Sensitive data (extra RLS)
public class PayrollRecord : ITenantScoped
{
    public string TenantId { get; set; }
    public string EmployeeId { get; set; }
    public decimal GrossPay { get; set; }
    public DateTime PayDate { get; set; }
}
```

#### Key Features
- ✅ **Confidentiality:** All employee data tenant-isolated
- ✅ **Compliance:** GDPR, SOC2 via RLS policies
- ✅ **No Cross-Tenant:** Companies never see each other's data

---

### 4. Accounting Software

**Examples:** QuickBooks Online, Xero, FreshBooks

#### Tenant Model
- **Type:** `TenantType.Organization`
- **One Business = One Tenant**
- **Accountant Access:** Multi-tenant users (accountant can access multiple client tenants)

#### Implementation

```csharp
// Business tenant
var businessTenant = TenantFactory.CreateOrganizationTenant(
    organizationName: "Coffee Shop LLC",
    organizationEmail: "owner@coffeeshop.com",
    vatNumber: "US123456789"
);

// Financial data tenant-scoped
public class Invoice : ITenantScoped
{
    public string TenantId { get; set; }
    public string InvoiceNumber { get; set; }
    public string CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
}

// Accountant can access multiple tenants
public class AccountantAccess
{
    public string AccountantUserId { get; set; }
    public List<string> AccessibleTenantIds { get; set; }  // Multiple clients
}
```

#### Key Features
- ✅ **Financial Isolation:** Critical for accounting
- ✅ **Multi-Tenant Users:** Accountants access multiple clients
- ✅ **Audit Trail:** Track changes per tenant
- ✅ **Tax Compliance:** VATNumber for reporting

---

### 5. Team Communication Platforms

**Examples:** Slack, Microsoft Teams, Discord

#### Tenant Model
- **Type:** `TenantType.Organization` (or Personal for communities)
- **Workspace = Organization Tenant**
- **Channels = Teams within tenant**

#### Implementation

```csharp
// Company workspace
var workspaceTenant = TenantFactory.CreateOrganizationTenant(
    organizationName: "StartupCo Workspace",
    organizationEmail: "workspace@startupco.com"
);

// Channels = Teams
var generalChannel = new TeamEntity 
{ 
    Name = "general", 
    TenantId = workspaceTenant.Id 
};

var engineeringChannel = new TeamEntity 
{ 
    Name = "engineering", 
    TenantId = workspaceTenant.Id 
};

// Messages scoped to tenant
public class Message : ITenantScoped
{
    public string TenantId { get; set; }
    public string ChannelId { get; set; }  // TeamId
    public string UserId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
}
```

#### Key Features
- ✅ **Workspace Isolation:** Each company separate
- ✅ **Channels:** Teams represent channels
- ✅ **Real-Time:** All in same tenant for low latency
- ✅ **Guest Access:** External users can be invited

---

## Hybrid Applications

Some applications need BOTH B2C and B2B models:

### Example: Freelance Marketplace (Upwork, Fiverr)

```csharp
// Individual freelancer
var freelancerTenant = TenantFactory.CreatePersonalTenant(freelancerUser);
// TenantType = Personal

// Freelance agency
var agencyTenant = TenantFactory.CreateOrganizationTenant(
    "Creative Agency LLC",
    "team@agency.com"
);
// TenantType = Organization
// Multiple freelancers as users

// Job postings cross tenants
public class Job
{
    public string ClientTenantId { get; set; }  // Company posting job
    public string FreelancerTenantId { get; set; }  // Freelancer assigned
}
```

---

## Architecture Patterns

### Pattern Summary

| App Type | Tenant Type | Users/Tenant | Teams | Cross-Tenant | Deployment |
|----------|-------------|--------------|-------|--------------|------------|
| Dating | Personal | 1 | No | Yes (matches) | Shared DB |
| Rental (Individual) | Personal | 1 | No | Yes (bookings) | Shared DB |
| Rental (PMC) | Organization | 5-50 | Yes | Yes (bookings) | Elastic Pool |
| Social Media | Personal | 1 | No | Yes (social) | Shared DB |
| Finance | Personal | 1-4 | No | No | Shared DB |
| Streaming | Personal | 1-6 | No | No | Shared DB |
| CRM | Organization | 5-500+ | Yes | No | Elastic Pool → Dedicated |
| Project Mgmt | Organization | 10-1000+ | Yes | No | Elastic Pool → Dedicated |
| HR System | Organization | 50-10000+ | Yes | No | Dedicated DB |
| Accounting | Organization | 1-50 | Optional | No | Shared → Dedicated |
| Team Chat | Organization | 10-1000+ | Yes | No | Elastic Pool |

### Decision Matrix

**Choose Personal Tenant When:**
- ✅ Individual users
- ✅ User profile is main entity
- ✅ 1:1 user-to-account ratio
- ✅ Simple account structure
- ✅ Consumer-facing app

**Choose Organization Tenant When:**
- ✅ Business customers
- ✅ Multiple users collaborate
- ✅ Need teams/departments
- ✅ Require VAT invoicing
- ✅ Enterprise features needed

### Cross-Tenant Relationships

Some apps need relationships ACROSS tenants:
- **Dating:** Matches between Personal tenants
- **Rental:** Bookings between landlord and renter tenants
- **Social Media:** Follows, likes across Personal tenants
- **Marketplace:** Jobs connecting client and freelancer tenants

Implementation:
```csharp
// Cross-tenant entity (no ITenantScoped)
public class Match
{
    public string User1TenantId { get; set; }
    public string User2TenantId { get; set; }
    // NOT tenant-scoped - connects two tenants
}
```

---

## Conclusion

The unified tenant architecture with `TenantType` discriminator supports:

✅ **B2C Apps** - Personal tenants for individual users  
✅ **B2B Apps** - Organization tenants for companies  
✅ **Hybrid Apps** - Mix of Personal and Organization tenants  
✅ **Cross-Tenant** - Relationships between different tenants  
✅ **Scalable** - Shared DB → Elastic Pool → Dedicated DB  

This matches **Microsoft's recommended patterns** and is used by successful SaaS companies worldwide.

---

## Further Reading

- [Microsoft Multi-Tenancy Guide](https://learn.microsoft.com/en-us/azure/architecture/guide/multitenant/)
- [Database Tenancy Patterns](https://learn.microsoft.com/en-us/azure/azure-sql/database/saas-tenancy-app-design-patterns)
- [B2C/B2B Refactoring ADR](./B2C-B2B-TENANT-REFACTORING.md)
