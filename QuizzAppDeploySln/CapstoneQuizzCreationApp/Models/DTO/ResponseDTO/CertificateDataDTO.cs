namespace CapstoneQuizzCreationApp.Models.DTO.ResponseDTO
{
    public class CertificateDataDTO
    {
        public bool IsPassed { get; set; }=false;
        public int CertificateId { get; set; }
        public string CertificateTestName { get; set; }
        public int ObtainedScore { get; set; }
        public DateTime ProvidedDate { get; set; }
        public double TimeTaken { get; set; }
        public string UserName { get; set; }
        public bool IsFastAchiver { get; set; }
        public int TotalMark {  get; set; }

    }
}
