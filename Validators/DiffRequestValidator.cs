using FluentValidation;
using StringCompare.API.Models;

namespace StringCompare.API.Validators
{
    public class DiffRequestValidator : AbstractValidator<DiffRequest>
    {
        public DiffRequestValidator()
        {
            RuleFor(x => x.Text1).NotNull();
            RuleFor(x => x.Text2).NotNull();
        }
    }
}
