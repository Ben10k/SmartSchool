using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSchool.Model {
    [Table("PurchaseLog")]
    public class PurchaseLog {
        [Key]
        public Guid Id { get; set; }

        public User User { get; set; }
        public Product Product{ get; set; }
        public DateTime Time { get; set; }
    }
}