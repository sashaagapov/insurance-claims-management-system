import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  ApproveReserveRequest,
  ApproveReserveResponse,
  ClaimDetail,
  ClaimListItem,
  CreateClaimRequest,
  CreateClaimResponse,
  CreateReserveRequest,
  CreateReserveResponse,
  RejectReserveRequest,
  RejectReserveResponse,
  UpdateClaimStatusRequest,
  UpdateClaimStatusResponse,
} from '../models';

@Injectable({
  providedIn: 'root',
})
export class ClaimsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/claims`;

  getClaims(): Observable<ClaimListItem[]> {
    return this.http.get<ClaimListItem[]>(this.baseUrl);
  }

  getClaimById(claimId: string): Observable<ClaimDetail> {
    return this.http.get<ClaimDetail>(`${this.baseUrl}/${claimId}`);
  }

  createClaim(request: CreateClaimRequest): Observable<CreateClaimResponse> {
    return this.http.post<CreateClaimResponse>(this.baseUrl, request);
  }

  updateClaimStatus(
    claimId: string,
    request: UpdateClaimStatusRequest,
  ): Observable<UpdateClaimStatusResponse> {
    return this.http.patch<UpdateClaimStatusResponse>(`${this.baseUrl}/${claimId}/status`, request);
  }

  createReserve(claimId: string, request: CreateReserveRequest): Observable<CreateReserveResponse> {
    return this.http.post<CreateReserveResponse>(`${this.baseUrl}/${claimId}/reserves`, request);
  }

  approveReserve(
    claimId: string,
    reserveId: string,
    request: ApproveReserveRequest,
  ): Observable<ApproveReserveResponse> {
    return this.http.patch<ApproveReserveResponse>(
      `${this.baseUrl}/${claimId}/reserves/${reserveId}/approve`,
      request,
    );
  }

  rejectReserve(
    claimId: string,
    reserveId: string,
    request: RejectReserveRequest,
  ): Observable<RejectReserveResponse> {
    return this.http.patch<RejectReserveResponse>(
      `${this.baseUrl}/${claimId}/reserves/${reserveId}/reject`,
      request,
    );
  }
}
