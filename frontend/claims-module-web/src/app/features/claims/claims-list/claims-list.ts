import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
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
    MatIconModule,
    MatProgressSpinnerModule,
    MatTableModule,
    RouterLink,
  ],
  templateUrl: './claims-list.html',
  styleUrl: './claims-list.scss',
})
export class ClaimsList implements OnInit {
  private readonly claimsApi = inject(ClaimsApiService);

  readonly displayedColumns = ['claimNumber', 'lossDate', 'status', 'actions'];
  readonly claims = signal<ClaimListItem[]>([]);
  readonly isLoading = signal(true);

  ngOnInit(): void {
    this.claimsApi
      .getClaims()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (claims) => this.claims.set(claims),
        error: () => this.claims.set([]),
      });
  }
}
