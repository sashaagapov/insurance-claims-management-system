import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Policy } from '../models';

@Injectable({
  providedIn: 'root',
})
export class PoliciesApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/policies`;

  getPolicies(): Observable<Policy[]> {
    return this.http.get<Policy[]>(this.baseUrl);
  }
}
