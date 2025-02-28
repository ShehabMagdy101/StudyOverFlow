using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyOverFlow.DTOs.Manage
{
    public class TagDto
    {
        public int? TagId { get; set; }
        public string Name { get; set; } = null!;
        public int ColorId { get; set; }
        public string? type { get; set; }
    }
}
