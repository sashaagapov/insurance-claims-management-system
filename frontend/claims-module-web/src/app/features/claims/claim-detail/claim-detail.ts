import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { catchError, finalize, of, switchMap } from 'rxjs';

import { AuditLogEntry, ClaimDetail as ClaimDetailModel } from '../../../core/models';
import { ClaimsApiService } from '../../../core/services/claims-api.service';

@Component({
  selector: 'app-claim-detail',
  imports: [
    CurrencyPipe,
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatTabsModule,
    RouterLink,
  ],
  templateUrl: './claim-detail.html',
  styleUrl: './claim-detail.scss',
})
export class ClaimDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly claimsApi = inject(ClaimsApiService);

  readonly claim = signal<ClaimDetailModel | null>(null);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly partyColumns = ['partyType', 'fullName', 'email', 'phone'];
  readonly riskObjectColumns = ['objectType', 'externalReference', 'description'];
  readonly reserveColumns = ['amount', 'currency', 'status', 'createdAtUtc', 'glPostingReference'];
  readonly auditColumns = ['createdAtUtc', 'action', 'actorUserId', 'details'];
  readonly auditLogEntries = computed(() => sortAuditEntries(this.claim()?.auditLogEntries ?? []));

  ngOnInit(): void {
    this.route.paramMap
      .pipe(
        switchMap((params) => {
          this.isLoading.set(true);
          this.errorMessage.set(null);
          this.claim.set(null);

          const claimId = params.get('id');
          if (!claimId) {
            this.errorMessage.set('Claim id is missing.');
            this.isLoading.set(false);
            return of(null);
          }

          return this.claimsApi.getClaimById(claimId).pipe(
            catchError((error: unknown) => {
              this.errorMessage.set(getClaimLoadErrorMessage(error));
              return of(null);
            }),
            finalize(() => this.isLoading.set(false)),
          );
        }),
      )
      .subscribe((claim) => this.claim.set(claim));
  }
}

function getClaimLoadErrorMessage(error: unknown): string {
  if (typeof error === 'object' && error && 'status' in error && error.status === 404) {
    return 'Claim not found.';
  }

  return 'Unable to load claim details.';
}

function sortAuditEntries(entries: AuditLogEntry[]): AuditLogEntry[] {
  return [...entries].sort((left, right) => right.createdAtUtc.localeCompare(left.createdAtUtc));
}
