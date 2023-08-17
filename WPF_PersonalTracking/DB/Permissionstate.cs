using System;
using System.Collections.Generic;

namespace WPF_PersonalTracking.DB;

public partial class Permissionstate
{
    public int Id { get; set; }

    public string StateName { get; set; } = null!;

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
