import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { ExchangeRatesComponent } from './app/exchange-rates.component';

bootstrapApplication(ExchangeRatesComponent, {
  providers: [provideHttpClient()]
}).catch(err => console.error(err));
