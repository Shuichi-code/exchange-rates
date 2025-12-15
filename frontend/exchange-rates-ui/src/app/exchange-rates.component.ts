import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExchangeRatesService } from './exchange-rates.service';
import { ExchangeRate } from './models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './exchange-rates.component.html',
  styleUrls: ['./exchange-rates.component.css']
})
export class ExchangeRatesComponent implements OnInit {
  loading = false;
  error: string | null = null;
  asOfDate: string | null = null;
  baseCurrency: string | null = null;
  searchTerm = '';
  rates: ExchangeRate[] = [];

  constructor(private exchangeRatesService: ExchangeRatesService) {}

  ngOnInit(): void {
    this.fetchRates();
  }

  get filteredRates(): ExchangeRate[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return this.rates;
    }

    return this.rates.filter(rate =>
      rate.code.toLowerCase().includes(term) ||
      rate.country.toLowerCase().includes(term) ||
      rate.currencyName.toLowerCase().includes(term)
    );
  }

  private fetchRates(): void {
    this.loading = true;
    this.error = null;

    this.exchangeRatesService.getLatestRates().subscribe({
      next: response => {
        this.loading = false;
        this.asOfDate = response.asOfDate;
        this.baseCurrency = response.baseCurrency;
        this.rates = [...response.rates].sort((a, b) => a.code.localeCompare(b.code));
      },
      error: err => {
        this.loading = false;
        const statusPart = err?.status ? ` (HTTP ${err.status})` : '';
        const message = err?.message ? ` - ${err.message}` : '';
        this.error = `Failed to load exchange rates${statusPart}${message}`;
      }
    });
  }
}
