using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachine.Models;

[Index(nameof(Name), IsUnique = true)]
public class CoffeeModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }    
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
    public int CoffeeAmount { get; set; }
    public int MilkAmount { get; set; }
    public int WaterAmount { get; set; }
}
