import { Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { ConfigService } from './config.service';

@Injectable({ providedIn: 'root' })
export class SignalRService implements OnDestroy {
  private hub?: signalR.HubConnection;
  private readonly destroyed$ = new Subject<void>();
  readonly bookingUpdates$ = new Subject<unknown>();
  readonly invoiceUpdates$ = new Subject<unknown>();

  constructor(private readonly configService: ConfigService) {}

  connect(token?: string) {
    const hubUrl = this.configService.getSignalrHubUrl();
    this.hub = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, { accessTokenFactory: () => token ?? '' })
      .withAutomaticReconnect()
      .build();

    this.hub.on('BookingUpdated', (payload) => this.bookingUpdates$.next(payload));
    this.hub.on('InvoicePaid', (payload) => this.invoiceUpdates$.next(payload));

    void this.hub.start();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    void this.hub?.stop();
  }
}
