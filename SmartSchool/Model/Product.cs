using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSchool.Model {
    [Table("Product")]
    public class Product {
        [Key]
        public Guid Id { get; set; }

        public double Price { get; set; }
        public string Name { get; set; }
    }
}