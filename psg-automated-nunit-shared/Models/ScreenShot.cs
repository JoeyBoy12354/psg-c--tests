namespace psg_automated_nunit_shared.Models
{
    /// <summary>
    /// Record to keep Screenshot data, to be saved to disk later
    /// </summary>  
    public sealed record ScreenShot(string? Key,
                                    byte[]? ImageData);
   
}
