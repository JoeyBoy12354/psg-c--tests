using FluentValidation;
using psg_automated_nunit_shared.Models;

namespace psg_automated_nunit_common.Validator
{
    public sealed class TestResultDtoValidator : AbstractValidator<TestResultDto>
    {
        public TestResultDtoValidator()
        {
            RuleFor(x => x.Key).NotEmpty();
            RuleFor(x => x.Status).NotEmpty().WithMessage("Status must be 'Passed' or 'Failed'");
            RuleFor(x => x.Date).NotEmpty();
        }
    }
}
