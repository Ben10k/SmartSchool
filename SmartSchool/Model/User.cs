using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSchool.Model {
    [Table("User")]
    public class User {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        public double Balance { get; set; }
    }
}