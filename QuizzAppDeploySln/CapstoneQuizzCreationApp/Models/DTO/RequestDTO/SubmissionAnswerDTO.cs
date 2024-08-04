namespace CapstoneQuizzCreationApp.Models.DTO.RequestDTO
{
    public class SubmissionAnswerDTO
    {
        public int UserId { get; set; }
        public int SubmissionId { get; set;}
        public DateTime FinishTime { get; set; }=DateTime.Now;

    }

}
