namespace SocleRHHBESSAIH.ViewModels.DTO
{
    public class ExpenseDTO
    {
        public string Id { get; set; } = "";
        public string User { get; set; } = "";
        public DateTime Date { get; set; }
        public string ExpenseOrigin { get; set; } = "";
        public decimal Amount { get; set; }
        public string ExpenseCurrency { get; set; } = "";
        public string Comment { get; set; } = "";


    }
}
