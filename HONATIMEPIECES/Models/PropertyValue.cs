using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace HONATIMEPIECES.Models
{
    public class PropertyValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Giá trị thuộc tính không được để trống")]
        public string Value { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<PropertyProduct> PropertyProducts { get; set; } = new List<PropertyProduct>();
    }
}
