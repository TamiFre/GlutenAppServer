using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlutenAppServer.Models;

public partial class Critic
{
    [Key]
    [Column("CriticID")]
    public int CriticId { get; set; }

    [StringLength(1000)]
    public string? CriticText { get; set; }

    public int? Rate { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("RestID")]
    public int? RestId { get; set; }

    [ForeignKey("RestId")]
    [InverseProperty("Critics")]
    public virtual Restaurant? Rest { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Critics")]
    public virtual User? User { get; set; }
}
