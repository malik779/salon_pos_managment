import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

type SyncStatus = 'online' | 'syncing' | 'offline';

@Injectable({ providedIn: 'root' })
export class SyncStatusService {
  private readonly subject = new BehaviorSubject<SyncStatus>('online');
  readonly status$ = this.subject.asObservable();

  setSyncing() {
    this.subject.next('syncing');
  }

  setOnline() {
    this.subject.next('online');
  }

  setOffline() {
    this.subject.next('offline');
  }
}
