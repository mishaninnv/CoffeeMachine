using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeMachine.Models;

public class RequestModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int AddMilk { get; set; }
    public int AddSugar { get; set; }
    public int Price { get; set; }

    [ForeignKey("PaymentTypeModel")]
    public int PaymentType { get; set; }
    public PaymentTypeModel PaymentTypeModel { get; set; } = null!;
}
