# Demo Script

This script describes the intended review flow after the backend MVP is implemented.

## Before Demo

Prepare:

- API running locally
- SQL Server running locally, preferably through Docker
- database migrated and seeded
- Swagger page open
- mock users available:
  - handler
  - supervisor
  - manager

## Demo Flow

### 1. Start API

Run the API locally and confirm that Swagger is available.

What to explain:

- the API is the primary demo surface
- the frontend is optional for the MVP

### 2. Open Swagger

Open Swagger/OpenAPI in the browser.

What to explain:

- Swagger is used to demonstrate the backend-first vertical slice
- each endpoint maps to a clear use case

### 3. Search Policies

Use the policy lookup endpoint to find seeded policies.

What to explain:

- policies are seeded for the MVP
- no real policy system is integrated

### 4. Create Claim

Create a claim through the FNOL endpoint.

What to explain:

- FNOL means First Notice of Loss
- the claim is linked to a policy
- validation protects required input

### 5. View Claim Detail

Open the claim detail endpoint.

What to explain:

- the detail view shows the claim, loss details, status, reserves, and audit entries

### 6. View Audit Log

Show audit entries for claim creation.

What to explain:

- important actions are written to an append-only audit log
- audit history supports traceability

### 7. Transition Claim Status

Move the claim to the next supported status.

What to explain:

- only allowed status transitions should succeed
- rejected transitions should return a clear error

### 8. Create Reserve Under 10,000

Create a reserve with an amount below 10,000.

Expected result:

- reserve is auto-approved
- audit log records the action

What to explain:

- small reserves are auto-approved as a simplified business rule

### 9. Create Reserve Over 10,000

Create a reserve with an amount over 10,000.

Expected result:

- reserve is pending approval
- audit log records the action

What to explain:

- larger reserves require supervisor or manager approval

### 10. Try Self-Approval

Attempt to approve the reserve as the same user who created it.

Expected result:

- approval is rejected
- error message explains that self-approval is not allowed

What to explain:

- self-approval prevention is a core business rule

### 11. Approve As Supervisor Or Manager

Approve the reserve as a supervisor or manager.

Expected result:

- reserve becomes approved
- audit log records approval
- GL posting job is enqueued

What to explain:

- role-sensitive approval is mocked for the MVP

### 12. Show Hangfire GL Posting Audit Entry

Wait for or trigger the Hangfire job and show the resulting audit log entry.

Expected result:

- GL posting is simulated
- duplicate posting is prevented by idempotency protection

What to explain:

- background processing is useful for work that should happen after approval
- idempotency prevents accidental double posting

## Closing Notes

End the demo by showing:

- architecture documents
- tradeoffs
- known limitations
- what would be improved with more time
