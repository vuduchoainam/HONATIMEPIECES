using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace HONATIMEPIECES.Models
{
    public class PropertyProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int PropertyId { get; set; }
        public Property? Property { get; set; }

        public int PropertyValueId { get; set; }
        public PropertyValue? PropertyValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
