namespace Vouchee.Data.Models.Filters
{
    public class ShopFilter
    {
        public string? name { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public decimal? percentShow { get; set; }
        public int? responsibilityScore { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }
}