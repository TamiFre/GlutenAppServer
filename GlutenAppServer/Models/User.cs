using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlutenAppServer.Models;

public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(20)]
    public string? UserName { get; set; }

    [StringLength(20)]
    public string? UserPass { get; set; }

    [Column("TypeID")]
    public int? TypeId { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Critic> Critics { get; set; } = new List<Critic>();

    [InverseProperty("User")]
    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    [InverseProperty("User")]
    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();

    [ForeignKey("TypeId")]
    [InverseProperty("Users")]
    public virtual TypeUserId? Type { get; set; }
}
