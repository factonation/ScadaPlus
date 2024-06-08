using Microsoft.AspNetCore.Identity;

namespace ScadaPlus.Data.Identity;

public class ScadaPlusIdentityUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}
