import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { CauseOfLossCode } from '../models';

@Injectable({
  providedIn: 'root',
})
export class ReferenceDataApiService {
  private readonly http = inject(HttpClient);
  private readonly causeOfLossCodesUrl = `${environment.apiBaseUrl}/api/cause-of-loss-codes`;

  getCauseOfLossCodes(): Observable<CauseOfLossCode[]> {
    return this.http.get<CauseOfLossCode[]>(this.causeOfLossCodesUrl);
  }
}
