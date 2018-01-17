using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace User_Registration_MVC.Models
{
    [MetadataType(typeof(SleepMetaData))]
    public partial class Sleep
    {
        public Sleep()
        {

        }
        public Sleep(DateTime start, DateTime end)
        {
            this.StartSleep = start;
            this.EndSleep = end;
            SetAmountOfSleep();
        }
        public Sleep(Sleep sleep)
        {
            this.EndSleep = sleep.EndSleep;
            this.StartSleep = sleep.StartSleep;
            this.MorningRating = sleep.MorningRating;
            this.EveningRating = sleep.EveningRating;
            this.Note = sleep.Note;
            this.QuickSleep = sleep.QuickSleep;
            SetAmountOfSleep();
        }
        public void SetAmountOfSleep()
        {
            this.AmountOfSleep = EndSleep - StartSleep;
            AmountOfSleep = TimeExtensions.StripMilliseconds((TimeSpan)AmountOfSleep);
        }
    
        public DayOfWeek DayOfWeek { get { return this.StartSleep.DayOfWeek; } }
    }

    public class SleepMetaData
    {
        [Display(Name = "Start Sleep")]
        //[DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public System.DateTime StartSleep { get; set; }
        [Display(Name = "End Sleep")]
        //[DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public System.DateTime EndSleep { get; set; }
        [Display(Name = "Morning Rating (1-10)")]
        [Range(typeof(int),"1","10",ErrorMessage = "must be intiger from 0-10")]
        public Nullable<int> MorningRating { get; set; }
        [Display(Name = "Evening Rating (1-10)")]
        [Range(typeof(int), "1", "10", ErrorMessage = "must be intiger from 0-10")]
        public Nullable<int> EveningRating { get; set; }
        [Display(Name = "Amount of Sleep")]
        //[DisplayFormat(DataFormatString = "{0:t}", ApplyFormatInEditMode = true)]
        public Nullable<System.TimeSpan> AmountOfSleep { get; set; }
        [Display(Name ="Just a nap")]
        public bool QuickSleep { get; set; }
        [Display(Name ="Day of week")]
        public DayOfWeek DayOfWeek { get; }

    }
    public static class TimeExtensions
    {
        public static TimeSpan StripMilliseconds(this TimeSpan time)
        {
            return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
        }
    }
}