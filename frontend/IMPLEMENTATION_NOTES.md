# Non-Paginated Records Implementation

## Overview
This implementation follows SOLID principles to support fetching all records without pagination, which is essential for dropdowns, selects, and other UI components.

## SOLID Principles Applied

### 1. **Interface Segregation Principle (ISP)**
- Created separate `ListableApi<T>` interface for entities that support fetching all records
- Clients only depend on what they need - not all entities need to implement `getAll()`
- `EntityApi` remains focused on paginated queries

### 2. **Single Responsibility Principle (SRP)**
- `load()` - handles paginated queries
- `loadAll()` - handles non-paginated queries
- Each method has a single, well-defined responsibility

### 3. **Open/Closed Principle**
- Extended functionality without modifying existing `EntityApi` interface
- New `ListableApi` interface can be optionally implemented
- Existing code remains unchanged

### 4. **Dependency Inversion Principle**
- Store depends on abstractions (`EntityApi`, `ListableApi`)
- Concrete implementations can vary without affecting the store

## Usage Examples

### In API Implementation
```typescript
@Injectable({ providedIn: 'root' })
export class BranchApi implements EntityApi<Branch>, ListableApi<Branch> {
  getAll(searchTerm?: string): Observable<Branch[]> {
    const params = searchTerm ? { searchTerm } : undefined;
    return this.http.get<Branch[]>(`${this.basePath}/all`, params ? { params } : {});
  }
}
```

### In Component
```typescript
// For dropdowns/selects
this.branchStore.loadAll();
const branches = this.branchStore.allItems(); // Returns cached or loads if needed

// With search
this.branchStore.loadAll('search term');

// Clear cache when data changes
this.branchStore.clearAllItems();
```

## Features

1. **Caching**: `loadAll()` caches results to avoid unnecessary API calls
2. **Optional Search**: Supports optional search term filtering
3. **Type Safety**: Full TypeScript support with generics
4. **Error Handling**: Consistent error handling with other store methods
5. **State Management**: Separate state for all items vs paginated items

## Backend Requirements

The backend should implement an endpoint like:
- `GET /api/branches/all?searchTerm=optional` - Returns `Branch[]` (not paginated)

## Benefits

1. **Performance**: Caching reduces API calls for frequently accessed dropdown data
2. **Separation of Concerns**: Paginated and non-paginated queries are separate
3. **Flexibility**: Not all entities need to support `getAll()`
4. **Type Safety**: Compile-time checking ensures correct usage
5. **Maintainability**: Clear separation makes code easier to understand and maintain
