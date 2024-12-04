using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlutenAppServer.Models;

public partial class Status
{
    [Key]
    [Column("StatusID")]
    public int StatusId { get; set; }

    [StringLength(50)]
    public string? StatusDesc { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    [InverseProperty("Status")]
    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
}
