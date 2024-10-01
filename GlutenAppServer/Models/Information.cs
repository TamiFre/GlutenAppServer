using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlutenAppServer.Models;

public partial class Information
{
    [Key]
    [Column("InfoID")]
    public int InfoId { get; set; }

    [StringLength(1000)]
    public string? InfoText { get; set; }
}
