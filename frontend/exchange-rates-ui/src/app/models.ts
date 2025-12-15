export interface ExchangeRate {
  country: string;
  currencyName: string;
  amount: number;
  code: string;
  rateCzk: number;
}

export interface ExchangeRatesResponse {
  asOfDate: string;
  baseCurrency: string;
  rates: ExchangeRate[];
}
