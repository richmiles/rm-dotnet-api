namespace RM.Api.Data
{
    public class ModelBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime TimestampUtc { get; set; }
    }
}
