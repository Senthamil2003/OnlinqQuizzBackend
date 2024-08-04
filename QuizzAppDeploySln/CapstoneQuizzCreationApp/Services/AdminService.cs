using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using CapstoneQuizzCreationApp.Models.DTO.RequestDTO;
using CapstoneQuizzCreationApp.Models.DTO.ResponseDTO;

namespace CapstoneQuizzCreationApp.Services
{
    public class AdminService:IAdminService
    {
        private readonly IRepository<int,CertificationTest> _certificationRepo;
        private readonly IRepository<int,Question> _questionRepo;
        private readonly IRepository<int,Option> _optionRepo;
        private readonly ITransactionService _transactionService;
        private readonly IBlobService _blobService;
        public AdminService(
            IRepository<int, Option> optionRepo,
            IRepository<int,Question> questionRepo,
            IRepository<int,CertificationTest> certificationRepo,
            ITransactionService transactionService,
            IBlobService blobService
            )
        {
            _optionRepo = optionRepo;
            _questionRepo = questionRepo;
            _certificationRepo = certificationRepo;
            _transactionService = transactionService;
            _blobService = blobService;
        }
        private async Task CreateQuestion(List<QuestionDTO> questions,int CertificateTestId)
        {
            try
            {
                foreach (var questionItem in questions)
                {
                    Question question = new Question()
                    {
                        QuestionDescription = questionItem.Question,
                        Points = questionItem.points,
                        TestId = CertificateTestId,
                        QuestionType = questionItem.QuestionType,
                        IsActive = questionItem.IsActive,
                        CorrectAnswer=questionItem.CorrectAnswer,
                    };
                    await _questionRepo.Add(question);
                    if (question.QuestionType!="Fillups")
                    {
                        if(question.QuestionType== "True/False")
                        {
                            int ct = 0;
                            foreach (var optionItem in questionItem.Options)
                            {
                                if(ct==2)
                                {
                                    break;
                                }

                                Option option = new Option()
                                {
                                    OptionName = optionItem,
                                    QuestionId = question.QuestionId,

                                };
                                await _optionRepo.Add(option);
                                ct++;

                            }

                        }
                        else
                        {
                            foreach (var optionItem in questionItem.Options)
                            {

                                Option option = new Option()
                                {
                                    OptionName = optionItem,
                                    QuestionId = question.QuestionId,

                                };
                                await _optionRepo.Add(option);

                            }

                        }
                      

                    }

                   
                    
                }

            }
            catch
            {
                throw;
            }



         }
        public async Task<TestVisibleResponseDTO> ChangeVisibility(int testId)
        {   
            try
            {
               CertificationTest test=await _certificationRepo.Get(testId);
               test.IsActive=!test.IsActive;
               await _certificationRepo.Update(test);
                return new TestVisibleResponseDTO()
                {
                    Code = 200,
                    Status = test.IsActive,
                    Message = "Update successful",
                };

            }
            catch
            {
                throw;
            }

        }
        public async Task<List<AdminAllTestDTO>> GetAllTest()
        {
            try
            {
                var tests = await _certificationRepo.Get();
                List<AdminAllTestDTO> adminAllTests = new List<AdminAllTestDTO>();
                foreach (var test in tests)
                {
                    string testDifficult = test.dificultyLeavel;
                    if (test.TestTakenCount > 10)
                    {
                        if (test.PassCount == 0)
                        {
                            testDifficult = "Hard";
                        }
                        else
                        {
                            double percent = (double)test.PassCount / test.TestTakenCount * 100;
                            if (percent >= 80)
                            {
                                testDifficult = "Easy";
                            }
                            else if (percent >= 60)
                            {
                                testDifficult = "Medium";
                            }
                            else
                            {
                                testDifficult = "Hard";
                            }
                        }

                    }

                    AdminAllTestDTO testDTO = new AdminAllTestDTO()
                    {
                        TestId = test.TestId,
                        Difficulty = testDifficult,
                        Duration = test.TestDurationMinutes,
                        TestDescription = test.TestDescription,
                        IsActive = test.IsActive,
                        TestName = test.TestName,
                    };
                    adminAllTests.Add(testDTO);

                }
                return adminAllTests;
            }
            catch
            {
                throw;

            }
        }
        public async Task<SuccessCertificationTestCreatedDTO> CreateCertificationTest(CreateQuestionDTO createQuestion)
            {
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                    CertificationTest certificationTest = new CertificationTest()
                    {
                        CreatedDate = DateTime.Now,
                        TotalQuestionCount = createQuestion.questions.Count,
                        QuestionNeedTotake = createQuestion.AttendQuestionCount,
                        RetakeWaitDays = createQuestion.RetakeWaitDays,
                        TestDescription = createQuestion.TestDescription,
                        TestDurationMinutes = createQuestion.TestDuration,
                        TestName = createQuestion.CertificationName,
                        TestTakenCount = 0,
                        IsActive = createQuestion.IsActive,
                        PassCount=0,
                        dificultyLeavel=createQuestion.DificultyLeavel,
                        ImageUrl="",
                        
                    };
                    await _certificationRepo.Add(certificationTest);
                    string fileExtension = Path.GetExtension(createQuestion.TestImage.FileName);
                    string blobName = $"product-{certificationTest.TestId}{fileExtension}";
                    string imageUrl = await _blobService.UploadImageAsync(createQuestion.TestImage, blobName);

                    certificationTest.ImageUrl = imageUrl;
                   await _certificationRepo.Update(certificationTest);


                    await CreateQuestion(createQuestion.questions, certificationTest.TestId);
                    await _transactionService.CommitTransactionAsync();
                    SuccessCertificationTestCreatedDTO successCertification = new SuccessCertificationTestCreatedDTO()
                    {
                        Message = "Certification Test Created Sucessfully",

                    };
                    return successCertification;
                    
                }
                catch
                {
                    await _transactionService.RollbackTransactionAsync();
                    throw;
                }

            }
        }
    }
}
