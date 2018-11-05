using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSchool.Model {
    [Table("AccessLog")]
    public class AccessLog {
        [Key]
        public Guid Id { get; set; }
        public User User { get; set; }
        public DateTime Time { get; set; }
    }
}