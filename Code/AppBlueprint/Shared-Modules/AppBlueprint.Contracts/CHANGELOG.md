# Changelog

All notable changes to AppBlueprint.Contracts will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Request and response DTOs for B2B operations
- User management contracts (CreateUserRequest, UpdateUserRequest, UserResponse)
- Team management contracts (CreateTeamRequest, TeamMemberResponse, InviteMemberRequest)
- Organization contracts (CreateOrganizationRequest, OrganizationResponse)
- Authentication contracts (LoginRequest, LoginResponse, TokenResponse, RefreshTokenRequest)
- Subscription contracts (SubscriptionResponse, CreateSubscriptionRequest, UpdatePlanRequest)
- Billing contracts (InvoiceResponse, PaymentMethodResponse)
- Pagination contracts (PaginatedRequest, PaginatedResponse)
- Error response contracts (ErrorResponse, ValidationErrorResponse)
- API versioning contracts
- Common enums (UserRole, SubscriptionStatus, PaymentStatus)
- Validation attributes for DTOs

## [0.1.0] - Unreleased

Initial release preparation.
