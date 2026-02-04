using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CloudPart3.Areas.Identity.Data;
public class ApplicationUser : IdentityUser
{
    public bool IsAdmin { get; set; } = false;  //default is non-admin
}

