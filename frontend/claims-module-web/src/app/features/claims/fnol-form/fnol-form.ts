import { Component, OnInit, inject, signal } from '@angular/core';
import {
  AbstractControl,
  ReactiveFormsModule,
  UntypedFormArray,
  UntypedFormBuilder,
  UntypedFormGroup,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatStepperModule } from '@angular/material/stepper';
import { Router, RouterLink } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';

import {
  CauseOfLossCode,
  CreateClaimParty,
  CreateClaimRequest,
  CreateClaimRiskObject,
  PartyType,
  Policy,
  RiskObjectType,
} from '../../../core/models';
import { ClaimsApiService } from '../../../core/services/claims-api.service';
import { AuthContextService } from '../../../core/services/auth-context.service';
import { PoliciesApiService } from '../../../core/services/policies-api.service';
import { ReferenceDataApiService } from '../../../core/services/reference-data-api.service';

@Component({
  selector: 'app-fnol-form',
  imports: [
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSnackBarModule,
    MatStepperModule,
    ReactiveFormsModule,
    RouterLink,
  ],
  templateUrl: './fnol-form.html',
  styleUrl: './fnol-form.scss',
})
export class FnolForm implements OnInit {
  private readonly fb = inject(UntypedFormBuilder);
  private readonly claimsApi = inject(ClaimsApiService);
  private readonly policiesApi = inject(PoliciesApiService);
  private readonly referenceDataApi = inject(ReferenceDataApiService);
  readonly authContext = inject(AuthContextService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  readonly partyTypes: PartyType[] = ['Policyholder', 'Claimant', 'Insured', 'ThirdParty', 'Witness'];
  readonly riskObjectTypes: RiskObjectType[] = ['Vehicle', 'Property', 'Person', 'Other'];
  readonly policies = signal<Policy[]>([]);
  readonly causeOfLossCodes = signal<CauseOfLossCode[]>([]);
  readonly isLoadingReferenceData = signal(true);
  readonly isSubmitting = signal(false);

  readonly form = this.fb.group({
    claimInformation: this.fb.group({
      policyId: ['', Validators.required],
      causeOfLossCodeId: ['', Validators.required],
      lossDate: [todayIsoDate(), [Validators.required, notFutureDateValidator]],
      description: ['', [Validators.required, Validators.maxLength(2000)]],
    }),
    parties: this.fb.array([this.createPartyGroup()]),
    riskObjects: this.fb.array([this.createRiskObjectGroup()]),
  });

  ngOnInit(): void {
    forkJoin({
      policies: this.policiesApi.getPolicies(),
      causeOfLossCodes: this.referenceDataApi.getCauseOfLossCodes(),
    })
      .pipe(finalize(() => this.isLoadingReferenceData.set(false)))
      .subscribe({
        next: ({ policies, causeOfLossCodes }) => {
          this.policies.set(policies);
          this.causeOfLossCodes.set(causeOfLossCodes);
        },
        error: () => {
          this.policies.set([]);
          this.causeOfLossCodes.set([]);
        },
      });
  }

  get claimInformation(): UntypedFormGroup {
    return this.form.get('claimInformation') as UntypedFormGroup;
  }

  get parties(): UntypedFormArray {
    return this.form.get('parties') as UntypedFormArray;
  }

  get riskObjects(): UntypedFormArray {
    return this.form.get('riskObjects') as UntypedFormArray;
  }

  addParty(): void {
    this.parties.push(this.createPartyGroup());
  }

  removeParty(index: number): void {
    if (this.parties.length > 1) {
      this.parties.removeAt(index);
    }
  }

  addRiskObject(): void {
    this.riskObjects.push(this.createRiskObjectGroup());
  }

  removeRiskObject(index: number): void {
    if (this.riskObjects.length > 1) {
      this.riskObjects.removeAt(index);
    }
  }

  selectedPolicy(): Policy | undefined {
    return this.policies().find((policy) => policy.id === this.claimInformation.get('policyId')?.value);
  }

  selectedCauseOfLoss(): CauseOfLossCode | undefined {
    return this.causeOfLossCodes().find(
      (cause) => cause.id === this.claimInformation.get('causeOfLossCodeId')?.value,
    );
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.snackBar.open('Complete the required FNOL fields before submitting.', 'Dismiss', {
        duration: 4000,
      });
      return;
    }

    const request = this.toCreateClaimRequest();
    this.isSubmitting.set(true);

    this.claimsApi
      .createClaim(request)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (response) => {
          this.snackBar.open(`Claim ${response.claimNumber} created.`, 'Dismiss', {
            duration: 5000,
          });
          void this.router.navigate(['/claims', response.claimId]);
        },
      });
  }

  private createPartyGroup(): UntypedFormGroup {
    return this.fb.group({
      partyType: ['Claimant' satisfies PartyType, Validators.required],
      fullName: ['', [Validators.required, Validators.maxLength(200)]],
      email: ['', [Validators.email, Validators.maxLength(250)]],
      phone: ['', Validators.maxLength(50)],
      notes: ['', Validators.maxLength(1000)],
    });
  }

  private createRiskObjectGroup(): UntypedFormGroup {
    return this.fb.group({
      objectType: ['Vehicle' satisfies RiskObjectType, Validators.required],
      externalReference: ['', Validators.maxLength(100)],
      description: ['', [Validators.required, Validators.maxLength(1000)]],
    });
  }

  private toCreateClaimRequest(): CreateClaimRequest {
    const claimInformation = this.claimInformation.getRawValue();
    const parties = this.parties.getRawValue() as CreateClaimParty[];
    const riskObjects = this.riskObjects.getRawValue() as CreateClaimRiskObject[];

    return {
      policyId: claimInformation.policyId,
      causeOfLossCodeId: claimInformation.causeOfLossCodeId,
      lossDate: claimInformation.lossDate,
      description: claimInformation.description,
      createdByUserId: this.authContext.activeUser().id,
      parties: parties.map((party) => ({
        partyType: party.partyType,
        fullName: party.fullName,
        email: toNullableString(party.email),
        phone: toNullableString(party.phone),
        notes: toNullableString(party.notes),
      })),
      riskObjects: riskObjects.map((riskObject) => ({
        objectType: riskObject.objectType,
        externalReference: toNullableString(riskObject.externalReference),
        description: riskObject.description,
      })),
    };
  }
}

function todayIsoDate(): string {
  return new Date().toISOString().slice(0, 10);
}

function notFutureDateValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value as string | null;
  if (!value) {
    return null;
  }

  return value > todayIsoDate() ? { futureDate: true } : null;
}

function toNullableString(value: string | null | undefined): string | null {
  const normalized = value?.trim() ?? '';
  return normalized.length > 0 ? normalized : null;
}
