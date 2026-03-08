# API Contract: MVP Subscription Management

**Purpose**: Define REST API contract between backend and frontend for subscription management.

**Version**: 1.0 | **Date**: 2026-03-08

**Base URL**: `http://localhost:5000` (development) | `https://api.example.com` (production)

---

## Endpoints

### 1. GET /api/subscriptions

**Purpose**: Retrieve all current subscriptions.

**Request**:
```http
GET /api/subscriptions HTTP/1.1
Host: localhost:5000
Accept: application/json
```

**Response (200 OK)**:
```json
{
  "subscriptions": [
    {
      "id": 1,
      "url": "https://devblogs.microsoft.com/dotnet/feed/",
      "addedAt": "2026-03-08T10:30:00Z"
    },
    {
      "id": 2,
      "url": "https://example.com/rss",
      "addedAt": "2026-03-08T10:35:00Z"
    }
  ]
}
```

**Response (500 Internal Server Error)**:
```json
{
  "error": "An error occurred while retrieving subscriptions"
}
```

**Status Codes**:
- **200 OK**: Subscriptions retrieved successfully (even if list is empty)
- **500 Internal Server Error**: Server error (rare in MVP)

**Notes**:
- Always returns array, even if empty
- Subscriptions ordered by `addedAt` (oldest first)
- No filtering or pagination in MVP

---

### 2. POST /api/subscriptions

**Purpose**: Add a new subscription.

**Request**:
```http
POST /api/subscriptions HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "url": "https://example.com/feed.xml"
}
```

**Request Body Schema**:
```json
{
  "type": "object",
  "properties": {
    "url": {
      "type": "string",
      "minLength": 1,
      "maxLength": 2000,
      "description": "RSS/Atom feed URL"
    }
  },
  "required": ["url"]
}
```

**Response (201 Created)**:
```json
{
  "id": 3,
  "url": "https://example.com/feed.xml",
  "addedAt": "2026-03-08T10:40:00Z"
}
```

**Response (400 Bad Request)** - Empty/whitespace URL:
```json
{
  "error": "URL cannot be empty"
}
```

**Response (400 Bad Request)** - URL exceeds max length:
```json
{
  "error": "URL cannot exceed 2000 characters"
}
```

**Response (500 Internal Server Error)**:
```json
{
  "error": "An error occurred while adding the subscription"
}
```

**Status Codes**:
- **201 Created**: Subscription added successfully
- **400 Bad Request**: Invalid input (empty, too long, etc.)
- **500 Internal Server Error**: Server error

**Notes**:
- Backend trims whitespace from URL before storing
- No validation of URL format or RSS/Atom feed validity
- Allows duplicate URLs (de-duplication deferred to Extended-MVP)
- Returns created subscription object for frontend confirmation

---

## Error Handling

### HTTP Status Code Usage

| Status | Meaning | When Used |
|--------|---------|-----------|
| 200 OK | Request succeeded | GET successful |
| 201 Created | Resource created | POST successful |
| 400 Bad Request | Invalid input | Empty URL, too long URL |
| 500 Internal Server Error | Server error | Unexpected exceptions |

### Error Response Format

All error responses follow this format:
```json
{
  "error": "Human-readable error message"
}
```

**No stack traces or implementation details** in error messages (per Constitution principle: error messages must not expose internal details).

---

## Request/Response Headers

### Request Headers (Required)
- `Content-Type: application/json` (for POST)
- `Accept: application/json`

### Response Headers
- `Content-Type: application/json`
- `X-Total-Count` (optional, for listing endpoints)

---

## CORS Configuration

**Development**:
```
Allowed Origins: http://localhost:5173, https://localhost:5173
Allowed Methods: GET, POST, OPTIONS
Allowed Headers: Content-Type, Accept
```

**Per Constitution Security Requirement**: Never use wildcard `allow-credentials: true` or Access-Control-Allow-Origin: *

---

## API Versioning Strategy (Future)

When Extended-MVP adds new endpoints:
- Current version: v1 (implied, unversioned in URL)
- New endpoints: Use `GET /api/v1/subscriptions` format
- Deprecation: Support v1 for 2 minor versions before removal
- Backwards compatibility: Maintain API contract when adding fields (mark as optional)

**For MVP**: No versioning prefix needed (only internal endpoint).

---

## Rate Limiting (Future)

For Extended-MVP or production:
- Rate limit: 100 requests per minute per IP
- Header: `X-RateLimit-Remaining`, `X-RateLimit-Reset`
- Status 429 Too Many Requests when exceeded

**MVP**: No rate limiting (single-user, trusted environment).

---

## Authentication & Authorization (Future)

For multi-user Extended-MVP:
- OAuth 2.0 or JWT tokens
- Endpoint: `POST /api/auth/login`
- Headers: `Authorization: Bearer {token}`
- 401 Unauthorized if token missing/invalid
- 403 Forbidden if user lacks permission

**MVP**: No authentication (single-user assumed).

---

## Example API Usage Flow

### Flow: Add Two Subscriptions

```
1. POST /api/subscriptions
   Request: { "url": "https://devblogs.microsoft.com/dotnet/feed/" }
   Response: { 
     "id": 1, 
     "url": "https://devblogs.microsoft.com/dotnet/feed/", 
     "addedAt": "2026-03-08T10:30:00Z" 
   }

2. GET /api/subscriptions
   Response: {
     "subscriptions": [
       { 
         "id": 1, 
         "url": "https://devblogs.microsoft.com/dotnet/feed/",
         "addedAt": "2026-03-08T10:30:00Z" 
       }
     ]
   }

3. POST /api/subscriptions
   Request: { "url": "https://example.com/rss.xml" }
   Response: { 
     "id": 2, 
     "url": "https://example.com/rss.xml", 
     "addedAt": "2026-03-08T10:35:00Z" 
   }

4. GET /api/subscriptions
   Response: {
     "subscriptions": [
       { 
         "id": 1, 
         "url": "https://devblogs.microsoft.com/dotnet/feed/",
         "addedAt": "2026-03-08T10:30:00Z" 
       },
       { 
         "id": 2, 
         "url": "https://example.com/rss.xml",
         "addedAt": "2026-03-08T10:35:00Z" 
       }
     ]
   }
```

---

## API Documentation Tools

**Recommended for reference**:
- Swagger/OpenAPI: Generate interactive API documentation (future)
- Postman: Test API endpoints (development)

**For MVP**: Manual documentation (this file) is sufficient.
