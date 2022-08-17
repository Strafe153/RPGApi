using Core.ViewModels;
using FluentValidation;

namespace WebApi.Validators
{
    public class HitValidator : AbstractValidator<HitViewModel>
    {
        public HitValidator()
        {
            RuleFor(h => h.DealerId)
                .NotEmpty()
                .WithMessage("Dealer is required");

            RuleFor(h => h.ReceiverId)
                .NotEmpty()
                .WithMessage("Receiver is required");

            RuleFor(h => h.ItemId)
                .NotEmpty()
                .WithMessage("Item is required");
        }
    }
}
