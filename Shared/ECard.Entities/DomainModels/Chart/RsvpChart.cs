using System.Collections.Generic;

namespace ECard.Entities.DomainModels.Chart
{
    public class RsvpChart
    {
        public List<PieChartData> PieChartDatas { get; set; }
        public List<TableChartData> TableChartDatas { get; set; }
    }
}
