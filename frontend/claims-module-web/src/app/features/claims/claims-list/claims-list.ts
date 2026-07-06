import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { ClaimListItem } from '../../../core/models';
import { ClaimsApiService } from '../../../core/services/claims-api.service';

@Component({
  selector: 'app-claims-list',
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatTableModule,
    RouterLink,
  ],
  templateUrl: './claims-list.html',
  styleUrl: './claims-list.scss',
})
export class ClaimsList implements OnInit {
  private readonly claimsApi = inject(ClaimsApiService);

  readonly displayedColumns = [
    'claimNumber',
    'policyNumber',
    'policyholderName',
    'lossDate',
    'causeOfLoss',
    'status',
    'actions',
  ];
  readonly claims = signal<ClaimListItem[]>([]);
  readonly isLoading = signal(true);
  readonly searchText = signal('');
  readonly statusFilter = signal('all');
  readonly causeFilter = signal('all');

  readonly statuses = computed(() => getUniqueValues(this.claims(), (claim) => claim.status));
  readonly causeOfLossCodes = computed(() =>
    getUniqueValues(this.claims(), (claim) => claim.causeOfLossCode),
  );
  readonly filteredClaims = computed(() => {
    const search = normalize(this.searchText());
    const status = this.statusFilter();
    const cause = this.causeFilter();

    return this.claims().filter((claim) => {
      const matchesSearch =
        search.length === 0 ||
        normalize(claim.claimNumber).includes(search) ||
        normalize(claim.policyNumber).includes(search) ||
        normalize(claim.policyholderName).includes(search);
      const matchesStatus = status === 'all' || claim.status === status;
      const matchesCause = cause === 'all' || claim.causeOfLossCode === cause;

      return matchesSearch && matchesStatus && matchesCause;
    });
  });

  ngOnInit(): void {
    this.claimsApi
      .getClaims()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (claims) => this.claims.set(claims),
        error: () => this.claims.set([]),
      });
  }

  setSearchText(value: string): void {
    this.searchText.set(value);
  }

  setStatusFilter(value: string): void {
    this.statusFilter.set(value);
  }

  setCauseFilter(value: string): void {
    this.causeFilter.set(value);
  }
}

function normalize(value: string): string {
  return value.trim().toLowerCase();
}

function getUniqueValues(
  claims: ClaimListItem[],
  selector: (claim: ClaimListItem) => string,
): string[] {
  return [...new Set(claims.map(selector))].sort((left, right) => left.localeCompare(right));
}
