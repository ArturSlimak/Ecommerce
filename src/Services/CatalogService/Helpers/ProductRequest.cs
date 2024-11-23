using CatalogService.DTOs;
using FluentValidation;

namespace CatalogService.Helpers;

public class ProductRequest
{
    public class Index
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;


        public class Validator : AbstractValidator<Index>
        {
            public Validator()
            {
                RuleFor(x => x.PageSize)
                    .GreaterThan(0).When(x => x.PageSize != default);
                RuleFor(x => x.Page)
                    .GreaterThan(0).When(x => x.Page != default);

            }
        }
    }

    public class Create
    {
        public required ProductDTO.Mutate Product { get; set; }

        public class Validator : AbstractValidator<Create>
        {
            public Validator()
            {
                RuleFor(model => model.Product)
                    .NotNull()
                    .SetValidator(new ProductDTO.Mutate.Validator());
            }
        }

    }
}
