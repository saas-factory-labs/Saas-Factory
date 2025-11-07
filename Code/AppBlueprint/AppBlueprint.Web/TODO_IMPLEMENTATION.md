# Todo Feature Implementation

## Overview
This document describes the MudBlazor-based Todo feature implementation that connects to the TodoAppKernel API.

## Files Created/Modified

### 1. TodoService.cs (NEW)
**Location:** `AppBlueprint.Web/Services/TodoService.cs`

**Purpose:** HTTP client service for making API calls to the TodoAppKernel endpoints.

**Features:**
- Get all todos: `GetTodosAsync()`
- Create todo: `CreateTodoAsync(CreateTodoRequest)`
- Get todo by ID: `GetTodoByIdAsync(string id)`
- Update todo: `UpdateTodoAsync(string id, UpdateTodoRequest)`
- Delete todo: `DeleteTodoAsync(string id)`
- Complete todo: `CompleteTodoAsync(string id)`

**API Endpoints Used:**
- `GET /api/v1.0/todo` - List all todos
- `POST /api/v1.0/todo` - Create new todo
- `GET /api/v1.0/todo/{id}` - Get todo by ID
- `PUT /api/v1.0/todo/{id}` - Update todo
- `DELETE /api/v1.0/todo/{id}` - Delete todo
- `PATCH /api/v1.0/todo/{id}/complete` - Mark todo as complete

### 2. TodoPage.razor (UPDATED)
**Location:** `AppBlueprint.Web/Components/Pages/TodoPage.razor`

**Purpose:** Interactive Blazor page for managing todos with MudBlazor components.

**Lifecycle:**
- Uses `OnAfterRenderAsync` to load todos (instead of `OnInitializedAsync`)
- This ensures JavaScript interop is available for authentication token retrieval
- Handles Blazor's prerendering phase gracefully

**UI Components:**
- **Add Todo Form:**
  - Title input field (required, max 200 chars with counter)
  - Description input field (optional, max 1000 chars with counter)
  - Add button with loading state
  - Form validation using MudForm

- **Todo List:**
  - Empty state message when no todos exist
  - Todo count display (active/total)
  - Each todo item shows:
    - Checkbox for completion toggle
    - Title and description
    - Delete button
    - Completion timestamp (when completed)
  - Todos are ordered by completion status, then by creation date

**User Experience:**
- Real-time form validation
- Loading indicators for async operations
- Success/error notifications using MudSnackbar
- Responsive layout using MudGrid
- Strikethrough text for completed todos

### 3. AuthenticationDelegatingHandler.cs (NEW)
**Location:** `AppBlueprint.Web/Services/AuthenticationDelegatingHandler.cs`

**Purpose:** HTTP message handler that automatically adds JWT authentication tokens to outgoing HTTP requests.

**Features:**
- Retrieves JWT token from `ITokenStorageService` (browser local storage)
- Adds Bearer token to Authorization header
- Logs authentication status for debugging
- Works with Logto authentication flow

### 4. Program.cs (UPDATED)
**Location:** `AppBlueprint.Web/Program.cs`

**Changes:** 
- Registered `TodoService` with dependency injection
- Registered `AuthenticationDelegatingHandler` as a transient service
- Configured HttpClient with Aspire service discovery
- Added authentication handler to HttpClient pipeline
- Uses `https+http://apiservice` for automatic service resolution

```csharp
// Register authentication handler for TodoService
builder.Services.AddTransient<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>();

// Add TodoService with HttpClient configured for Aspire service discovery
builder.Services.AddHttpClient<AppBlueprint.Web.Services.TodoService>(client =>
{
    client.BaseAddress = new Uri("https+http://apiservice");
})
.AddHttpMessageHandler<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>();
```

### 5. appsettings.json (ALREADY CONFIGURED)
**Location:** `AppBlueprint.Web/appsettings.json`

**Navigation Entry:**
```json
{
  "Name": "Todos",
  "Href": "/todos",
  "MudblazorIconPath": "Icons.Material.Filled.CheckCircle"
}
```

## Architecture

### Authentication Flow
The TodoService automatically includes JWT authentication tokens in API requests:

1. **User Authentication:**
   - User logs in via Logto (OAuth/OIDC flow)
   - JWT token is stored in browser local storage via `ITokenStorageService`

2. **API Request Flow:**
   - TodoService makes HTTP request
   - `AuthenticationDelegatingHandler` intercepts the request
   - Handler retrieves JWT token from `ITokenStorageService`
   - Handler adds `Authorization: Bearer {token}` header
   - Request proceeds to API with authentication

3. **Token Management:**
   - Tokens are persisted in browser local storage
   - Tokens are automatically included in all TodoService requests
   - 401 Unauthorized responses indicate expired/invalid tokens

### Service Discovery
The implementation uses **Aspire service discovery** to automatically locate the API service:
- The TodoService HttpClient is configured with `https+http://apiservice`
- Aspire resolves this to the actual API endpoint at runtime
- No hardcoded URLs needed in the Web project

### API Integration
The TodoAppKernel module provides the backend API:
- Controllers: `AppBlueprint.TodoAppKernel/Controllers/TodoController.cs`
- Domain: `AppBlueprint.TodoAppKernel/Domain/TodoEntity.cs`
- DTOs: `AppBlueprint.TodoAppKernel/Controllers/Dto/`

### Data Flow
```
User Interaction (TodoPage.razor)
    ↓
TodoService (HTTP calls)
    ↓
AuthenticationDelegatingHandler (adds JWT token)
    ↓
Aspire Service Discovery
    ↓
API Service (apiservice)
    ↓
TodoController (TodoAppKernel)
    ↓
Database (via Entity Framework)
```

## Usage

### Accessing the Page
Navigate to `/todos` in the application or use the navigation menu item "Todos".

### Adding a Todo
1. Enter a title (required)
2. Optionally enter a description
3. Click "Add Todo" button
4. The form will reset and the new todo appears in the list

### Completing a Todo
1. Click the checkbox next to any todo
2. The todo will be marked as complete and moved to the bottom of the list
3. A completion timestamp will be displayed

### Deleting a Todo
1. Click the trash icon button on the right side of any todo
2. The todo will be immediately removed from the list

## MudBlazor Components Used

- `MudContainer` - Main layout container
- `MudPaper` - Card-like surfaces for form and list items
- `MudForm` - Form validation
- `MudTextField` - Input fields with validation and character counters
- `MudButton` - Action buttons
- `MudCheckBox` - Completion toggle
- `MudIconButton` - Delete action
- `MudGrid` / `MudItem` - Responsive layout
- `MudProgressLinear` - Page loading indicator
- `MudProgressCircular` - Button loading spinner
- `MudAlert` - Error messages
- `MudText` - Typography
- `MudIcon` - Empty state icon
- `MudStack` - Vertical stacking of todo items
- `ISnackbar` - Toast notifications

## Future Enhancements

Potential improvements that could be added:
- [ ] Mark completed todos as incomplete
- [ ] Edit existing todos inline
- [ ] Priority levels (Low, Medium, High, Urgent)
- [ ] Due dates
- [ ] Filtering by completion status
- [ ] Sorting options
- [ ] Search functionality
- [ ] Pagination for large lists
- [ ] Bulk operations (complete/delete multiple)
- [ ] Categories/tags
- [ ] User assignment (assign to other users)

## Testing

To test the implementation:
1. Ensure AppHost is running (do not rebuild if already running in watch mode)
2. Navigate to the Web application
3. **Log in via Logto** - This is required to get a JWT token
4. Click on "Todos" in the navigation menu
5. Try adding, completing, and deleting todos
6. Verify API calls in browser developer tools Network tab

### Troubleshooting Authentication Issues

**401 Unauthorized Error:**
- Ensure you are logged in via Logto
- Check browser DevTools > Application > Local Storage for `auth_token`
- Verify the token is being added to requests (Network tab > Headers > Authorization)
- Check token expiration (JWT tokens typically expire after 1 hour)
- Try logging out and logging back in to get a fresh token

**Token Not Found:**
- The `AuthenticationDelegatingHandler` logs warnings when no token is found
- Check browser console for authentication-related log messages
- Ensure Logto authentication flow completed successfully

**Token Storage:**
- Tokens are stored in browser local storage via `ITokenStorageService`
- Key: `auth_token`
- Clearing browser data will remove stored tokens

## Notes

- The API currently returns empty lists (placeholder implementation in TodoController)
- Full database persistence needs to be implemented in the TodoAppKernel module
- **Authentication is configured with Logto** - JWT tokens are automatically added to API requests
- The `AuthenticationDelegatingHandler` retrieves tokens from browser local storage
- All API calls use proper cancellation tokens for performance
- Error handling is implemented with try-catch and user-friendly messages
- 401 Unauthorized errors indicate authentication issues (token missing/expired/invalid)

