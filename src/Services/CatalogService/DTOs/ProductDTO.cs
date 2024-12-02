using FluentValidation;

namespace CatalogService.DTOs;

public class ProductDTO
{

    public class Index
    {
        public string? Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }

    }

    public class ToCreate
    {

        public string? Name { get; set; }
        public decimal Price { get; set; }

        public class Validator : AbstractValidator<ToCreate>
        {
            public Validator()
            {
                RuleFor(model => model.Name).NotEmpty();
                RuleFor(model => model.Price).GreaterThan(5);
            }
        }
    }

    public class ToMutate
    {

        public string? Name { get; set; }
        public decimal Price { get; set; }

        public class Validator : AbstractValidator<ToMutate>
        {
            public Validator()
            {
                RuleFor(model => model.Name).NotEmpty();
                RuleFor(model => model.Price).GreaterThan(5);
            }
        }
    }
}