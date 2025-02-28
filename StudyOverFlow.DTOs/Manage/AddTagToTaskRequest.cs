using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyOverFlow.DTOs.Manage
{
    public class AddTagToTaskRequest
    {
        public int TaskId { get; set; }
        public int TagId { get; set; }
    }
}
