namespace CapstoneQuizzCreationApp.Models.DTO.RequestDTO
{
    public class QuestionDTO
    {
        public string Question {  get; set; }
        public int points { get; set; }
        public string CorrectAnswer { get; set; }
        public string QuestionType { get; set; }
        public bool IsActive { get; set; }

        public List<string> Options { get; set; }
    }
}
