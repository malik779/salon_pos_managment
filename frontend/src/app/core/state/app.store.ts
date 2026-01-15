import { inject, ProviderToken } from '@angular/core';
import { PaginatedResponse } from '../models/domain.models';
import { EntityApi } from '../api/api-client';
import { signalStore, patchState, withMethods, withState } from '@ngrx/signals';
import { catchError, of, tap } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

export interface PaginationMetadata {
  page: number;
  pageSize: number;
  total: number;
  totalPages?: number;
}

export interface EntityState<T> {
  items: T[];
  loaded: boolean;
  loading: boolean;
  error: string | null;
  pagination: PaginationMetadata | null;
  allItems: T[] | null;
  allItemsLoaded: boolean;
  allItemsLoading: boolean;
}

export function createInitialEntityState<T>(): EntityState<T> {
  return {
    items: [],
    loaded: false,
    loading: false,
    error: null,
    pagination: null,
    allItems: null,
    allItemsLoaded: false,
    allItemsLoading: false
  };
}

export function createEntityStore<T, TId = string>(
  apiToken: ProviderToken<EntityApi<T,TId>>,
  getId: (entity: T) => TId
) {
  return signalStore(
    { providedIn: 'root' },

    withState({
      items: [] as T[],
      loaded: false,
      loading: false,
      saving: false,
      error: null as string | null,
      pagination: null as PaginationMetadata | null,
      allItems: null as T[] | null,
      allItemsLoaded: false,
      allItemsLoading: false
    }),

    withMethods((store, api = inject(apiToken)) => {
      let lastLoadParams: { page: number; pageSize: number; searchTerm: string } | null = null;

      return {
        load(page: number=1, pageSize: number=10, searchTerm: string='') {
          if (store.loaded()) return;

          lastLoadParams = { page, pageSize, searchTerm };
          patchState(store, { loading: true, error: null });

          api.paginatedList(page, pageSize, searchTerm)
            .pipe(
              tap((response: PaginatedResponse<T>) => {
                patchState(store, {
                  items: response.items ?? [],
                  pagination: {
                    page: response.pageNumber ?? page,
                    pageSize: response.pageSize ?? pageSize,
                    total: response.totalCount ?? 0,
                    totalPages: response.totalPages ?? Math.ceil((response.totalCount ?? 0) / (response.pageSize ?? pageSize))
                  },
                  loaded: true,
                  loading: false,
                  error: null
                });
              }),
              catchError((error: HttpErrorResponse | Error) => {
                const errorMessage = error instanceof HttpErrorResponse
                  ? error.error?.message ?? error.message ?? 'Failed to load items'
                  : error.message ?? 'Failed to load items';

                patchState(store, {
                  error: errorMessage,
                  loading: false,
                  loaded: false
                });

                return of(null);
              })
            )
            .subscribe();
        },

        reload() {
          patchState(store, { loaded: false });
          if (lastLoadParams) {
            this.load(lastLoadParams.page, lastLoadParams.pageSize, lastLoadParams.searchTerm);
          }
        },

        clear() {
          patchState(store, createInitialEntityState<T>());
        },

        /**
         * Loads all records without pagination
         * Useful for dropdowns, selects, and other UI components
         * Implements caching - only fetches if not already loaded
         * Follows Single Responsibility Principle - separate concern from paginated load
         */
        loadAll(searchTerm: string = '') {
          // Return cached data if already loaded and no search term
          if (store.allItemsLoaded() && !searchTerm && store.allItems()) {
            return;
          }

          patchState(store, { allItemsLoading: true, error: null });

          api.getAll(searchTerm)
            .pipe(
              tap((items: T[]) => {
                patchState(store, {
                  allItems: items,
                  allItemsLoaded: true,
                  allItemsLoading: false,
                  error: null
                });
              }),
              catchError((error: HttpErrorResponse | Error) => {
                const errorMessage = error instanceof HttpErrorResponse
                  ? error.error?.message ?? error.message ?? 'Failed to load all items'
                  : error.message ?? 'Failed to load all items';

                patchState(store, {
                  error: errorMessage,
                  allItemsLoading: false,
                  allItemsLoaded: false
                });

                return of(null);
              })
            )
            .subscribe();
        },

        /**
         * Clears the cached all items
         * Useful when you know data has changed and cache should be invalidated
         */
        clearAllItems() {
          patchState(store, {
            allItems: null,
            allItemsLoaded: false
          });
        },

        create(item: T) {
          patchState(store, { saving: true, error: null });

          api.create(item)
            .pipe(
              tap((created: T) => {
                patchState(store, {
                  items: [...store.items(), created],
                  saving: false,
                  error: null
                });
              }),
              catchError((error: HttpErrorResponse | Error) => {
                const errorMessage = error instanceof HttpErrorResponse
                  ? error.error?.message ?? error.message ?? 'Failed to create item'
                  : error.message ?? 'Failed to create item';

                patchState(store, {
                  error: errorMessage,
                  saving: false
                });

                return of(null);
              })
            )
            .subscribe();
        },

      update(item: T) {
        const id = getId(item);

        patchState(store, { saving: true, error: null });

        api.update(id, item)
          .pipe(
            tap((updated: T) => {
              patchState(store, {
                items: store.items().map(i =>
                  getId(i) === id ? updated : i
                ),
                saving: false,
                error: null
              });
            }),
            catchError((error: HttpErrorResponse | Error) => {
              const errorMessage = error instanceof HttpErrorResponse
                ? error.error?.message ?? error.message ?? 'Failed to update item'
                : error.message ?? 'Failed to update item';

              patchState(store, {
                error: errorMessage,
                saving: false
              });

              return of(null);
            })
          )
          .subscribe();
      },

      delete(id: TId) {
        patchState(store, { saving: true, error: null });

        api.delete(id)
          .pipe(
            tap(() => {
              patchState(store, {
                items: store.items().filter(i => getId(i) !== id),
                saving: false,
                error: null
              });
            }),
            catchError((error: HttpErrorResponse | Error) => {
              const errorMessage = error instanceof HttpErrorResponse
                ? error.error?.message ?? error.message ?? 'Failed to delete item'
                : error.message ?? 'Failed to delete item';

              patchState(store, {
                error: errorMessage,
                saving: false
              });

              return of(null);
            })
          )
          .subscribe();
      }
      };
    }));
}
