import { inject, Provider } from '@angular/core';
import { Branch } from '../models/domain.models';
import { BranchApi } from '../api/api-client';
import { signalStore, patchState, withMethods, withState } from '@ngrx/signals';

interface AppState {
  branches: Branch[];
  branchesLoaded: boolean;
}

const initialState: AppState = {
  branches: [],
  branchesLoaded: false
};

export const AppStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store, branchApi = inject(BranchApi)) => ({
    loadBranches() {
      if (store.branchesLoaded()) {
        return;
      }
      branchApi.list().subscribe((branches) => {
        patchState(store, { branches, branchesLoaded: true });
      });
    }
  }))
);

export function provideAppStore(): Provider[] {
  return [AppStore];
}
