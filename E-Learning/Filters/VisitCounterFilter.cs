using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Filters
{
    public class VisitCounterFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            using (var db = new ELEARNINGEntities())
            {
                var today = DateTime.Today;

                // Ngày
                int daily = db.PageVisit
                    .Count(x => DbFunctions.TruncateTime(x.VisitDate) == today);

                // Tháng
                int monthly = db.PageVisit
                    .Count(x => x.VisitDate.Month == today.Month
                             && x.VisitDate.Year == today.Year);

                // Tuần
                int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                var startOfWeek = today.AddDays(-diff).Date;
                var endOfWeek = startOfWeek.AddDays(7);

                int weekly = db.PageVisit
                    .Count(x => x.VisitDate >= startOfWeek
                             && x.VisitDate < endOfWeek);

                filterContext.Controller.ViewBag.DailyVisits = daily;
                filterContext.Controller.ViewBag.WeeklyVisits = weekly;
                filterContext.Controller.ViewBag.MonthlyVisits = monthly;

                // Tổng
                int total = db.PageVisit.Count();
                filterContext.Controller.ViewBag.TotalVisits = total;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}