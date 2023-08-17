using System;
using System.Collections.Generic;

namespace WPF_PersonalTracking.DB;

public partial class Permission
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public DateTime PermissionStartDate { get; set; }

    public DateTime PermisstionEndDate { get; set; }

    public int PermissionState { get; set; }

    public string? PermissionExplaination { get; set; }

    public int PermissionDay { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Permissionstate PermissionStateNavigation { get; set; } = null!;
}
