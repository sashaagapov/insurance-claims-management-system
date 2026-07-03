# Project Context

## Assessment Context

This repository is for a DICEUS Claims Module technical assessment.

The goal is not to build a full enterprise insurance platform. The goal is to prepare and then implement a clear backend-first MVP that demonstrates practical .NET, API, persistence, architecture, and business-rule handling.

## Candidate Context


That means:

- clarity is more important than advanced patterns
- the backend should be explainable during review
- unnecessary enterprise complexity should be avoided
- documentation should capture why choices were made
- the code should show learning and engineering discipline

## Confidential Documents

The assessment documents are confidential and are expected to live inside:

- `docs/specs/DICEUS_Fullstack_Technical_Assessment.docx`
- `docs/specs/Claims_Module_Candidate_Specification.docx`

Do not expose their contents outside this private repository. Do not copy long confidential document text into public places, prompts, issues, or external tools.

## Product Summary

The project is a simplified Claims Management System for an insurance domain.

Core flow:

1. A user reports a loss event.
2. The system creates a claim through FNOL.
3. The claim can be linked to a simulated insurance policy.
4. The claim stores loss details, parties, risk objects, status, reserves, and audit logs.
5. Reserve approvals follow simple threshold and role rules.
6. Approved reserves trigger simulated GL posting through Hangfire.
7. Important actions are stored in an append-only audit log.

## Review Goal

The review should be able to show:

- a planned Clean Architecture backend
- simple but meaningful insurance-domain behavior
- validation and business rules
- persistence with EF Core and SQL Server
- background processing with Hangfire
- auditability
- practical tradeoffs
