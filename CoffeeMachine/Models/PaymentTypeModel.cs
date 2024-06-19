using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachine.Models;

[Index(nameof(Name), IsUnique = true)]
public class PaymentTypeModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<RequestModel> Requests { get; set; } = null!;
}
