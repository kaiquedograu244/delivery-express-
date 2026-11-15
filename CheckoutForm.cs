namespace DeliveryExpress.Models;

public class CheckoutForm
{
    [Required(ErrorMessage = "Informe seu nome.")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "Informe seu nome completo.")]
    public string Nome { get; set; } = "";

    [Required(ErrorMessage = "Informe o telefone com DDD.")]
    [RegularExpression(@"^[0-9\s\-\(\)]{8,20}$", ErrorMessage = "Telefone inválido.")]
    public string Telefone { get; set; } = "";

    [Required(ErrorMessage = "Informe o endereço de entrega.")]
    [StringLength(200, MinimumLength = 8, ErrorMessage = "Informe a rua e o número.")]
    public string Endereco { get; set; } = "";

    [StringLength(80, ErrorMessage = "O complemento pode ter no máximo 80 caracteres.")]
    public string Complemento { get; set; } = "";

    [Required(ErrorMessage = "Escolha a forma de pagamento.")]
    public string Pagamento { get; set; } = "";

    [StringLength(240, ErrorMessage = "A observação pode ter no máximo 240 caracteres.")]
    public string Observacao { get; set; } = "";
}
