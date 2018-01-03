using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace User_Registration_MVC.Models
{
    public partial class Sleep
    {
        public void SetAmountOfSleep()
        {
            this.AmountOfSleep = EndSleep - StartSleep;
        }
        public DayOfWeek GetDayOfWeek()
        {
            return this.StartSleep.DayOfWeek;
        }
    }
}