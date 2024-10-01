using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlutenAppServer.Models;

[Table("TypeFood")]
public partial class TypeFood
{
    [Key]
    [Column("TypeFoodID")]
    public int TypeFoodId { get; set; }

    [StringLength(30)]
    public string? TypeFoodName { get; set; }

    [InverseProperty("TypeFood")]
    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
}
