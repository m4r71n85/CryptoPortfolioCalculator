import { Component, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { interval, Subscription } from 'rxjs';

interface PortfolioSummary {
  items: PortfolioItem[];
  initialPortfolioValue: number;
  currentPortfolioValue: number;
  overallChange: number;
}

interface PortfolioItem {
  amount: number;
  coin: string;
  initialPrice: number;
  currentPrice?: number;
  change?: number;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnDestroy {
  errorMessage: string | null = null;
  portfolioSummary: PortfolioSummary | null = null;
  private refreshSubscription: Subscription | null = null;
  refreshInterval: number = 5; // Default 5 minutes
  lastUpdated: Date | null = null;
  selectedFile: File | null = null;
  isFileUploaded: boolean = false;
  baseUrl: string = "https://localhost:7208/api/";

  constructor(private http: HttpClient) { }

  ngOnDestroy() {
    this.stopRefresh();
  }

  onFileSelected(event: Event) {
    this.errorMessage = null;
    const element = event.currentTarget as HTMLInputElement;
    let fileList: FileList | null = element.files;
    if (fileList) {
      this.selectedFile = fileList[0];
    }
  }

  uploadFile() {


    if (this.selectedFile) {
      const formData = new FormData();
      formData.append('file', this.selectedFile);

      this.http.post<PortfolioSummary>(this.baseUrl + 'Portfolio/calculate', formData).subscribe(
        (data) => {
          this.portfolioSummary = data;
          this.lastUpdated = new Date();
          this.isFileUploaded = true;
          this.startRefresh();
        },
        (error) => {
          console.error('Error calculating portfolio:', error);
          this.errorMessage = error.error.Message;
        }
      );
    }
  }

  calculatePortfolio(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    this.http.post<PortfolioSummary>(this.baseUrl + 'Portfolio/calculate', formData).subscribe(
      (data) => {
        this.portfolioSummary = data;
        this.lastUpdated = new Date();
        this.startRefresh();
      },
      (error) => {
        console.error('Error calculating portfolio:', error);
      }
    );
  }

  startRefresh() {
    this.stopRefresh(); // Stop any existing refresh
    this.refreshSubscription = interval(this.refreshInterval * 60 * 1000).subscribe(() => {
      if (this.portfolioSummary && this.selectedFile) {
        this.refreshPortfolio();
      }
    });
  }

  stopRefresh() {
    if (this.refreshSubscription) {
      this.refreshSubscription.unsubscribe();
      this.refreshSubscription = null;
    }
  }

  refreshPortfolio() {
    if (this.selectedFile) {
      this.calculatePortfolio(this.selectedFile);
    } else {
      console.error('No file to refresh with');
    }
  }

  onRefreshIntervalChange(event: Event) {
    const target = event.target as HTMLInputElement;
    const value = parseInt(target.value, 10);
    if (!isNaN(value) && value > 0) {
      this.setRefreshInterval(value);
    }
  }

  setRefreshInterval(minutes: number) {
    this.refreshInterval = minutes;
    this.startRefresh();
  }
}
