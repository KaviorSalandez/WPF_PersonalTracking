using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_PersonalTracking.ViewModels
{
    public class PositionModel
    {
        public int Id { get; set; } 
        public string PositionName { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
            
    }
}
