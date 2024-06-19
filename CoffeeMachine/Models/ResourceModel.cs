using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeMachine.Models;

public class ResourceModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int MilkCurrent { get; set; }
    public int MilkMax { get; set; }
    public int CoffeeCurrent { get; set; }
    public int CoffeeMax { get; set; }
    public int SugarCurrent { get; set; }
    public int SugarMax { get; set;}
    public int CupsCurrent { get; set; }
    public int CupsMax { get; set;}
    public int WaterCurrent { get; set; }
    public int WaterMax { get; set; }
}
