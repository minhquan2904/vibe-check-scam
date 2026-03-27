using System.Threading.Tasks;

namespace api_vibe.Services
{
    public interface IAICoreService
    {
        Task<string> ParseTransactionAsync(string rawText);
    }
}
