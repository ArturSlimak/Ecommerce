using FluentValidation;

namespace CatalogService.DTOs;

public class ProductDTO
{

    public class Index
    {
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

    }
    public class Details
    {
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class ToCreate
    {

        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public class Validator : AbstractValidator<ToCreate>
        {
            public Validator()
            {
                RuleFor(model => model.Name).NotEmpty();
                RuleFor(model => model.Description).NotEmpty();
                RuleFor(model => model.Price).GreaterThanOrEqualTo(0);
                RuleFor(model => model.Quantity).GreaterThanOrEqualTo(0);
            }
        }
    }

    public class ToMutate
    {

        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public class Validator : AbstractValidator<ToMutate>
        {
            public Validator()
            {
                RuleFor(model => model.Name).NotEmpty();
                RuleFor(model => model.Description).NotEmpty();
                RuleFor(model => model.Price).GreaterThanOrEqualTo(0);
                RuleFor(model => model.Quantity).GreaterThanOrEqualTo(0);
            }
        }
    }

}