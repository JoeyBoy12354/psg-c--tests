namespace psg_automated_nunit_common.Contracts
{
    public interface IAuthorisationService
    {
        Task<bool> CheckAuthorisationAsync();
    }
}
