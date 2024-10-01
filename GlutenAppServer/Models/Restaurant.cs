using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlutenAppServer.Models;

public partial class Restaurant
{
    [Key]
    [Column("RestID")]
    public int RestId { get; set; }

    [StringLength(70)]
    public string? RestAddress { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("TypeFoodID")]
    public int? TypeFoodId { get; set; }

    [InverseProperty("Rest")]
    public virtual ICollection<Critic> Critics { get; set; } = new List<Critic>();

    [ForeignKey("TypeFoodId")]
    [InverseProperty("Restaurants")]
    public virtual TypeFood? TypeFood { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Restaurants")]
    public virtual User? User { get; set; }
}
