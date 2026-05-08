namespace HealthUrWealth.Api.Contracts.Cart
{
    public sealed record UpdateCartRequest(
    long ProductId,
    int Quantity
);
}
