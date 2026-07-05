export type ClaimStatus =
  | 'Open'
  | 'UnderInvestigation'
  | 'PendingPayment'
  | 'Closed'
  | 'Reopened'
  | 'Withdrawn';

export type PartyType = 'Policyholder' | 'Claimant' | 'Insured' | 'ThirdParty' | 'Witness';

export type RiskObjectType = 'Vehicle' | 'Property' | 'Person' | 'Other';

export type ReserveStatus = 'PendingApproval' | 'Approved' | 'Rejected';

export interface ClaimListItem {
  claimId: string;
  claimNumber: string;
  policyNumber: string;
  policyholderName: string;
  causeOfLossCode: string;
  lossDate: string;
  reportedAtUtc: string;
  status: ClaimStatus;
  createdAtUtc: string;
}

export interface ClaimDetail {
  claimId: string;
  claimNumber: string;
  policyId: string;
  policyNumber: string;
  policyholderName: string;
  productType: string;
  causeOfLossCodeId: string;
  causeOfLossCode: string;
  causeOfLossDescription: string;
  lossDate: string;
  reportedAtUtc: string;
  description: string;
  status: ClaimStatus;
  createdByUserId: string;
  parties: ClaimParty[];
  riskObjects: RiskObject[];
  reserves: Reserve[];
  auditLogEntries: AuditLogEntry[];
}

export interface ClaimParty {
  id: string;
  partyType: PartyType;
  fullName: string;
  email: string | null;
  phone: string | null;
  notes: string | null;
}

export interface RiskObject {
  id: string;
  objectType: RiskObjectType;
  externalReference: string | null;
  description: string;
}

export interface Reserve {
  id: string;
  amount: number;
  currency: string;
  status: ReserveStatus;
  createdAtUtc: string;
  approvedByUserId: string | null;
  approvedAtUtc: string | null;
  rejectedByUserId: string | null;
  rejectedAtUtc: string | null;
  glPostedAtUtc: string | null;
  glPostingReference: string | null;
}

export interface AuditLogEntry {
  id: string;
  action: string;
  actorUserId: string;
  createdAtUtc: string;
  details: string | null;
}

export interface CreateClaimParty {
  partyType: PartyType;
  fullName: string;
  email: string | null;
  phone: string | null;
  notes: string | null;
}

export interface CreateClaimRiskObject {
  objectType: RiskObjectType;
  externalReference: string | null;
  description: string;
}

export interface CreateClaimRequest {
  policyId: string;
  causeOfLossCodeId: string;
  lossDate: string;
  description: string;
  createdByUserId: string;
  parties: CreateClaimParty[];
  riskObjects: CreateClaimRiskObject[];
}

export interface CreateClaimResponse {
  claimId: string;
  claimNumber: string;
  status: ClaimStatus;
  reportedAtUtc: string;
}

export interface UpdateClaimStatusRequest {
  newStatus: ClaimStatus;
  actorUserId: string;
}

export interface UpdateClaimStatusResponse {
  claimId: string;
  claimNumber: string;
  oldStatus: ClaimStatus;
  newStatus: ClaimStatus;
  changedAtUtc: string;
}

export interface CreateReserveRequest {
  amount: number;
  currency: string;
  createdByUserId: string;
}

export interface CreateReserveResponse {
  reserveId: string;
  claimId: string;
  amount: number;
  currency: string;
  status: ReserveStatus;
  createdAtUtc: string;
}

export interface ApproveReserveRequest {
  actorUserId: string;
}

export interface ApproveReserveResponse {
  reserveId: string;
  claimId: string;
  amount: number;
  currency: string;
  status: ReserveStatus;
  approvedByUserId: string | null;
  approvedAtUtc: string | null;
}

export interface RejectReserveRequest {
  actorUserId: string;
  reason: string;
}

export interface RejectReserveResponse {
  reserveId: string;
  claimId: string;
  amount: number;
  currency: string;
  status: ReserveStatus;
  rejectedByUserId: string | null;
  rejectedAtUtc: string | null;
  rejectionReason: string | null;
}
