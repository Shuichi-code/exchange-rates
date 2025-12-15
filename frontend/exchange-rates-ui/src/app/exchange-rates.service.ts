import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { ExchangeRatesResponse } from './models';

@Injectable({
  providedIn: 'root'
})
export class ExchangeRatesService {
  private readonly latestEndpoint = `${environment.apiBaseUrl}/api/exchange-rates/latest`;

  constructor(private http: HttpClient) {}

  getLatestRates(): Observable<ExchangeRatesResponse> {
    return this.http.get<ExchangeRatesResponse>(this.latestEndpoint);
  }
}
