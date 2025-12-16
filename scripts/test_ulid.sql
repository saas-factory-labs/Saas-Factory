-- Test ULID insertion with correct table structures
INSERT INTO "Customers" ("Id", "CustomerType", "CurrentlyAtOnboardingFlowStep", "CreatedAt", "IsSoftDeleted") 
VALUES ('customer_01JGQ1GQ8G9NRQH3X7WVFV2K3X', 1, 1, NOW(), false);

INSERT INTO "Tenants" ("Id", "Name", "Description", "IsActive", "IsPrimary", "Email", "Phone", "Type", "VatNumber", "Country", "CustomerId", "CreatedAt", "IsSoftDeleted") 
VALUES ('tenant_01JGQ1GQ8G9NRQH3X7WVFV2K3Y', 'Test Tenant', 'Test Description', true, true, 'tenant@example.com', '123-456-7890', 'Business', 'VAT123', 'USA', 'customer_01JGQ1GQ8G9NRQH3X7WVFV2K3X', NOW(), false);

INSERT INTO "Users" ("Id", "FirstName", "LastName", "UserName", "IsActive", "Email", "LastLogin", "TenantId", "CreatedAt", "IsSoftDeleted") 
VALUES ('user_01JGQ1GQ8G9NRQH3X7WVFV2K3Z', 'Test', 'User', 'testuser', true, 'test@example.com', NOW(), 'tenant_01JGQ1GQ8G9NRQH3X7WVFV2K3Y', NOW(), false);
