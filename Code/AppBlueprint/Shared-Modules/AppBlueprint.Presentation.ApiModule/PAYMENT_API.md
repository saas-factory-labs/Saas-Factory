# Payment API Documentation

## Overview
The Payment API provides endpoints for managing Stripe customers and subscriptions within the AppBlueprint application.

## Base URL
```
/api/payment
```

## Authentication
All endpoints require proper authentication. Authentication details depend on the application's configured authentication scheme.

## Endpoints

### Create Customer
Creates a new customer in Stripe.

```http
POST /api/payment/create-customer
```

**Request Body:**
```json
{
  "email": "customer@example.com",
  "paymentMethodId": "pm_1234567890",
  "name": "John Doe",
  "phone": "+1234567890"
}
```

**Response (201 Created):**
```json
{
  "id": "cus_1234567890",
  "email": "customer@example.com",
  "name": "John Doe",
  "phone": "+1234567890",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

### Create Subscription
Creates a new subscription for a customer.

```http
POST /api/payment/create-subscription
```

**Request Body:**
```json
{
  "customerId": "cus_1234567890",
  "priceId": "price_1234567890"
}
```

**Response (201 Created):**
```json
{
  "id": "sub_1234567890",
  "customerId": "cus_1234567890",
  "status": "active",
  "priceId": "price_1234567890",
  "amount": 1999,
  "currency": "usd",
  "createdAt": "2024-01-15T10:30:00Z",
  "currentPeriodStart": "2024-01-15T10:30:00Z",
  "currentPeriodEnd": "2024-02-15T10:30:00Z"
}
```

### Get Subscription
Retrieves details of a specific subscription.

```http
GET /api/payment/subscription/{subscriptionId}
```

**Response (200 OK):**
```json
{
  "id": "sub_1234567890",
  "customerId": "cus_1234567890",
  "status": "active",
  "priceId": "price_1234567890",
  "amount": 1999,
  "currency": "usd",
  "createdAt": "2024-01-15T10:30:00Z",
  "canceledAt": null,
  "currentPeriodStart": "2024-01-15T10:30:00Z",
  "currentPeriodEnd": "2024-02-15T10:30:00Z"
}
```

### Get Customer Subscriptions
Retrieves all subscriptions for a specific customer.

```http
GET /api/payment/customer/{customerId}/subscriptions
```

**Response (200 OK):**
```json
{
  "subscriptions": [
    {
      "id": "sub_1234567890",
      "customerId": "cus_1234567890",
      "status": "active",
      "priceId": "price_1234567890",
      "amount": 1999,
      "currency": "usd",
      "createdAt": "2024-01-15T10:30:00Z",
      "canceledAt": null,
      "currentPeriodStart": "2024-01-15T10:30:00Z",
      "currentPeriodEnd": "2024-02-15T10:30:00Z"
    }
  ],
  "hasMore": false,
  "totalCount": 1
}
```

### Cancel Subscription
Cancels an existing subscription.

```http
POST /api/payment/cancel-subscription
```

**Request Body:**
```json
{
  "subscriptionId": "sub_1234567890"
}
```

**Response (200 OK):**
```json
{
  "id": "sub_1234567890",
  "customerId": "cus_1234567890",
  "status": "canceled",
  "priceId": "price_1234567890",
  "amount": 1999,
  "currency": "usd",
  "createdAt": "2024-01-15T10:30:00Z",
  "canceledAt": "2024-01-20T10:30:00Z",
  "currentPeriodStart": "2024-01-15T10:30:00Z",
  "currentPeriodEnd": "2024-02-15T10:30:00Z"
}
```

## Error Responses

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Invalid Request",
  "detail": "Email cannot be null or empty"
}
```

### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Subscription Not Found",
  "detail": "The requested subscription could not be found"
}
```

### 500 Internal Server Error
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Customer Creation Failed",
  "detail": "An error occurred while creating the customer"
}
```

## Configuration

### Required Settings
The following configuration is required in your application settings:

```json
{
  "ConnectionStrings": {
    "StripeApiKey": "sk_test_your_stripe_secret_key_here"
  }
}
```

### Environment Variables
For production, it's recommended to use environment variables:
- `ConnectionStrings__StripeApiKey`: Your Stripe secret API key

## Notes
- All monetary amounts are returned in cents (e.g., $19.99 = 1999)
- All timestamps are in UTC format
- The API uses Stripe's standard status values for subscriptions (active, canceled, incomplete, etc.)
- Payment method IDs should be obtained through Stripe's client-side libraries (Stripe.js, etc.)