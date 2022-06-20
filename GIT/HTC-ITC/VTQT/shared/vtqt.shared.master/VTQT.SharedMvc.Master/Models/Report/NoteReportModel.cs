namespace VTQT.SharedMvc.Master.Models
{
    public class NoteReportModel
    {
        public string ProjectId { get; set; }

        public string DepartmentId { get; set; }

        public string ProjectName { get; set; }

        public string DepartmentName { get; set; }

        public string Proposer { get; set; }

        public string Purpose { get; set; }

        public int ImportQuantity { get; set; }

        public int ExportQuantity { get; set; }
    }
}
