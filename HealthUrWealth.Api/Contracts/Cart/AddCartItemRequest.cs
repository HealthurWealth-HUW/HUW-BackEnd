namespace HealthUrWealth.Api.Contracts.Cart
{
    public sealed record AddCartItemRequest(
    Guid GuestId,
    long ProductId,
    int Quantity
);
}
