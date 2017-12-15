using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace User_Registration_MVC.Models
{
    public class SleepsInitializer
    {
        public static List<Sleep> SleepsInitialize()
        {
            var sleeps = new List<Sleep>
            {
                new Sleep{SleepId = 1,Note = "przykladowa notatka1", MorningRating = 5, EveningRating=6, StartSleep = (DateTime.Now.AddDays(-8)), EndSleep = DateTime.Now.AddHours(6).AddDays(-8)},
                new Sleep{Note = "przykladowa notatka2", MorningRating = 5, EveningRating=6, StartSleep = (DateTime.Now.AddDays(-7)), EndSleep = DateTime.Now.AddHours(5).AddDays(-7)},
                new Sleep{Note = "przykladowa notatka9", MorningRating = 5, EveningRating=6, StartSleep = (DateTime.Now.AddDays(-6)), EndSleep = DateTime.Now.AddHours(4).AddDays(-6)},
                new Sleep{Note = "przykladowa notatka3", MorningRating = 5, EveningRating=6, StartSleep = (DateTime.Now.AddDays(-5)), EndSleep = DateTime.Now.AddHours(4.5).AddDays(-5)},
                new Sleep{Note = "przykladowa notatka4", MorningRating = 5, EveningRating=6, StartSleep = (DateTime.Now.AddDays(-4)), EndSleep = DateTime.Now.AddHours(3).AddDays(-4)},
                new Sleep{Note = "przykladowa notatka5", MorningRating = 5, EveningRating=6, StartSleep = (DateTime.Now.AddDays(-3)), EndSleep = DateTime.Now.AddHours(3.77).AddDays(-3)},
                new Sleep{Note = "przykladowa notatka6", MorningRating = 5, EveningRating=6, StartSleep = (DateTime.Now.AddDays(-2)), EndSleep = DateTime.Now.AddHours(6).AddDays(-2)},
                new Sleep{Note = "przykladowa notatka7", MorningRating = 5, EveningRating=6, StartSleep = (DateTime.Now.AddDays(-1)), EndSleep = DateTime.Now.AddHours(10).AddDays(-1)},
            };
            return sleeps;
        }
    }
}