namespace psg_automated_nunit_common.Contracts
{
    public interface IAuthorisationClient
    {
        Task<bool> ValidateTokenAsync(string token);
    }
}
