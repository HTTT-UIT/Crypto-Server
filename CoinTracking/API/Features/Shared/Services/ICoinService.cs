namespace API.Features.Shared.Services
{
    public interface ICoinService
    {
        Guid SyncCoin(string coinId);
    }
}
