using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace User_Registration_MVC.Models.ViewModels
{
    public class ReloadTimer
    {
        public bool RememberTimer { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }
}