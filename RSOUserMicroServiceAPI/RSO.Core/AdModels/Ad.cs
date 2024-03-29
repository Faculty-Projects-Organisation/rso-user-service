﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RSO.Core.AdModels;

public partial class Ad
{
    [Key]
    public int ID { get; set; }
    
    public int UserId { get; set; }

    [Column(TypeName = "char")]
    public string Thing { get; set; }

    public int? Price { get; set; }

    [Column(TypeName = "char")]
    public string Category { get; set; }

    public DateTime PostTime { get; set; }
}