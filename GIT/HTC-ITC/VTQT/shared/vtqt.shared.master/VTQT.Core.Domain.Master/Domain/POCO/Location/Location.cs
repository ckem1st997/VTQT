namespace VTQT.Core.Domain.Master
{
    public class Location
    {
        public string LocationId { get; set; }

        public string Name { get; set; }

        public string ParentId { get; set; }

        public int Level { get; set; }

        public bool Active { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsShow { get; set; }

        public string Code { get; set; }

        public string ParentName { get; set; }

        public string Path { get; set; }
    }
}
