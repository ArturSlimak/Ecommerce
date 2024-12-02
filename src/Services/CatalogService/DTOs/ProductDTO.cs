﻿using FluentValidation;

namespace CatalogService.DTOs;

public class ProductDTO
{

    public class Index
    {
        public string? PublicId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

    }

    public class ToCreate
    {

        public string? Name { get; set; }
        public string? Description { get; set; }

        public class Validator : AbstractValidator<ToCreate>
        {
            public Validator()
            {
                RuleFor(model => model.Name).NotEmpty();
                RuleFor(model => model.Description).NotEmpty();
            }
        }
    }

    public class ToMutate
    {

        public string? Name { get; set; }
        public string? Description { get; set; }

        public class Validator : AbstractValidator<ToMutate>
        {
            public Validator()
            {
                RuleFor(model => model.Name).NotEmpty();
                RuleFor(model => model.Description).NotEmpty();
            }
        }
    }
}