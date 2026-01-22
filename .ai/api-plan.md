# REST API Plan

## 1. Resources

| Resource | Database Table | Description |
|----------|---------------|-------------|
| Auth | auth.users (Supabase) | Authentication operations (register, login, logout) |
| Profiles | profiles | User profile management |
| Locations | locations | Tourist destinations (cities, regions) |
| Attractions | attractions | Tourist attractions with details |
| Trips | trips | User trip plans |
| TripAttractions | trip_attractions | Attractions assigned to trips with schedule |

---

## 2. Endpoints

### 2.1. Authentication

#### POST /api/auth/register

Register a new user account.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "displayName": "John Doe"
}
```

**Response (201 Created):**
```json
{
  "id": "uuid",
  "email": "user@example.com",
  "displayName": "John Doe",
  "createdAt": "2026-01-22T10:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error (invalid email, weak password) |
| 409 | Email already registered |

---

#### POST /api/auth/login

Authenticate user and return JWT token.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2026-01-23T10:00:00Z",
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "displayName": "John Doe"
  }
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error |
| 401 | Invalid credentials |

---

#### POST /api/auth/logout

Invalidate current session/token.

**Headers:**
- `Authorization: Bearer {token}`

**Response (204 No Content)**

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |

---

#### GET /api/auth/me

Get current authenticated user information.

**Headers:**
- `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "id": "uuid",
  "email": "user@example.com",
  "displayName": "John Doe",
  "createdAt": "2026-01-22T10:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |

---

### 2.2. Profiles

#### GET /api/profiles/me

Get current user's profile.

**Headers:**
- `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "id": "uuid",
  "displayName": "John Doe",
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T10:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |

---

#### PUT /api/profiles/me

Update current user's profile.

**Headers:**
- `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "displayName": "John Smith"
}
```

**Response (200 OK):**
```json
{
  "id": "uuid",
  "displayName": "John Smith",
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T11:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error (displayName too long) |
| 401 | Not authenticated |

---

### 2.3. Locations

#### GET /api/locations

List all locations with optional search and pagination.

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| search | string | No | Search by name or country |
| page | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 20, max: 100) |

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "uuid",
      "name": "Athens",
      "country": "Greece",
      "timezone": "Europe/Athens"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8
  }
}
```

---

#### GET /api/locations/{id}

Get a specific location by ID.

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Location identifier |

**Response (200 OK):**
```json
{
  "id": "uuid",
  "name": "Athens",
  "country": "Greece",
  "timezone": "Europe/Athens",
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": "2026-01-01T00:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 404 | Location not found |

---

### 2.4. Attractions

#### GET /api/attractions

List attractions with filtering, sorting, and pagination.

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| locationId | UUID | No | Filter by location |
| search | string | No | Search by name |
| sortBy | string | No | Sort field: `rating`, `name`, `reviewCount` (default: `rating`) |
| sortOrder | string | No | `asc` or `desc` (default: `desc`) |
| isVerified | bool | No | Filter verified/unverified attractions |
| page | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 20, max: 100) |

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "uuid",
      "locationId": "uuid",
      "name": "Acropolis of Athens",
      "description": "Ancient citadel located on a rocky outcrop...",
      "latitude": 37.9715323,
      "longitude": 23.7257492,
      "rating": 4.8,
      "reviewCount": 95420,
      "estimatedDuration": 180,
      "imageUrl": "https://example.com/acropolis.jpg",
      "isVerified": true,
      "createdByUserId": null
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 45,
    "totalPages": 3
  }
}
```

---

#### GET /api/attractions/{id}

Get a specific attraction by ID.

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Attraction identifier |

**Response (200 OK):**
```json
{
  "id": "uuid",
  "locationId": "uuid",
  "location": {
    "id": "uuid",
    "name": "Athens",
    "country": "Greece"
  },
  "name": "Acropolis of Athens",
  "description": "Ancient citadel located on a rocky outcrop...",
  "latitude": 37.9715323,
  "longitude": 23.7257492,
  "rating": 4.8,
  "reviewCount": 95420,
  "estimatedDuration": 180,
  "imageUrl": "https://example.com/acropolis.jpg",
  "isVerified": true,
  "createdByUserId": null,
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": "2026-01-01T00:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 404 | Attraction not found |

---

#### POST /api/attractions

Create a custom attraction (user-generated).

**Headers:**
- `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "locationId": "uuid",
  "name": "Hidden Coffee Shop",
  "description": "A great local coffee shop...",
  "latitude": 37.9750,
  "longitude": 23.7350,
  "estimatedDuration": 45,
  "imageUrl": "https://example.com/coffee.jpg"
}
```

**Response (201 Created):**
```json
{
  "id": "uuid",
  "locationId": "uuid",
  "name": "Hidden Coffee Shop",
  "description": "A great local coffee shop...",
  "latitude": 37.9750,
  "longitude": 23.7350,
  "rating": null,
  "reviewCount": null,
  "estimatedDuration": 45,
  "imageUrl": "https://example.com/coffee.jpg",
  "isVerified": false,
  "createdByUserId": "uuid",
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T10:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error |
| 401 | Not authenticated |
| 404 | Location not found |

---

#### PUT /api/attractions/{id}

Update a custom attraction (only owner can update).

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Attraction identifier |

**Request Body:**
```json
{
  "name": "Amazing Coffee Shop",
  "description": "Updated description...",
  "latitude": 37.9751,
  "longitude": 23.7351,
  "estimatedDuration": 60,
  "imageUrl": "https://example.com/coffee-updated.jpg"
}
```

**Response (200 OK):**
```json
{
  "id": "uuid",
  "locationId": "uuid",
  "name": "Amazing Coffee Shop",
  "description": "Updated description...",
  "latitude": 37.9751,
  "longitude": 23.7351,
  "rating": null,
  "reviewCount": null,
  "estimatedDuration": 60,
  "imageUrl": "https://example.com/coffee-updated.jpg",
  "isVerified": false,
  "createdByUserId": "uuid",
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T11:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error |
| 401 | Not authenticated |
| 403 | Cannot modify attraction created by another user |
| 404 | Attraction not found |

---

#### DELETE /api/attractions/{id}

Delete a custom attraction (only owner can delete).

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Attraction identifier |

**Response (204 No Content)**

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |
| 403 | Cannot delete attraction created by another user |
| 404 | Attraction not found |
| 409 | Attraction is used in other users' trips |

---

### 2.5. Trips

#### GET /api/trips

List user's trips and public trips from other users.

**Headers:**
- `Authorization: Bearer {token}`

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| locationId | UUID | No | Filter by location |
| onlyMine | bool | No | Show only user's own trips (default: false) |
| onlyPublic | bool | No | Show only public trips (default: false) |
| search | string | No | Search by trip name |
| page | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 20, max: 100) |

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "uuid",
      "ownerId": "uuid",
      "name": "Athens 2026",
      "locationId": "uuid",
      "location": {
        "id": "uuid",
        "name": "Athens",
        "country": "Greece"
      },
      "isPublic": false,
      "dailyHours": 8,
      "maxExtensionHours": 2,
      "startTime": "09:00:00",
      "attractionCount": 12,
      "totalDays": 3,
      "isOwner": true,
      "createdAt": "2026-01-22T10:00:00Z",
      "updatedAt": "2026-01-22T10:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 5,
    "totalPages": 1
  }
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |

---

#### GET /api/trips/{id}

Get a specific trip with full details.

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Trip identifier |

**Response (200 OK):**
```json
{
  "id": "uuid",
  "ownerId": "uuid",
  "name": "Athens 2026",
  "locationId": "uuid",
  "location": {
    "id": "uuid",
    "name": "Athens",
    "country": "Greece",
    "timezone": "Europe/Athens"
  },
  "isPublic": false,
  "dailyHours": 8,
  "maxExtensionHours": 2,
  "startTime": "09:00:00",
  "isOwner": true,
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T10:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |
| 403 | Trip is private and not owned by user |
| 404 | Trip not found |

---

#### POST /api/trips

Create a new trip.

**Headers:**
- `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "name": "Athens 2026",
  "locationId": "uuid",
  "dailyHours": 8,
  "maxExtensionHours": 2,
  "startTime": "09:00"
}
```

**Response (201 Created):**
```json
{
  "id": "uuid",
  "ownerId": "uuid",
  "name": "Athens 2026",
  "locationId": "uuid",
  "isPublic": false,
  "dailyHours": 8,
  "maxExtensionHours": 2,
  "startTime": "09:00:00",
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T10:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error |
| 401 | Not authenticated |
| 404 | Location not found |

---

#### PUT /api/trips/{id}

Update an existing trip (only owner can update).

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Trip identifier |

**Request Body:**
```json
{
  "name": "Athens Summer 2026",
  "locationId": "uuid",
  "dailyHours": 10,
  "maxExtensionHours": 3,
  "startTime": "08:00"
}
```

**Response (200 OK):**
```json
{
  "id": "uuid",
  "ownerId": "uuid",
  "name": "Athens Summer 2026",
  "locationId": "uuid",
  "isPublic": false,
  "dailyHours": 10,
  "maxExtensionHours": 3,
  "startTime": "08:00:00",
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T11:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error |
| 401 | Not authenticated |
| 403 | Cannot modify trip owned by another user |
| 404 | Trip or location not found |

---

#### DELETE /api/trips/{id}

Delete a trip (only owner can delete).

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Trip identifier |

**Response (204 No Content)**

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |
| 403 | Cannot delete trip owned by another user |
| 404 | Trip not found |

---

#### PATCH /api/trips/{id}/publish

Toggle trip public/private status.

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Trip identifier |

**Request Body:**
```json
{
  "isPublic": true
}
```

**Response (200 OK):**
```json
{
  "id": "uuid",
  "isPublic": true,
  "updatedAt": "2026-01-22T11:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error |
| 401 | Not authenticated |
| 403 | Cannot modify trip owned by another user |
| 404 | Trip not found |

---

### 2.6. Trip Attractions

#### GET /api/trips/{tripId}/attractions

Get all attractions in a trip with schedule.

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| tripId | UUID | Trip identifier |

**Response (200 OK):**
```json
{
  "tripId": "uuid",
  "totalDays": 3,
  "totalDuration": 1440,
  "days": [
    {
      "dayNumber": 1,
      "totalDuration": 480,
      "attractions": [
        {
          "id": "uuid",
          "attractionId": "uuid",
          "attraction": {
            "id": "uuid",
            "name": "Acropolis of Athens",
            "latitude": 37.9715323,
            "longitude": 23.7257492,
            "rating": 4.8,
            "estimatedDuration": 180,
            "imageUrl": "https://example.com/acropolis.jpg"
          },
          "dayNumber": 1,
          "orderIndex": 1,
          "plannedStartTime": "09:00:00"
        }
      ]
    }
  ]
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |
| 403 | Trip is private and not owned by user |
| 404 | Trip not found |

---

#### POST /api/trips/{tripId}/attractions

Add an attraction to a trip.

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| tripId | UUID | Trip identifier |

**Request Body:**
```json
{
  "attractionId": "uuid",
  "dayNumber": 1,
  "orderIndex": 3
}
```

**Response (201 Created):**
```json
{
  "id": "uuid",
  "tripId": "uuid",
  "attractionId": "uuid",
  "dayNumber": 1,
  "orderIndex": 3,
  "plannedStartTime": null,
  "createdAt": "2026-01-22T10:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error |
| 401 | Not authenticated |
| 403 | Cannot modify trip owned by another user |
| 404 | Trip or attraction not found |
| 409 | Attraction already exists in this trip |

---

#### PUT /api/trips/{tripId}/attractions/{attractionId}

Update attraction assignment in a trip (day, order, time).

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| tripId | UUID | Trip identifier |
| attractionId | UUID | Attraction identifier |

**Request Body:**
```json
{
  "dayNumber": 2,
  "orderIndex": 1,
  "plannedStartTime": "10:00"
}
```

**Response (200 OK):**
```json
{
  "id": "uuid",
  "tripId": "uuid",
  "attractionId": "uuid",
  "dayNumber": 2,
  "orderIndex": 1,
  "plannedStartTime": "10:00:00",
  "updatedAt": "2026-01-22T11:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error |
| 401 | Not authenticated |
| 403 | Cannot modify trip owned by another user |
| 404 | Trip or trip_attraction not found |

---

#### DELETE /api/trips/{tripId}/attractions/{attractionId}

Remove an attraction from a trip.

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| tripId | UUID | Trip identifier |
| attractionId | UUID | Attraction identifier |

**Response (204 No Content)**

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |
| 403 | Cannot modify trip owned by another user |
| 404 | Trip or trip_attraction not found |

---

#### POST /api/trips/{tripId}/attractions/reorder

Bulk reorder attractions within a trip.

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| tripId | UUID | Trip identifier |

**Request Body:**
```json
{
  "attractions": [
    {
      "attractionId": "uuid",
      "dayNumber": 1,
      "orderIndex": 1
    },
    {
      "attractionId": "uuid",
      "dayNumber": 1,
      "orderIndex": 2
    },
    {
      "attractionId": "uuid",
      "dayNumber": 2,
      "orderIndex": 1
    }
  ]
}
```

**Response (200 OK):**
```json
{
  "tripId": "uuid",
  "updatedCount": 3,
  "updatedAt": "2026-01-22T11:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error (invalid day/order numbers, missing attractions) |
| 401 | Not authenticated |
| 403 | Cannot modify trip owned by another user |
| 404 | Trip not found |

---

### 2.7. Route Optimization

#### POST /api/trips/{tripId}/optimize-route

Calculate and apply optimal visiting order using nearest neighbor algorithm.

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| tripId | UUID | Trip identifier |

**Request Body:**
```json
{
  "startingAttractionId": "uuid"
}
```

**Response (200 OK):**
```json
{
  "tripId": "uuid",
  "optimizedOrder": [
    {
      "attractionId": "uuid",
      "attractionName": "Acropolis of Athens",
      "dayNumber": 1,
      "orderIndex": 1
    },
    {
      "attractionId": "uuid",
      "attractionName": "Parthenon",
      "dayNumber": 1,
      "orderIndex": 2
    }
  ],
  "totalDistance": 12500.5,
  "optimizedAt": "2026-01-22T11:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error (starting attraction not in trip) |
| 401 | Not authenticated |
| 403 | Cannot modify trip owned by another user |
| 404 | Trip not found |
| 422 | Trip has no attractions to optimize |

---

### 2.8. Schedule Generation

#### POST /api/trips/{tripId}/generate-schedule

Auto-generate daily schedule based on trip settings and estimated durations.

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| tripId | UUID | Trip identifier |

**Response (200 OK):**
```json
{
  "tripId": "uuid",
  "totalDays": 3,
  "schedule": [
    {
      "dayNumber": 1,
      "totalDuration": 480,
      "isExtended": false,
      "attractions": [
        {
          "attractionId": "uuid",
          "attractionName": "Acropolis of Athens",
          "orderIndex": 1,
          "plannedStartTime": "09:00:00",
          "plannedEndTime": "12:00:00",
          "estimatedDuration": 180
        },
        {
          "attractionId": "uuid",
          "attractionName": "Ancient Agora",
          "orderIndex": 2,
          "plannedStartTime": "12:00:00",
          "plannedEndTime": "14:00:00",
          "estimatedDuration": 120
        }
      ]
    }
  ],
  "generatedAt": "2026-01-22T11:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |
| 403 | Cannot modify trip owned by another user |
| 404 | Trip not found |
| 422 | Trip has no attractions to schedule |

---

#### GET /api/trips/{tripId}/schedule

Get current schedule without regenerating.

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| tripId | UUID | Trip identifier |

**Response (200 OK):**
```json
{
  "tripId": "uuid",
  "totalDays": 3,
  "totalDuration": 1440,
  "dailyHours": 8,
  "startTime": "09:00:00",
  "schedule": [
    {
      "dayNumber": 1,
      "totalDuration": 480,
      "attractions": [
        {
          "attractionId": "uuid",
          "attractionName": "Acropolis of Athens",
          "orderIndex": 1,
          "plannedStartTime": "09:00:00",
          "estimatedDuration": 180
        }
      ]
    }
  ]
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |
| 403 | Trip is private and not owned by user |
| 404 | Trip not found |

---

## 3. Authentication and Authorization

### 3.1. Authentication Mechanism

The API uses **JWT (JSON Web Token)** based authentication integrated with **ASP.NET Core Identity**.

**Authentication Flow:**
1. User registers via `POST /api/auth/register`
2. User logs in via `POST /api/auth/login` and receives JWT token
3. Client includes token in `Authorization` header for subsequent requests
4. Token is validated on each request

**Token Format:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Token Claims:**
- `sub` - User ID (UUID)
- `email` - User email
- `exp` - Expiration timestamp
- `iat` - Issued at timestamp

### 3.2. Authorization Rules

| Resource | Action | Rule |
|----------|--------|------|
| Locations | Read | Public (all users) |
| Locations | Write | Admin only (service role) |
| Attractions | Read | Public (all users) |
| Attractions | Create | Authenticated users |
| Attractions | Update/Delete | Owner only |
| Trips | Read | Owner or public trips |
| Trips | Create | Authenticated users |
| Trips | Update/Delete | Owner only |
| TripAttractions | Read | Trip owner or public trips |
| TripAttractions | Create/Update/Delete | Trip owner only |

### 3.3. Row Level Security

The database uses PostgreSQL Row Level Security (RLS) policies to enforce authorization at the database level, providing an additional security layer beyond API-level checks.

---

## 4. Validation and Business Logic

### 4.1. Validation Rules by Resource

#### Profiles
| Field | Rule |
|-------|------|
| displayName | Max 100 characters |

#### Locations
| Field | Rule |
|-------|------|
| name | Required, max 100 characters |
| country | Required, max 100 characters |
| timezone | Max 50 characters, valid IANA timezone format |

#### Attractions
| Field | Rule |
|-------|------|
| locationId | Required, must reference existing location |
| name | Required, max 200 characters |
| description | Optional, text |
| latitude | Required, range: -90 to 90 |
| longitude | Required, range: -180 to 180 |
| rating | Optional, range: 0.0 to 5.0, precision: 1 decimal |
| reviewCount | Optional, non-negative integer |
| estimatedDuration | Optional, positive integer (minutes) |
| imageUrl | Optional, max 500 characters, valid URL format |

#### Trips
| Field | Rule |
|-------|------|
| name | Required, max 100 characters |
| locationId | Optional, must reference existing location |
| dailyHours | Required, range: 1 to 24 |
| maxExtensionHours | Required, range: 0 to 8 |
| startTime | Required, valid time format (HH:mm) |

#### TripAttractions
| Field | Rule |
|-------|------|
| tripId | Required, must reference existing trip owned by user |
| attractionId | Required, must reference existing attraction |
| dayNumber | Required, positive integer (> 0) |
| orderIndex | Required, positive integer (> 0) |
| plannedStartTime | Optional, valid time format (HH:mm) |

**Constraint:** Same attraction cannot be added to the same trip twice (unique constraint on tripId + attractionId).

### 4.2. Business Logic Implementation

#### Route Optimization (FR-10, FR-11)
- **Algorithm:** Nearest Neighbor
- **Process:**
  1. Start from the specified starting attraction
  2. Calculate Haversine distance to all unvisited attractions
  3. Select the nearest unvisited attraction
  4. Repeat until all attractions are visited
- **Output:** Ordered list of attractions with minimal travel distance

#### Time Planning (FR-15, FR-17, FR-18)
- **Process:**
  1. Start from `startTime` of the trip
  2. Assign attractions sequentially based on `orderIndex`
  3. Calculate `plannedStartTime` for each attraction by adding previous durations
  4. When daily time limit (`dailyHours`) is exceeded:
     - Check if extension is possible (`maxExtensionHours`)
     - If not, move to next day and reset to `startTime`
  5. Increment `dayNumber` accordingly
- **Output:** Complete schedule with days, times, and durations

#### Attraction Deletion (Application-level validation)
- Before deleting a user-created attraction:
  1. Check if attraction is used in trips owned by other users
  2. If yes, reject deletion with 409 Conflict
  3. If only used in owner's trips, cascade delete from trip_attractions

#### Trip Visibility
- Private trips (`isPublic = false`) are only accessible by the owner
- Public trips (`isPublic = true`) are readable by all authenticated users
- Only the owner can modify a trip regardless of visibility

### 4.3. Error Response Format

All error responses follow a consistent format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Validation failed",
  "errors": {
    "name": ["Name is required", "Name must not exceed 100 characters"],
    "latitude": ["Latitude must be between -90 and 90"]
  },
  "traceId": "00-abc123-def456-00"
}
```

### 4.4. Pagination Format

All list endpoints support pagination with consistent response format:

```json
{
  "items": [...],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

**Default values:**
- page: 1
- pageSize: 20
- maxPageSize: 100
