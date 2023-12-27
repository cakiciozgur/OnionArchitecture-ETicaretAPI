using ETicaretAPI.Application.ViewModels.Products;
using FluentValidation;

namespace ETicaretAPI.Application.Validators.Products
{
    public class CreateProductValidator : AbstractValidator<VMCreateProduct>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Lütfen ürün adını boş geçmeyiniz!")
                .MaximumLength(150)
                .MinimumLength(5)
                    .WithMessage("Ürün adı 5-150 karakter arasında olmalıdır");

            RuleFor(p => p.Stock)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Lütfen stok bilgisini boş geçmeyiniz!")
                .Must(p => p >= 0)
                    .WithMessage("Stok bilgisini giriniz!");


            RuleFor(p => p.Price)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Lütfen fiyat bilgisini boş geçmeyiniz!")
                .Must(p => p >= 0)
                    .WithMessage("Fiyat bilgisini giriniz!");
        }
    }
}
