using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_PersonalTracking.ViewModels
{
    public class PermissionModel
    {
        public int Id { get; set; } 
        public int Userno { get; set; }
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string StateName { get; set; }
        public int PermissionState { get; set; }
        public string Explaination { get; set; }

        public int DayAmount { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }

    }
}
