﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bwarrickBugTracker.Models.Helpers
{
    public class AdminUserViewModels
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }
    }
}