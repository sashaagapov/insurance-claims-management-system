export type PolicyStatus = 'Active' | 'Expired' | 'Cancelled';

export interface Policy {
  id: string;
  policyNumber: string;
  policyholderName: string;
  productType: string;
  effectiveFrom: string;
  effectiveTo: string;
  status: PolicyStatus;
  coverageLimit: number;
  currency: string;
}
