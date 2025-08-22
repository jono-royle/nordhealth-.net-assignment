using Api.Models;

namespace Api.Data
{
    internal static class VetData
    {
        internal static List<Veterinarian> Vets = new()
        {
            new Veterinarian { Id = new Guid("70a5e606-25c9-4412-907d-e490bee7bfce"), Name = "Vet1" },
            new Veterinarian { Id = new Guid("7399388f-3134-4381-951a-de10a9895bd0"), Name = "Vet2" }
        };
    }
}
