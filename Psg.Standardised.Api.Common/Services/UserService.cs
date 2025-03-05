using Psg.Standardised.Api.Common.Models;

namespace Psg.Standardised.Api.Common.Services
{
    public class UserService
    {
        public ApiUser User { get; }

        public UserService(IRequestContextService requestContext)
            => User = new ApiUser(requestContext);
    }
}
