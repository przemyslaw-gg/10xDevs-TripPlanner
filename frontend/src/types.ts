// =============================================================================
// TripPlanner API Types
// =============================================================================
// This file contains TypeScript type definitions for DTOs (Data Transfer Objects)
// and Command Models used in communication with the REST API.
// Types are derived from database models defined in .ai/db-plan.md
// =============================================================================

// =============================================================================
// COMMON TYPES
// =============================================================================

/**
 * UUID type alias for clarity
 */
export type UUID = string;

/**
 * ISO 8601 datetime string (e.g., "2026-01-22T10:00:00Z")
 */
export type ISODateTime = string;

/**
 * Time string in HH:mm:ss or HH:mm format (e.g., "09:00:00" or "09:00")
 */
export type TimeString = string;

/**
 * Pagination metadata returned with list endpoints
 */
export interface PaginationDTO {
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasNextPage?: boolean;
  hasPreviousPage?: boolean;
}

/**
 * Generic paginated response wrapper
 */
export interface PaginatedResponse<T> {
  items: T[];
  pagination: PaginationDTO;
}

/**
 * Standard API error response format (RFC 7807 Problem Details)
 */
export interface ApiErrorResponse {
  type: string;
  title: string;
  status: number;
  detail: string;
  errors?: Record<string, string[]>;
  traceId?: string;
}

// =============================================================================
// BASE ENTITY TYPES (derived from database models)
// =============================================================================

/**
 * Base profile entity - extends Supabase auth.users
 * Maps to: profiles table
 */
export interface Profile {
  id: UUID;
  displayName: string | null;
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

/**
 * Location entity - tourist destinations (cities, regions)
 * Maps to: locations table
 */
export interface Location {
  id: UUID;
  name: string;
  country: string;
  timezone: string | null;
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

/**
 * Attraction entity - tourist attractions
 * Maps to: attractions table
 */
export interface Attraction {
  id: UUID;
  locationId: UUID;
  name: string;
  description: string | null;
  latitude: number;
  longitude: number;
  rating: number | null;
  reviewCount: number | null;
  estimatedDuration: number | null;
  imageUrl: string | null;
  createdByUserId: UUID | null;
  isVerified: boolean;
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

/**
 * Trip entity - user trip plans
 * Maps to: trips table
 */
export interface Trip {
  id: UUID;
  ownerId: UUID;
  name: string;
  locationId: UUID | null;
  isPublic: boolean;
  dailyHours: number;
  maxExtensionHours: number;
  startTime: TimeString;
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

/**
 * TripAttraction entity - junction table linking trips with attractions
 * Maps to: trip_attractions table
 */
export interface TripAttraction {
  id: UUID;
  tripId: UUID;
  attractionId: UUID;
  dayNumber: number;
  orderIndex: number;
  plannedStartTime: TimeString | null;
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

// =============================================================================
// AUTHENTICATION DTOs
// =============================================================================

/**
 * User information returned in auth responses
 */
export interface UserDTO {
  id: UUID;
  email: string;
  displayName: string;
}

/**
 * Response from POST /api/auth/register
 */
export interface RegisterResponseDTO {
  id: UUID;
  email: string;
  displayName: string;
  createdAt: ISODateTime;
}

/**
 * Response from POST /api/auth/login
 */
export interface LoginResponseDTO {
  accessToken: string;
  expiresAt: ISODateTime;
  user: UserDTO;
}

/**
 * Response from GET /api/auth/me
 */
export interface CurrentUserDTO {
  id: UUID;
  email: string;
  displayName: string;
  createdAt: ISODateTime;
}

// =============================================================================
// PROFILE DTOs
// =============================================================================

/**
 * Response from GET/PUT /api/profiles/me
 * Derived from: Profile entity
 */
export interface ProfileDTO {
  id: UUID;
  displayName: string | null;
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

// =============================================================================
// LOCATION DTOs
// =============================================================================

/**
 * Minimal location info for nested responses
 */
export interface LocationSummaryDTO {
  id: UUID;
  name: string;
  country: string;
}

/**
 * Location item in list responses
 * Response from GET /api/locations
 */
export interface LocationListItemDTO {
  id: UUID;
  name: string;
  country: string;
  timezone: string | null;
}

/**
 * Full location details
 * Response from GET /api/locations/{id}
 */
export interface LocationDTO extends LocationListItemDTO {
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

/**
 * Paginated locations response
 */
export type LocationsResponseDTO = PaginatedResponse<LocationListItemDTO>;

// =============================================================================
// ATTRACTION DTOs
// =============================================================================

/**
 * Minimal attraction info for nested responses (e.g., in trip schedules)
 */
export interface AttractionSummaryDTO {
  id: UUID;
  name: string;
  latitude: number;
  longitude: number;
  rating: number | null;
  estimatedDuration: number | null;
  imageUrl: string | null;
}

/**
 * Attraction item in list responses
 * Response from GET /api/attractions
 */
export interface AttractionListItemDTO {
  id: UUID;
  locationId: UUID;
  name: string;
  description: string | null;
  latitude: number;
  longitude: number;
  rating: number | null;
  reviewCount: number | null;
  estimatedDuration: number | null;
  imageUrl: string | null;
  isVerified: boolean;
  createdByUserId: UUID | null;
}

/**
 * Full attraction details with nested location
 * Response from GET /api/attractions/{id}
 */
export interface AttractionDTO extends AttractionListItemDTO {
  location: LocationSummaryDTO;
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

/**
 * Paginated attractions response
 */
export type AttractionsResponseDTO = PaginatedResponse<AttractionListItemDTO>;

// =============================================================================
// TRIP DTOs
// =============================================================================

/**
 * Trip item in list responses
 * Response from GET /api/trips
 */
export interface TripListItemDTO {
  id: UUID;
  ownerId: UUID;
  name: string;
  locationId: UUID | null;
  location: LocationSummaryDTO | null;
  isPublic: boolean;
  dailyHours: number;
  maxExtensionHours: number;
  startTime: TimeString;
  attractionCount: number;
  totalDays: number;
  isOwner: boolean;
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

/**
 * Full trip details with nested location
 * Response from GET /api/trips/{id}
 */
export interface TripDTO {
  id: UUID;
  ownerId: UUID;
  name: string;
  locationId: UUID | null;
  location: (LocationSummaryDTO & { timezone: string | null }) | null;
  isPublic: boolean;
  dailyHours: number;
  maxExtensionHours: number;
  startTime: TimeString;
  isOwner: boolean;
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

/**
 * Paginated trips response
 */
export type TripsResponseDTO = PaginatedResponse<TripListItemDTO>;

/**
 * Response from PATCH /api/trips/{id}/publish
 */
export interface PublishTripResponseDTO {
  id: UUID;
  isPublic: boolean;
  updatedAt: ISODateTime;
}

// =============================================================================
// TRIP ATTRACTION DTOs
// =============================================================================

/**
 * Single attraction assignment within a trip
 */
export interface TripAttractionItemDTO {
  id: UUID;
  attractionId: UUID;
  attraction: AttractionSummaryDTO;
  dayNumber: number;
  orderIndex: number;
  plannedStartTime: TimeString | null;
}

/**
 * Attractions grouped by day
 */
export interface TripDayDTO {
  dayNumber: number;
  totalDuration: number;
  attractions: TripAttractionItemDTO[];
}

/**
 * Response from GET /api/trips/{tripId}/attractions
 */
export interface TripAttractionsResponseDTO {
  tripId: UUID;
  totalDays: number;
  totalDuration: number;
  days: TripDayDTO[];
}

/**
 * Response from POST /api/trips/{tripId}/attractions
 */
export interface AddTripAttractionResponseDTO {
  id: UUID;
  tripId: UUID;
  attractionId: UUID;
  dayNumber: number;
  orderIndex: number;
  plannedStartTime: TimeString | null;
  createdAt: ISODateTime;
}

/**
 * Response from PUT /api/trips/{tripId}/attractions/{attractionId}
 */
export interface UpdateTripAttractionResponseDTO {
  id: UUID;
  tripId: UUID;
  attractionId: UUID;
  dayNumber: number;
  orderIndex: number;
  plannedStartTime: TimeString | null;
  updatedAt: ISODateTime;
}

/**
 * Response from POST /api/trips/{tripId}/attractions/reorder
 */
export interface ReorderAttractionsResponseDTO {
  tripId: UUID;
  updatedCount: number;
  updatedAt: ISODateTime;
}

// =============================================================================
// ROUTE OPTIMIZATION DTOs
// =============================================================================

/**
 * Single item in optimized route
 */
export interface OptimizedRouteItemDTO {
  attractionId: UUID;
  attractionName: string;
  dayNumber: number;
  orderIndex: number;
}

/**
 * Response from POST /api/trips/{tripId}/optimize-route
 */
export interface OptimizeRouteResponseDTO {
  tripId: UUID;
  optimizedOrder: OptimizedRouteItemDTO[];
  totalDistance: number;
  optimizedAt: ISODateTime;
}

// =============================================================================
// SCHEDULE DTOs
// =============================================================================

/**
 * Single attraction in schedule with time details
 */
export interface ScheduleAttractionDTO {
  attractionId: UUID;
  attractionName: string;
  orderIndex: number;
  plannedStartTime: TimeString;
  plannedEndTime: TimeString;
  estimatedDuration: number;
}

/**
 * Day schedule with attractions
 */
export interface ScheduleDayDTO {
  dayNumber: number;
  totalDuration: number;
  isExtended: boolean;
  attractions: ScheduleAttractionDTO[];
}

/**
 * Response from POST /api/trips/{tripId}/generate-schedule
 */
export interface GenerateScheduleResponseDTO {
  tripId: UUID;
  totalDays: number;
  schedule: ScheduleDayDTO[];
  generatedAt: ISODateTime;
}

/**
 * Response from GET /api/trips/{tripId}/schedule
 */
export interface ScheduleResponseDTO {
  tripId: UUID;
  totalDays: number;
  totalDuration: number;
  dailyHours: number;
  startTime: TimeString;
  schedule: Omit<ScheduleDayDTO, 'isExtended'>[];
}

// =============================================================================
// AUTHENTICATION COMMAND MODELS
// =============================================================================

/**
 * Request body for POST /api/auth/register
 */
export interface RegisterCommand {
  email: string;
  password: string;
  displayName: string;
}

/**
 * Request body for POST /api/auth/login
 */
export interface LoginCommand {
  email: string;
  password: string;
}

// =============================================================================
// PROFILE COMMAND MODELS
// =============================================================================

/**
 * Request body for PUT /api/profiles/me
 * Derived from: Profile entity (partial)
 */
export interface UpdateProfileCommand {
  displayName: string;
}

// =============================================================================
// ATTRACTION COMMAND MODELS
// =============================================================================

/**
 * Request body for POST /api/attractions
 * Derived from: Attraction entity (omitting auto-generated fields)
 */
export interface CreateAttractionCommand {
  locationId: UUID;
  name: string;
  description?: string | null;
  latitude: number;
  longitude: number;
  estimatedDuration?: number | null;
  imageUrl?: string | null;
}

/**
 * Request body for PUT /api/attractions/{id}
 * All fields optional for partial update
 */
export interface UpdateAttractionCommand {
  name?: string;
  description?: string | null;
  latitude?: number;
  longitude?: number;
  estimatedDuration?: number | null;
  imageUrl?: string | null;
}

// =============================================================================
// TRIP COMMAND MODELS
// =============================================================================

/**
 * Request body for POST /api/trips
 * Derived from: Trip entity (omitting auto-generated fields)
 */
export interface CreateTripCommand {
  name: string;
  locationId?: UUID | null;
  dailyHours: number;
  maxExtensionHours: number;
  startTime: TimeString;
}

/**
 * Request body for PUT /api/trips/{id}
 * Same structure as CreateTripCommand for full update
 */
export interface UpdateTripCommand {
  name: string;
  locationId?: UUID | null;
  dailyHours: number;
  maxExtensionHours: number;
  startTime: TimeString;
}

/**
 * Request body for PATCH /api/trips/{id}/publish
 */
export interface PublishTripCommand {
  isPublic: boolean;
}

// =============================================================================
// TRIP ATTRACTION COMMAND MODELS
// =============================================================================

/**
 * Request body for POST /api/trips/{tripId}/attractions
 */
export interface AddTripAttractionCommand {
  attractionId: UUID;
  dayNumber: number;
  orderIndex: number;
}

/**
 * Request body for PUT /api/trips/{tripId}/attractions/{attractionId}
 */
export interface UpdateTripAttractionCommand {
  dayNumber: number;
  orderIndex: number;
  plannedStartTime?: TimeString | null;
}

/**
 * Single item in reorder request
 */
export interface ReorderAttractionItem {
  attractionId: UUID;
  dayNumber: number;
  orderIndex: number;
}

/**
 * Request body for POST /api/trips/{tripId}/attractions/reorder
 */
export interface ReorderTripAttractionsCommand {
  attractions: ReorderAttractionItem[];
}

// =============================================================================
// ROUTE OPTIMIZATION COMMAND MODELS
// =============================================================================

/**
 * Request body for POST /api/trips/{tripId}/optimize-route
 */
export interface OptimizeRouteCommand {
  startingAttractionId: UUID;
}

// =============================================================================
// QUERY PARAMETER TYPES
// =============================================================================

/**
 * Query parameters for GET /api/locations
 */
export interface LocationsQueryParams {
  search?: string;
  page?: number;
  pageSize?: number;
}

/**
 * Query parameters for GET /api/attractions
 */
export interface AttractionsQueryParams {
  locationId?: UUID;
  search?: string;
  sortBy?: 'rating' | 'name' | 'reviewCount';
  sortOrder?: 'asc' | 'desc';
  isVerified?: boolean;
  page?: number;
  pageSize?: number;
}

/**
 * Query parameters for GET /api/trips
 */
export interface TripsQueryParams {
  locationId?: UUID;
  onlyMine?: boolean;
  onlyPublic?: boolean;
  search?: string;
  page?: number;
  pageSize?: number;
}

// =============================================================================
// UTILITY TYPES
// =============================================================================

/**
 * Extract the response type from a paginated endpoint
 */
export type ExtractPaginatedItem<T> = T extends PaginatedResponse<infer U> ? U : never;

/**
 * Make all properties of T optional except for K
 */
export type PartialExcept<T, K extends keyof T> = Partial<T> & Pick<T, K>;

/**
 * Make specified properties required
 */
export type WithRequired<T, K extends keyof T> = T & { [P in K]-?: T[P] };
