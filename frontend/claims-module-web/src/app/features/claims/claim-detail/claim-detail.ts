import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ReactiveFormsModule, UntypedFormBuilder, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { catchError, finalize, of, switchMap } from 'rxjs';

import { AuditLogEntry, ClaimDetail as ClaimDetailModel } from '../../../core/models';
import { AuthContextService } from '../../../core/services/auth-context.service';
import { ClaimsApiService } from '../../../core/services/claims-api.service';

@Component({
  selector: 'app-claim-detail',
  imports: [
    CurrencyPipe,
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSnackBarModule,
    MatTableModule,
    MatTabsModule,
    ReactiveFormsModule,
    RouterLink,
  ],
  templateUrl: './claim-detail.html',
  styleUrl: './claim-detail.scss',
})
export class ClaimDetail implements OnInit {
  private readonly fb = inject(UntypedFormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly claimsApi = inject(ClaimsApiService);
  private readonly snackBar = inject(MatSnackBar);
  readonly authContext = inject(AuthContextService);

  readonly claim = signal<ClaimDetailModel | null>(null);
  readonly currentClaimId = signal<string | null>(null);
  readonly isLoading = signal(true);
  readonly isCreatingReserve = signal(false);
  readonly isChangingStatus = signal(false);
  readonly activeReserveActionId = signal<string | null>(null);
  readonly rejectReserveId = signal<string | null>(null);
  readonly errorMessage = signal<string | null>(null);
  readonly partyColumns = ['partyType', 'fullName', 'email', 'phone'];
  readonly riskObjectColumns = ['objectType', 'externalReference', 'description'];
  readonly reserveColumns = [
    'amount',
    'currency',
    'status',
    'createdAtUtc',
    'glPostingReference',
    'actions',
  ];
  readonly auditColumns = ['createdAtUtc', 'action', 'actorUserId', 'details'];
  readonly auditLogEntries = computed(() => sortAuditEntries(this.claim()?.auditLogEntries ?? []));
  readonly reserveSummary = computed(() => {
    const reserves = this.claim()?.reserves ?? [];

    return reserves.reduce(
      (summary, reserve) => ({
        total: summary.total + reserve.amount,
        pendingApproval:
          summary.pendingApproval + (reserve.status === 'PendingApproval' ? reserve.amount : 0),
        approved: summary.approved + (reserve.status === 'Approved' ? reserve.amount : 0),
      }),
      { total: 0, pendingApproval: 0, approved: 0 },
    );
  });
  readonly canManageReserves = computed(() => {
    const role = this.authContext.activeUser().role;
    return role === 'Supervisor' || role === 'Manager';
  });
  readonly reserveForm = this.fb.group({
    amount: [1000, [Validators.required, Validators.min(0.01)]],
    currency: ['USD', [Validators.required, Validators.pattern(/^[A-Z]{3}$/)]],
  });

  readonly rejectForm = this.fb.group({
    reason: ['', [Validators.required, Validators.maxLength(1000)]],
  });

  ngOnInit(): void {
    this.route.paramMap
      .pipe(
        switchMap((params) => {
          const claimId = params.get('id');
          if (!claimId) {
            this.errorMessage.set('Claim id is missing.');
            this.isLoading.set(false);
            return of(null);
          }

          this.currentClaimId.set(claimId);
          return this.loadClaim(claimId);
        }),
      )
      .subscribe((claim) => this.claim.set(claim));
  }

  createReserve(): void {
    const claimId = this.currentClaimId();
    if (!claimId || this.reserveForm.invalid) {
      this.reserveForm.markAllAsTouched();
      return;
    }

    this.isCreatingReserve.set(true);
    this.claimsApi
      .createReserve(claimId, {
        amount: Number(this.reserveForm.get('amount')?.value),
        currency: String(this.reserveForm.get('currency')?.value).trim().toUpperCase(),
        createdByUserId: this.authContext.activeUser().id,
      })
      .pipe(finalize(() => this.isCreatingReserve.set(false)))
      .subscribe({
        next: (response) => {
          this.snackBar.open(`Reserve ${response.status}.`, 'Dismiss', { duration: 4000 });
          this.reserveForm.patchValue({ amount: 1000, currency: 'USD' });
          this.reloadClaim();
        },
      });
  }

  reserveAuthorityPreview(): string {
    const amount = Number(this.reserveForm.get('amount')?.value ?? 0);
    return amount > 10000 ? 'Requires supervisor or manager approval' : 'Auto-approved at submit';
  }

  approveReserve(reserveId: string): void {
    const claimId = this.currentClaimId();
    if (!claimId) {
      return;
    }

    this.activeReserveActionId.set(reserveId);
    this.claimsApi
      .approveReserve(claimId, reserveId, { actorUserId: this.authContext.activeUser().id })
      .pipe(finalize(() => this.activeReserveActionId.set(null)))
      .subscribe({
        next: () => {
          this.snackBar.open('Reserve approved.', 'Dismiss', { duration: 4000 });
          this.reloadClaim();
        },
      });
  }

  startRejectReserve(reserveId: string): void {
    this.rejectReserveId.set(reserveId);
    this.rejectForm.reset({ reason: '' });
  }

  cancelRejectReserve(): void {
    this.rejectReserveId.set(null);
    this.rejectForm.reset({ reason: '' });
  }

  rejectReserve(reserveId: string): void {
    const claimId = this.currentClaimId();
    if (!claimId || this.rejectForm.invalid) {
      this.rejectForm.markAllAsTouched();
      return;
    }

    this.activeReserveActionId.set(reserveId);
    this.claimsApi
      .rejectReserve(claimId, reserveId, {
        actorUserId: this.authContext.activeUser().id,
        reason: this.rejectForm.get('reason')?.value,
      })
      .pipe(finalize(() => this.activeReserveActionId.set(null)))
      .subscribe({
        next: () => {
          this.snackBar.open('Reserve rejected.', 'Dismiss', { duration: 4000 });
          this.cancelRejectReserve();
          this.reloadClaim();
        },
      });
  }

  transitionToUnderInvestigation(): void {
    const claim = this.claim();
    if (!claim || claim.status !== 'Open') {
      return;
    }

    this.isChangingStatus.set(true);
    this.claimsApi
      .updateClaimStatus(claim.claimId, {
        newStatus: 'UnderInvestigation',
        actorUserId: this.authContext.activeUser().id,
      })
      .pipe(finalize(() => this.isChangingStatus.set(false)))
      .subscribe({
        next: () => {
          this.snackBar.open('Claim moved to Under Investigation.', 'Dismiss', { duration: 4000 });
          this.reloadClaim();
        },
      });
  }

  private reloadClaim(): void {
    const claimId = this.currentClaimId();
    if (!claimId) {
      return;
    }

    this.loadClaim(claimId).subscribe((claim) => this.claim.set(claim));
  }

  private loadClaim(claimId: string) {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.claim.set(null);

    return this.claimsApi.getClaimById(claimId).pipe(
      catchError((error: unknown) => {
        this.errorMessage.set(getClaimLoadErrorMessage(error));
        return of(null);
      }),
      finalize(() => this.isLoading.set(false)),
    );
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
