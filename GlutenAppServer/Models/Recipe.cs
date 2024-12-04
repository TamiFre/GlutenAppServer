using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlutenAppServer.Models;

public partial class Recipe
{
    [Key]
    [Column("RecipeID")]
    public int RecipeId { get; set; }

    [StringLength(1000)]
    public string? RecipeText { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("StatusID")]
    public int? StatusId { get; set; }

    [ForeignKey("StatusId")]
    [InverseProperty("Recipes")]
    public virtual Status? Status { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Recipes")]
    public virtual User? User { get; set; }
}
