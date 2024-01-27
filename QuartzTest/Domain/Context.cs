namespace QuartzTest.Domain
{
    public class Context
    {
        public string Name { get; set; }
        public DateTime Date => DateTime.Now;
    }
}
