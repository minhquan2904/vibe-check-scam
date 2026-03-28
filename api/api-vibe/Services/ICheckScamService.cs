using api_vibe.Models.Requests;
using api_vibe.Models.Responses;

namespace api_vibe.Services;

public interface ICheckScamService
{
    Task<CheckScamResponse> ProcessCheckScamAsync(CheckScamRequest request, CancellationToken cancellationToken = default);
}
