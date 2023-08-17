using System;
using System.Collections.Generic;

namespace WPF_PersonalTracking.DB;

public partial class Taskstate
{
    public int Id { get; set; }

    public string StateName { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
