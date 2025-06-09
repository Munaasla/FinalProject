using System.Collections.Generic;

namespace Project.Models
{
    public class DashboardModel
    {
        public string ParentName { get; set; }
        public List<Baby> Babies { get; set; }
        public double AverageWeight { get; set; }
        public double AverageHeight { get; set; }
    }
}