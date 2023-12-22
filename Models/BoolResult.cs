namespace ZentitleOnPremDemo.Models
{
    public class BoolResult
    {
        public BoolResult(bool success)
        {
            Success = success;
        }

        public bool Success { get; set; }
    }
}
