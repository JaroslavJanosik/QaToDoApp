using System;

namespace QaToDoApp.Models;

public abstract class Auditable
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
}