namespace CapstoneQuizzCreationApp.Models.DTO.ResponseDTO
{
    public class TestQuestionDTO
    {
        public int SubmissionAnswerId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionDescription { get; set; }
        public string QuestionType { get; set; }
        public bool IsFlagged { get; set; }
        public string SelectedAnswer { get; set; }

       public  List<OptionDTO> Options { get; set; }   


    }
}
