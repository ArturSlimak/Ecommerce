using CatalogService.DTOs;
using CatalogService.Models.Product;
using FluentValidation;

namespace CatalogService.Helpers;

public class ProductRequest
{
    public class Index
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string SortBy { get; set; } = nameof(SortField.Name);
        public bool SortDescending { get; set; }


        public class Validator : AbstractValidator<Index>
        {
            public Validator()
            {
                RuleFor(x => x.PageSize)
                    .GreaterThan(0).When(x => x.PageSize != default);
                RuleFor(x => x.Page)
                    .GreaterThan(0).When(x => x.Page != default);

                RuleFor(x => x.SortBy)
               .Must(sortBy =>
                   Enum.GetNames(typeof(SortField))
                       .Any(validSort => string.Equals(validSort, sortBy, StringComparison.OrdinalIgnoreCase)))
               .WithMessage($"Invalid sort field. Valid values are: {string.Join(", ", Enum.GetNames(typeof(SortField)))}.");

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
