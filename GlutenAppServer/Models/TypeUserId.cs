using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlutenAppServer.Models;

[Table("TypeUserID")]
public partial class TypeUserId
{
    [Key]
    [Column("TypeID")]
    public int TypeId { get; set; }

    [StringLength(20)]
    public string? TypeName { get; set; }

    [InverseProperty("Type")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
