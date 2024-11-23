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

    public class Mutate
    {

        public string? Name { get; set; }
        public decimal Price { get; set; }

        public class Validator : AbstractValidator<Mutate>
        {
            public Validator()
            {
                RuleFor(model => model.Name).NotEmpty();
                RuleFor(model => model.Price).GreaterThan(5);
            }
        }
    }
}