using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClaimsModule.Application.CauseOfLossCodes.GetCauseOfLossCodes;
using ClaimsModule.Application.Claims.CreateClaim;
using ClaimsModule.Application.Claims.GetClaimById;
using ClaimsModule.Application.Claims.GetClaims;
using ClaimsModule.Application.Claims.UpdateClaimStatus;
using ClaimsModule.Application.Policies.GetPolicies;
using ClaimsModule.Application.Reserves.ApproveReserve;
using ClaimsModule.Application.Reserves.CreateReserve;
using ClaimsModule.Application.Reserves.RejectReserve;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Infrastructure.BackgroundJobs;
using ClaimsModule.Persistence.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace ClaimsModule.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public sealed class BackendMvpFlowTests(ClaimsModuleApiFactory factory)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private static readonly Guid HandlerUserId = SeedDataIds.UserHandler;
    private static readonly Guid SupervisorUserId = SeedDataIds.UserSupervisor;
    private static readonly Guid ManagerUserId = SeedDataIds.UserManager;
    private static readonly Guid AutoPolicyId = SeedDataIds.PolicyAuto;
    private static readonly Guid CollisionCauseId = SeedDataIds.CauseCollision;

    [Fact]
    public async Task Health_endpoint_returns_ok()
    {
        var response = await factory.Client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("OK", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Lookup_endpoints_return_seeded_values()
    {
        var policies = await factory.Client.GetFromJsonAsync<IReadOnlyCollection<PolicyListItemDto>>("/api/policies", JsonOptions);
        var causeCodes = await factory.Client.GetFromJsonAsync<IReadOnlyCollection<CauseOfLossCodeDto>>("/api/cause-of-loss-codes", JsonOptions);

        Assert.NotNull(policies);
        Assert.Contains(policies, policy => policy.Id == AutoPolicyId);

        Assert.NotNull(causeCodes);
        Assert.Contains(causeCodes, causeCode => causeCode.Id == CollisionCauseId);
    }

    [Fact]
    public async Task Claim_creation_detail_list_and_status_transition_flow_work()
    {
        var createdClaim = await CreateClaimAsync();

        var detail = await GetClaimDetailAsync(createdClaim.ClaimId);
        Assert.Equal(AutoPolicyId, detail.PolicyId);
        Assert.Equal(CollisionCauseId, detail.CauseOfLossCodeId);
        Assert.Equal(ClaimStatus.Open, detail.Status);
        Assert.Contains(detail.Parties, party => party.FullName == "Integration Claimant");
        Assert.Contains(detail.RiskObjects, riskObject => riskObject.Description == "Integration test vehicle");
        Assert.Contains(detail.AuditLogEntries, audit => audit.Action == "ClaimCreated");

        var claims = await factory.Client.GetFromJsonAsync<IReadOnlyCollection<ClaimListItemDto>>("/api/claims", JsonOptions);
        Assert.NotNull(claims);
        Assert.Contains(claims, claim => claim.ClaimId == createdClaim.ClaimId);

        var statusResponse = await factory.Client.PatchAsJsonAsync(
            $"/api/claims/{createdClaim.ClaimId}/status",
            new
            {
                newStatus = ClaimStatus.UnderInvestigation,
                actorUserId = HandlerUserId
            },
            JsonOptions);

        Assert.Equal(HttpStatusCode.OK, statusResponse.StatusCode);
        var statusResult = await statusResponse.Content.ReadFromJsonAsync<UpdateClaimStatusResponse>(JsonOptions);
        Assert.NotNull(statusResult);
        Assert.Equal(ClaimStatus.UnderInvestigation, statusResult.NewStatus);

        var updatedDetail = await GetClaimDetailAsync(createdClaim.ClaimId);
        Assert.Equal(ClaimStatus.UnderInvestigation, updatedDetail.Status);

        var invalidTransitionResponse = await factory.Client.PatchAsJsonAsync(
            $"/api/claims/{createdClaim.ClaimId}/status",
            new
            {
                newStatus = ClaimStatus.Closed,
                actorUserId = HandlerUserId
            },
            JsonOptions);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, invalidTransitionResponse.StatusCode);
    }

    [Fact]
    public async Task Reserve_creation_approval_and_rejection_rules_work()
    {
        var claim = await CreateClaimAsync();

        var smallReserve = await CreateReserveAsync(claim.ClaimId, 5000m, HandlerUserId);
        Assert.Equal(ReserveStatus.Approved, smallReserve.Status);

        var handlerReserve = await CreateReserveAsync(claim.ClaimId, 15000m, SupervisorUserId);
        var handlerApprovalResponse = await ApproveReserveAsync(claim.ClaimId, handlerReserve.ReserveId, HandlerUserId);
        Assert.Equal(HttpStatusCode.Forbidden, handlerApprovalResponse.StatusCode);

        var selfApprovalReserve = await CreateReserveAsync(claim.ClaimId, 15000m, SupervisorUserId);
        var selfApprovalResponse = await ApproveReserveAsync(claim.ClaimId, selfApprovalReserve.ReserveId, SupervisorUserId);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, selfApprovalResponse.StatusCode);

        var approvalReserve = await CreateReserveAsync(claim.ClaimId, 15000m, HandlerUserId);
        Assert.Equal(ReserveStatus.PendingApproval, approvalReserve.Status);

        var approvalResponse = await ApproveReserveAsync(claim.ClaimId, approvalReserve.ReserveId, SupervisorUserId);
        Assert.Equal(HttpStatusCode.OK, approvalResponse.StatusCode);
        var approvedReserve = await approvalResponse.Content.ReadFromJsonAsync<ApproveReserveResponse>(JsonOptions);
        Assert.NotNull(approvedReserve);
        Assert.Equal(ReserveStatus.Approved, approvedReserve.Status);

        var duplicateApprovalResponse = await ApproveReserveAsync(claim.ClaimId, approvalReserve.ReserveId, ManagerUserId);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, duplicateApprovalResponse.StatusCode);

        var rejectionReserve = await CreateReserveAsync(claim.ClaimId, 15000m, HandlerUserId);
        var rejectionResponse = await factory.Client.PatchAsJsonAsync(
            $"/api/claims/{claim.ClaimId}/reserves/{rejectionReserve.ReserveId}/reject",
            new
            {
                actorUserId = ManagerUserId,
                reason = "Integration test rejection."
            },
            JsonOptions);

        Assert.Equal(HttpStatusCode.OK, rejectionResponse.StatusCode);
        var rejectedReserve = await rejectionResponse.Content.ReadFromJsonAsync<RejectReserveResponse>(JsonOptions);
        Assert.NotNull(rejectedReserve);
        Assert.Equal(ReserveStatus.Rejected, rejectedReserve.Status);
    }

    [Fact]
    public async Task Approved_reserves_are_posted_to_gl_and_gl_job_is_idempotent()
    {
        var claim = await CreateClaimAsync();
        var reserve = await CreateReserveAsync(claim.ClaimId, 5000m, HandlerUserId);

        var postedReserve = await PollForPostedReserveAsync(claim.ClaimId, reserve.ReserveId);
        var firstReference = postedReserve.GlPostingReference;
        Assert.NotNull(postedReserve.GlPostedAtUtc);
        Assert.False(string.IsNullOrWhiteSpace(firstReference));

        var detailAfterPosting = await GetClaimDetailAsync(claim.ClaimId);
        Assert.Single(detailAfterPosting.AuditLogEntries, audit => audit.Action == "ReserveGlPosted");

        using var scope = factory.Services.CreateScope();
        var job = scope.ServiceProvider.GetRequiredService<ReserveGlPostingJob>();
        await job.PostReserveAsync(reserve.ReserveId);

        var detailAfterSecondRun = await GetClaimDetailAsync(claim.ClaimId);
        var reserveAfterSecondRun = Assert.Single(detailAfterSecondRun.Reserves, item => item.Id == reserve.ReserveId);

        Assert.Equal(firstReference, reserveAfterSecondRun.GlPostingReference);
        Assert.Single(detailAfterSecondRun.AuditLogEntries, audit => audit.Action == "ReserveGlPosted");
    }

    private async Task<CreateClaimResponse> CreateClaimAsync()
    {
        var response = await factory.Client.PostAsJsonAsync("/api/claims", new CreateClaimCommand(
            AutoPolicyId,
            CollisionCauseId,
            new DateOnly(2026, 7, 2),
            "Integration test collision claim created through FNOL.",
            HandlerUserId,
            [
                new CreateClaimPartyDto(
                    PartyType.Claimant,
                    "Integration Claimant",
                    "integration.claimant@example.test",
                    "+380000000000",
                    "Created by integration test.")
            ],
            [
                new CreateRiskObjectDto(
                    RiskObjectType.Vehicle,
                    $"VIN-IT-{Guid.NewGuid():N}"[..16],
                    "Integration test vehicle")
            ]),
            JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdClaim = await response.Content.ReadFromJsonAsync<CreateClaimResponse>(JsonOptions);
        Assert.NotNull(createdClaim);
        return createdClaim;
    }

    private async Task<ClaimDetailDto> GetClaimDetailAsync(Guid claimId)
    {
        var detail = await factory.Client.GetFromJsonAsync<ClaimDetailDto>($"/api/claims/{claimId}", JsonOptions);
        Assert.NotNull(detail);
        return detail;
    }

    private async Task<CreateReserveResponse> CreateReserveAsync(
        Guid claimId,
        decimal amount,
        Guid createdByUserId)
    {
        var response = await factory.Client.PostAsJsonAsync(
            $"/api/claims/{claimId}/reserves",
            new
            {
                amount,
                currency = "USD",
                createdByUserId
            },
            JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var reserve = await response.Content.ReadFromJsonAsync<CreateReserveResponse>(JsonOptions);
        Assert.NotNull(reserve);
        return reserve;
    }

    private Task<HttpResponseMessage> ApproveReserveAsync(Guid claimId, Guid reserveId, Guid actorUserId)
    {
        return factory.Client.PatchAsJsonAsync(
            $"/api/claims/{claimId}/reserves/{reserveId}/approve",
            new { actorUserId },
            JsonOptions);
    }

    private async Task<ReserveSummaryDto> PollForPostedReserveAsync(Guid claimId, Guid reserveId)
    {
        var timeoutAt = DateTime.UtcNow.AddSeconds(20);

        while (DateTime.UtcNow < timeoutAt)
        {
            var detail = await GetClaimDetailAsync(claimId);
            var reserve = Assert.Single(detail.Reserves, item => item.Id == reserveId);

            if (reserve.GlPostedAtUtc is not null && !string.IsNullOrWhiteSpace(reserve.GlPostingReference))
            {
                return reserve;
            }

            await Task.Delay(500);
        }

        throw new TimeoutException("The approved reserve was not posted to the simulated GL within the expected time.");
    }
}
