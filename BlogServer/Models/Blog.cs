﻿using MySql.EntityFrameworkCore.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogServer.Models
{
    [MySQLCharset("utf8mb4")]
    internal class Blog
    {
        public long ID { get; set; }

        [Required]
        [MaxLength(205)]
        public string? Content { get; set; }
    }
}
