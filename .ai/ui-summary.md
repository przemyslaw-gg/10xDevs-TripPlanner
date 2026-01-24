# TripPlanner - UI Architecture Summary

## Overview

This document summarizes all UI architecture decisions for the TripPlanner MVP, based on the Q&A session covering 100 questions.

---

## 1. General Constraints

| Constraint | Decision |
|------------|----------|
| Trip duration | Single day only (no multi-day support) |
| Map integrations | None |
| Offline support | None - no internet = no app |
| Mobile support | Desktop/tablet only for MVP |
| Global search | Not implemented |
| Onboarding | Not implemented |
| Timezone handling | Not needed (single timezone assumed) |

---

## 2. Technology Stack

| Category | Choice |
|----------|--------|
| Frontend framework | React with TypeScript |
| UI Component library | shadcn/ui |
| State management | React Query/TanStack Query |
| Icons | Lucide Icons |
| Typography | Inter (14px base, 16px for important elements) |
| Color scheme | Primary blue, shadcn/ui default palette |

---

## 3. Navigation & Layout

| Element | Decision |
|---------|----------|
| Navigation type | Top horizontal navbar |
| Header | Sticky during scroll |
| Breadcrumbs | Yes - Home > Section > Detail |
| Dark mode | Not for MVP |
| Language (i18n) | Polish only for MVP |

### Main Navigation Items
- Home / Dashboard
- Browse Attractions
- My Trips
- Profile/Settings

---

## 4. Data Display

| Element | Decision |
|---------|----------|
| Estimated duration | Not displayed |
| Rating | Not displayed |
| Review count | Not displayed |
| Sorting | Not implemented |
| Filtering | Server-side |
| Pagination | Classic pagination (not infinite scroll) |
| Lists display | Cards (not tables) |

---

## 5. User Authentication

| Feature | Decision |
|---------|----------|
| Login form | Email + password |
| Registration | Email + password + password confirmation |
| "Remember me" | Yes - checkbox for longer session |
| Password requirements | Min 8 chars, 1 uppercase, 1 digit, strength indicator |
| Email confirmation | Not required |
| Password reset | Not implemented |
| Login attempt limits | Not implemented |
| Session expiry | Redirect to login with message, preserve target URL |
| Refresh token | Yes - silent refresh before expiry |
| Account deletion | Yes - in settings with password confirmation |
| Terms of service | Not required |

---

## 6. Trip Management

| Feature | Decision |
|---------|----------|
| Trip name max length | 100 characters (show counter from 80+) |
| Max attractions per trip | 20 |
| Attraction ordering | Yes - drag & drop or up/down buttons |
| Notes per attraction | Not implemented |
| Duplicate trip | Not implemented |
| Export to PDF | Not implemented |
| Share trip | Not implemented for MVP |
| Autosave | Yes - with 2s debounce, status indicator |

---

## 7. Forms & Validation

| Aspect | Decision |
|--------|----------|
| Validation timing | On blur + on submit |
| Error display | Inline under fields + summary for critical errors |
| Field error styling | Red border around field |
| Clear error | When user starts correcting |

---

## 8. Feedback & Notifications

| Element | Decision |
|---------|----------|
| Toast notifications | Bottom-right corner |
| Toast types | Success (green), Error (red), Info (blue) |
| Toast duration | 5s auto-dismiss for success/info, manual for errors |
| Confirmation dialogs | For destructive actions (delete trip, delete attraction) |
| Undo/Redo | Not for MVP |

---

## 9. Loading States

| Context | Approach |
|---------|----------|
| Initial page load | Skeleton loaders |
| List loading | Skeleton loaders |
| Action processing | Spinner |
| Saving | "Saving..." / "Saved" indicator |

---

## 10. Empty States

| Context | Message & CTA |
|---------|---------------|
| No trips | "You don't have any trips yet" + "Create first trip" button |
| No attractions in trip | "Add attractions to your trip" |
| No search results | "No attractions found" |

---

## 11. Error Pages

| Page | Content |
|------|---------|
| 404 | Friendly message + link to home |
| 500 | "Something went wrong" + "Try again" button + home link |

---

## 12. Component Structure (Feature-based)

```
src/
├── components/
│   ├── common/           # Shared components
│   │   ├── Button/
│   │   ├── Card/
│   │   ├── Modal/
│   │   ├── Toast/
│   │   └── Skeleton/
│   ├── layout/           # Layout components
│   │   ├── Header/
│   │   ├── Footer/
│   │   └── Breadcrumbs/
│   └── features/         # Feature-specific components
│       ├── auth/
│       │   ├── LoginForm/
│       │   ├── RegisterForm/
│       │   └── ProfileSettings/
│       ├── trips/
│       │   ├── TripList/
│       │   ├── TripCard/
│       │   ├── TripDetail/
│       │   ├── TripForm/
│       │   └── TripAttractionList/
│       ├── attractions/
│       │   ├── AttractionList/
│       │   ├── AttractionCard/
│       │   ├── AttractionDetail/
│       │   └── AttractionForm/
│       └── locations/
│           ├── LocationList/
│           └── LocationCard/
├── pages/                # Route pages
├── hooks/                # Custom hooks
├── services/             # API services
├── utils/                # Utility functions
└── types/                # TypeScript types
```

---

## 13. API Integration

| Aspect | Decision |
|--------|----------|
| Data fetching | React Query/TanStack Query |
| Cache staleTime | 5 minutes for attraction lists |
| Refetch strategy | On window focus for user data |
| Optimistic updates | Yes for CRUD operations |

---

## 14. Animations

| Element | Approach |
|---------|----------|
| Transitions | Subtle 150-200ms |
| Hover states | Yes |
| Modal open/close | Yes |
| Toast appearance | Yes |
| Heavy animations | Avoid - performance over effects |

---

## 15. Accessibility (a11y)

| Requirement | Implementation |
|-------------|----------------|
| Semantic HTML | button, nav, main, section, etc. |
| ARIA labels | For icons without text |
| Focus visible | For keyboard navigation |
| Color contrast | WCAG AA compliant |
| Image alt text | Required for attraction images |

---

## 16. Branding

| Element | Decision |
|---------|----------|
| Favicon | Yes - simple icon (16x16, 32x32) |
| Logo | Yes - minimalist for header |
| Analytics | Not integrated for MVP |

---

## 17. Key User Flows

### 17.1 Registration Flow
1. User clicks "Register"
2. Fills form (email, password, confirm password)
3. Submits → Account created → Redirect to dashboard

### 17.2 Login Flow
1. User clicks "Login"
2. Fills form (email, password, optional "Remember me")
3. Submits → Redirect to dashboard (or previous target URL)

### 17.3 Create Trip Flow
1. User clicks "Create Trip"
2. Enters trip name and date
3. Saves → Redirect to trip detail
4. Browses attractions → Adds to trip
5. Reorders attractions as needed

### 17.4 Browse Attractions Flow
1. User navigates to "Browse Attractions"
2. Selects location (filter)
3. Views paginated list of attractions
4. Clicks attraction → Views detail
5. Optionally adds to existing trip

---

## Summary of Excluded Features (for MVP)

- Multi-day trips
- Map integrations
- Offline support
- Mobile optimization
- Global search
- Onboarding flow
- Estimated duration display
- Rating/review count display
- Sorting
- Notes per attraction
- Duplicate trip
- Export to PDF
- Trip sharing (public links)
- Copy link button
- Password reset
- Email confirmation
- Login attempt limits
- Terms of service
- Analytics
- Dark mode
- i18n (multiple languages)
- Undo/Redo

---

*Document generated: 2026-01-24*
