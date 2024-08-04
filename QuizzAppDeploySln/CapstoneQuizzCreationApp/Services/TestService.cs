using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using CapstoneQuizzCreationApp.Models.DTO.RequestDTO;
using CapstoneQuizzCreationApp.Models.DTO.ResponseDTO;
using CapstoneQuizzCreationApp.Repositories.JoinedRepository;
using System;
using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;

namespace CapstoneQuizzCreationApp.Services
{
    public class TestService:ITestService
    {
        private readonly IRepository<int, Question> _questionRepo;
        private readonly IRepository<int,Option> _optionRepo;
        private readonly IRepository<int, Certificate> _certificateRepo;
        private readonly IRepository<int, Submission> _submissionRepo;
        private readonly IRepository<int,SubmissionAnswer> _submissionAnswerRepo;
        private readonly CertificationTestQuestionRepository _certificationTestQuestionRepo;
        private readonly ITransactionService _transactionService;
        public readonly IRepository<int, CertificationTest> _certificateTestRepo;
        private readonly UserTestHIstoryRepository _userTestHIstory;
        private readonly IRepository<int,TestHistory> _testHistoryRepo;
        private readonly SubmissionAnswerQuestionOnly _submissionAnswerQuestionOnly;
        private readonly SubmissionTestQuestionRepository _submissionQuestionRepo;
        private readonly HistoryWithUserRepository _historyWithUserRepo;
        private readonly IRepository<int, User> _userRepository;
        private readonly CertificationTestOnlyQuestionRepo _certificationTestOnlyQuestionRepo;
        public TestService(IRepository<int, Question> questionRepo,
            IRepository<int, Option> optionRepo,
            IRepository<int, Submission> submissionRepo,
            IRepository<int, SubmissionAnswer> submissionAnswerRepo,
            CertificationTestQuestionRepository certificationTestQuestionRepo,
            ITransactionService transactionService,
            UserTestHIstoryRepository userTestHIstory,
            IRepository<int, TestHistory> testHistoryRepo,
            SubmissionTestQuestionRepository submissionQuestionRepo,
            SubmissionAnswerQuestionOnly submissionAnswerQuestionOnly,
            IRepository<int, Certificate> certificateRepo,
            CertificationTestOnlyQuestionRepo certificationTestOnlyQuestionRepo,

            IRepository<int, CertificationTest> certificateTestRepo,
            HistoryWithUserRepository historyWithUserRepository,
            IRepository<int,User> uerRepo

            )
        {
            _questionRepo = questionRepo;
            _optionRepo = optionRepo;
            _submissionRepo = submissionRepo;
            _submissionAnswerRepo = submissionAnswerRepo;
            _certificationTestQuestionRepo = certificationTestQuestionRepo;
            _transactionService = transactionService;
            _userTestHIstory = userTestHIstory;
            _testHistoryRepo = testHistoryRepo;
            _submissionQuestionRepo = submissionQuestionRepo;
            _submissionAnswerQuestionOnly = submissionAnswerQuestionOnly;
            _certificateRepo = certificateRepo;
            _certificateTestRepo = certificateTestRepo;
            _historyWithUserRepo=historyWithUserRepository;
            _userRepository = uerRepo;
            _certificationTestOnlyQuestionRepo = certificationTestOnlyQuestionRepo;

        }
        public async Task<StartTestDTO> StartTest(int certificationTestId, int UserId)
        {
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {

                    TestHistory history = (await _userTestHIstory.Get(UserId)).TestHistories.FirstOrDefault(h => h.TestId == certificationTestId);
                    var test = (await _certificationTestOnlyQuestionRepo.Get(certificationTestId));
                    Random random = new Random();
                    var allquestions = test.Questions;
                    var questions = allquestions.OrderBy(q => random.Next()).Take(test.QuestionNeedTotake);
                    DateTime now = DateTime.Now;
                    DateTime testEndTime = now.AddMinutes(test.TestDurationMinutes);
                     if (history != null && DateTime.Now <= history.LatesttestEndTime)
                     {
                        throw new Exception("Test need to Resume no need to start");
                     }
                    else if (history != null)
                    {
                        DateTime retakedate = history.LatesttestEndTime.AddDays(test.RetakeWaitDays);
                        if (DateTime.Now < retakedate)
                        {
                            throw new Exception("User already taken please wait till waiting time is over");
                        }

                    }
                          
              
                    Submission submission = new Submission()
                    {
                        SubmissionTime = testEndTime,
                        TestId = certificationTestId,
                        TimeTaken = 0,
                        TotalScore = test.QuestionNeedTotake,
                        ObtainedScore=0,
                        UserId = UserId,
                        StartTime=DateTime.Now,             
                    };
                    await _submissionRepo.Add(submission);

                    if (history != null)
                    {
                        history.LatesttestEndTime = DateTime.Now.AddMinutes(test.TestDurationMinutes);
                        history.LatestSubmissionId = submission.SubmissionId;
                        history.LatestIsSubmited = false;
                        await _testHistoryRepo.Update(history);
                        
                    }
                    else
                    {
                        test.TestTakenCount += 1;
                        TestHistory testHistory = new TestHistory()
                        {
                            UserId = UserId,
                            TestId = certificationTestId,
                            LatestSubmissionId=submission.SubmissionId,
                            LatesttestEndTime = testEndTime, 
                           LatestIsSubmited=false,
                        };
                        await _testHistoryRepo.Add(testHistory);
                        await _certificateTestRepo.Update(test);
                    }
             
                    foreach (var question in questions)
                    {

                        SubmissionAnswer submissionAnswer = new SubmissionAnswer()
                        {
                            IsMarked = false,
                            SubmissionId = submission.SubmissionId,
                            QuestionId = question.QuestionId,
                            IsCorrect = false,
                            
                            
                        };
                        await _submissionAnswerRepo.Add(submissionAnswer);
                    
                    }

                    await _transactionService.CommitTransactionAsync();
                
                    return new StartTestDTO()
                    {
                        Code=200,
                        SubmissionId=submission.SubmissionId,
                        Message="Test Starteed Successfully"
                    }; 


                }
                catch
                {
                   await _transactionService.RollbackTransactionAsync();
                    throw;
                }
 

            }

        }
        public async Task<TestPreviewDTO> TestPreviewPage(int testId, int userId)
        {
            try
            {
                TestHistory history = (await _userTestHIstory.Get(userId)).TestHistories.FirstOrDefault(h => h.TestId == testId);
                var test = (await _certificationTestQuestionRepo.Get(testId));
                DateTime now = DateTime.Now;
                DateTime testEndTime = now.AddMinutes(test.TestDurationMinutes);
                bool isResume = false;
                bool isWait = false;
                bool isPending=false;
                DateTime LastEndTime=DateTime.Now;
                DateTime NextTakeTime = DateTime.Now;
                int submissionId = 0;
                
                if (history != null && DateTime.Now <= history.LatesttestEndTime)
                {
                    isResume=true;
                    submissionId = history.LatestSubmissionId;
                    
                    
                }
                else if(history != null && !history.LatestIsSubmited)
                {
                    isPending = true;
                    submissionId=history.LatestSubmissionId;    
                }
                else if (history != null)
                {
                    DateTime retakedate = history.LatesttestEndTime.AddDays(test.RetakeWaitDays);

                    
                    if (DateTime.Now < retakedate)
                    {
                        
                            isWait = true;
                            LastEndTime = history.LatesttestEndTime;
                            NextTakeTime = retakedate;
                        
                        
                    }

                }

                TestPreviewDTO testPreview = new TestPreviewDTO()
                {
                    Duration=test.TestDurationMinutes,
                    IsResume=isResume,
                    IsWait=isWait,
                    IsPending=isPending,
                    LastTakenTime=LastEndTime,
                    NextTestTime=NextTakeTime,
                    PassMark=6,
                    TestId=test.TestId,
                    TestName=test.TestName,
                    TotalMark=test.QuestionNeedTotake,
                    TotalQuestion=test.QuestionNeedTotake,
                    SubmissionId=submissionId,  
                    

                };
                return testPreview; 


            }
            catch
            {
                throw;
            }
        }
        public async Task<QuestionWithExpiryDate> ResumeTest(int SubmissionId, int userId )
        {
            try
            {
                Submission submission = (await _submissionQuestionRepo.Get(SubmissionId));
                if(submission.UserId != userId)
                {
                    throw new Exception("Submission not found for user");
                }
                if(submission.IsSubmited)
                {
                    QuestionWithExpiryDate questionWithExpiry = new QuestionWithExpiryDate()
                    {
                        SubmissionId = submission.SubmissionId,
                        IsSubmited = submission.IsSubmited,
                        TestEndTime = submission.SubmissionTime,
                        testQuestion = new List<TestQuestionDTO>(),

                    };
                    return questionWithExpiry;
                   
                }
              
                if(submission.SubmissionTime< DateTime.Now)
                {
                    QuestionWithExpiryDate questionWithExpiry = new QuestionWithExpiryDate()
                    {
                        SubmissionId = submission.SubmissionId,
                        IsSubmited = true,
                        TestEndTime = submission.SubmissionTime,
                        testQuestion = new List<TestQuestionDTO>(),

                    };
                    return questionWithExpiry;

                }
                var questions = submission.SubmissionAnswers;

                List<TestQuestionDTO> questionDTOs = new List<TestQuestionDTO>();
                foreach (var question in questions)
                {
                    

                    List<OptionDTO> optionDTOs = new List<OptionDTO>();
                    foreach (var option in question.Question.Options)
                    {
                        OptionDTO optionDTO = new OptionDTO()
                        {
                            OptionId = option.OptionId,
                            OptionName = option.OptionName,
                        };
                        optionDTOs.Add(optionDTO);
                    }
                    TestQuestionDTO testQuestionDTO = new TestQuestionDTO()
                    {
                        QuestionDescription = question.Question.QuestionDescription,
                        QuestionId = question.QuestionId,
                        SubmissionAnswerId = question.AnswerId,
                        Options = optionDTOs,
                        QuestionType= question.Question.QuestionType,
                        SelectedAnswer=question.Option,
                        IsFlagged=question.IsMarked,
                        
                    };
                    questionDTOs.Add(testQuestionDTO);

                }
                return new QuestionWithExpiryDate
                {
                    SubmissionId = submission.SubmissionId,
                    TestEndTime = submission.SubmissionTime,
                     TestDuration=submission.CertificationTest.TestDurationMinutes,
                     TestName=submission.CertificationTest.TestName,
                    testQuestion = questionDTOs
                };


            }
            catch
            {
               
                throw;
            }
        }
     
        public async Task<SuccessSynchronieDTO> SynchronizeDb(List<SynchronousDataDTO> answers) 
        {
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                    foreach(var answer in answers)
                    {
                        SubmissionAnswer newAnswer = await _submissionAnswerRepo.Get(answer.AnswerId);
                        newAnswer.Option=answer.AnswerName;
                        newAnswer.IsMarked=answer.IsFlaged;
                        await _submissionAnswerRepo.Update(newAnswer);
                    }
                    await _transactionService.CommitTransactionAsync();
                    return new SuccessSynchronieDTO()
                    {
                        Code = 200,
                        Message = "Synchronization Sucess"

                    };



                }
                catch
                {
                    await _transactionService.RollbackTransactionAsync();
                    throw;

                }


            }


        }
        private async Task<TestResultDTO> ReturnSubmitedResult(Submission submission)
        {
            try
            {
                TestHistory history = (await _userTestHIstory.Get(submission.UserId)).TestHistories.FirstOrDefault(h => h.TestId == submission.TestId);
                int MaxObtainedScore = history.MaxObtainedScore;



                TestResultDTO testResult = new TestResultDTO()
                {
                    IsPassed = submission.IsPassed,
                    MaxObtainedScore = MaxObtainedScore,
                    ObtainedScore = submission.ObtainedScore,
                    TotalScore = submission.TotalScore,
                    CertificateId = history.CertificateId ?? 0,
                    TestName = (await _certificateTestRepo.Get(submission.TestId)).TestName,
                    TotalTimeTaken = submission.TimeTaken,
                    TestId=submission.TestId,

                };
                return testResult;

            }
            catch
            {
                throw;

            }

        }
        public async Task<TestResultDTO> SubmitAnswer(SubmissionAnswerDTO submissionAnswer)
        {
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {

                    Submission submission=  (await _submissionAnswerQuestionOnly.Get(submissionAnswer.SubmissionId));
                    if (submission.UserId != submissionAnswer.UserId)
                    {
                        throw new SubmissionAnswerNotFoundException("No submission found for user");

                    }
                    if (submission.IsSubmited)
                    {
                        return await ReturnSubmitedResult(submission);
                    }
                    int totalScore=0;
                    int certificateId =0;   
                    int obtainedScore = 0;
                    foreach(var answer in submission.SubmissionAnswers)
                    {
                        totalScore += answer.Question.Points;
                        if (answer.Option == answer.Question.CorrectAnswer)
                        {
                            obtainedScore += answer.Question.Points;
                        }
                    }
                    submission.ObtainedScore= obtainedScore;

                    TimeSpan timeDifference = DateTime.Now - submission.StartTime;
                    submission.TimeTaken = timeDifference.TotalSeconds;
                    submission.IsSubmited = true;
                   
                    TestHistory history= (await _userTestHIstory.Get(submissionAnswer.UserId)).TestHistories.FirstOrDefault(h=>h.TestId==submission.TestId);
                   
                    int MaxObtainedScore=obtainedScore;
                    history.LatestIsSubmited= true;
                    double result = (Convert.ToDouble( obtainedScore) / Convert.ToDouble( totalScore)) * 100;
                    var test = await _certificateTestRepo.Get(submission.TestId);

                    if (result > 30)
                    {
                       
                        submission.IsPassed = true;
                        if (history.IsPassed)
                        {
                           
                           Certificate certificate=await _certificateRepo.Get(history.CertificateId?? 0);
                            if (submission.TimeTaken <= (test.TestDurationMinutes * 60) / 2)
                            {
                                certificate.IsFastAchiver = true;
                            }
                            if (certificate.MaxObtainedScore < obtainedScore)
                            {
                                certificate.MaxObtainedScore= obtainedScore;
                                certificate.SubmissionId = submission.SubmissionId;
                                certificate.TimeTaken = submission.TimeTaken;
                                history.MaxObtainedScore = obtainedScore;                                
                                history.PassSubmissionId = submission.SubmissionId;
                                history.TimeTaken = submission.TimeTaken;
                                history.SubmissionTIme = DateTime.Now;
                            }
                           certificateId=certificate.CertificateId;
                           MaxObtainedScore=certificate.MaxObtainedScore;

                           await _certificateRepo.Update(certificate);
                            
                        }
                        else
                        {
                          
                            test.PassCount += 1;
                            
                          await  _certificationTestQuestionRepo.Update(test);    
                            Certificate certificate = new Certificate()
                            {
                                ProvidedDate = DateTime.Now,
                                SubmissionId = submission.SubmissionId,
                                TestId = submission.TestId,
                                UserId = submission.UserId,
                                MaxObtainedScore=obtainedScore,
                            };
                            if (submission.TimeTaken <= (test.TestDurationMinutes * 60) / 2)
                            {
                                certificate.IsFastAchiver = true;
                            }

                            await _certificateRepo.Add(certificate);
                            history.IsPassed = true;
                            history.CertificateId=certificate.CertificateId;
                            history.MaxObtainedScore=certificate.MaxObtainedScore;
                            history.PassSubmissionId = submission.SubmissionId;
                            history.TimeTaken = submission.TimeTaken;
                            history.SubmissionTIme=DateTime.Now;    
                            certificateId=certificate.CertificateId;

                        }
                                     

                    }
                    else
                    {
                        if (history.MaxObtainedScore < obtainedScore)
                        {
                            history.MaxObtainedScore=obtainedScore;
                        }
                    }
                    history.LatesttestEndTime = DateTime.Now;
                    await _submissionRepo.Update(submission);
                    await _testHistoryRepo.Update(history);
                    TestResultDTO resultDTO = new TestResultDTO()
                    {
                        IsPassed = submission.IsPassed,
                        ObtainedScore = obtainedScore,
                        TotalScore = totalScore,
                        TestName= (await _certificateTestRepo.Get(submission.TestId)).TestName,
                        MaxObtainedScore = MaxObtainedScore,
                        CertificateId=certificateId,
                        TotalTimeTaken=submission.TimeTaken,
                        TestId = submission.TestId,
                    };
                    
                    await _transactionService.CommitTransactionAsync();

                    return resultDTO;

                }
                catch
                {
                    await _transactionService.RollbackTransactionAsync();
                    throw;
                }


            }
        }
        public async Task<List<LeaderBoardDTO>> GetLeaderBoard(int testId)
        {
            try
            {
                
                var testHistories = (await _historyWithUserRepo.Get())
                    .Where(h => h.TestId == testId && h.IsPassed)
                    .OrderByDescending(h => h.MaxObtainedScore)
                    .ThenBy(h => h.TimeTaken)
                    .ToList();

                var result = testHistories
                    .Select((item, index) => new LeaderBoardDTO
                    {
                        SubmissionTime = item.SubmissionTIme ?? DateTime.Now,
                        MaxObtainedMark = item.MaxObtainedScore,
                        Rank = index + 1,
                        TimeTaken = item.TimeTaken ?? 0,
                        UserId = item.UserId,
                        UserName = item.User.Name
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching the leaderboard.", ex);
            }
        }

        public async Task<TestStatsDTO> GetTestStats(int userId, int testId)
        {
            try
            {
                
                var history = (await _testHistoryRepo.Get())
                    .Where(h => h.IsPassed && h.TestId == testId).ToList();
                var userHistoryData = await _userTestHIstory.Get(userId);
                var userHistory = userHistoryData.TestHistories.FirstOrDefault(h => h.TestId == testId);
                var test = await _certificateTestRepo.Get(testId);

                if (userHistory == null)
                {
                    throw new Exception("User test history not found.");
                }

                int rank = 1;

                if (userHistory.IsPassed)
                {
                    rank = history.Count(h => h.MaxObtainedScore > userHistory.MaxObtainedScore) + 1;
                }
                else
                {
                    rank = 0;
                }

                TestStatsDTO testStats = new TestStatsDTO()
                {
                    IsPassed = userHistory.IsPassed,
                    MaxMark = userHistory.MaxObtainedScore,
                    MyRank = rank,
                    PassCount = test.PassCount,
                    TestTakenCount = test.TestTakenCount,
                    TotalMark=test.QuestionNeedTotake,
                };

                return testStats;
            }
            catch (Exception ex)
            {
                // Log the exception (if a logging framework is in place)
                throw ;
            }
        }


        public async Task<List<TestSubmissionDTO>> GetUserSubmission(int userId,int testId)
        {
            try
            {
              var submissions=  (await _submissionRepo.Get()).Where(s=>s.UserId==userId && s.TestId==testId);
                List<TestSubmissionDTO> submissionDTOs = new List<TestSubmissionDTO>();
               foreach(var submission in submissions)
                {
                    TestSubmissionDTO testSubmissionDTO = new TestSubmissionDTO()
                    {
                        SubmissionDate = submission.SubmissionTime,
                        SubmissionId = submission.SubmissionId,
                        ObtainedScore = submission.ObtainedScore,
                        IsPassed = submission.IsPassed,
                        TimeTaken = submission.TimeTaken,
                    };
                    submissionDTOs.Add(testSubmissionDTO);
                }
                return submissionDTOs;

            }
            catch
            {
                throw;
            }
        }
        public async Task<CertificateDataDTO> GetCertificateData(int userId,int testId)
        {
            try
            {
                var history = (await _userTestHIstory.Get(userId)).TestHistories.FirstOrDefault(h => h.TestId == testId);
                CertificateDataDTO dataDTO = new CertificateDataDTO()
                {
                    IsPassed = false,
                    CertificateId = 0,
                    ProvidedDate = DateTime.Now,
                    ObtainedScore = 0,
                    CertificateTestName = "",
                    TimeTaken = 0,
                    UserName = ""
                };
                if (history == null)
                {
                    return dataDTO;

                }
                else if (!history.IsPassed)
                {
                    return dataDTO;
                }
                else
                {
                    var certificateTest = (await _certificateTestRepo.Get(history.TestId));
                    dataDTO.ObtainedScore = history.MaxObtainedScore;
                    dataDTO.IsPassed = true;
                    dataDTO.TimeTaken = history.TimeTaken ??0;
                    dataDTO.CertificateTestName = certificateTest.TestName;
                    dataDTO.CertificateId = history.CertificateId ?? 0;
                    dataDTO.UserName =( await _userRepository.Get(history.UserId)).Name;
                    dataDTO.TotalMark=certificateTest.QuestionNeedTotake;
                    return dataDTO;
                }


            }
            catch
            {
                throw;
            }
      

        } 

    }
}
