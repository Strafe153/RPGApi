﻿using Core.ViewModels.MountViewModels;
using FluentValidation;

namespace WebApi.Validators.MountValidators
{
    public class MountCreateUpdateValidator : MountBaseValidator<MountBaseViewModel>
    {
        public MountCreateUpdateValidator()
        {
            RuleFor(w => w.Speed)
                .GreaterThan(0)
                .WithMessage("Damage must be greater than 0")
                .LessThan(51)
                .WithMessage("Damage must not be greater than 50");
        }
    }
}
