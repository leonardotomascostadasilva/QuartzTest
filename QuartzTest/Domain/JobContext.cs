namespace QuartzTest.Domain
{
    public class JobContext
    {
        public string Name { get; set; }
        public DateTime Date => DateTime.Now;
    }
}
