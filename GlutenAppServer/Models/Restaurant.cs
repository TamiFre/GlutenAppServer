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

    [StringLength(70)]
    public string? RestName { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("TypeFoodID")]
    public int? TypeFoodId { get; set; }

    [Column("StatusID")]
    public int? StatusId { get; set; }

    [InverseProperty("Rest")]
    public virtual ICollection<Critic> Critics { get; set; } = new List<Critic>();

    [ForeignKey("StatusId")]
    [InverseProperty("Restaurants")]
    public virtual Status? Status { get; set; }

    [ForeignKey("TypeFoodId")]
    [InverseProperty("Restaurants")]
    public virtual TypeFood? TypeFood { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Restaurants")]
    public virtual User? User { get; set; }
}
