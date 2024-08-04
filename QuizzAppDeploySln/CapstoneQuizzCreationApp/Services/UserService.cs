using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using CapstoneQuizzCreationApp.Models.DTO.ResponseDTO;
using CapstoneQuizzCreationApp.Repositories.JoinedRepository;

namespace CapstoneQuizzCreationApp.Services
{
    public class UserService:IUserService
    {
        private readonly IRepository<int, TestHistory> _testHistoryRepo;
        private readonly IRepository<int, Favourite> _favouriteRepository;
        private readonly IRepository<int, User> _userRepository;
        private readonly UserTestHIstoryRepository _userTestHIstory;
        private readonly UserFavouriteRepository _userFavouriteRepository;
        private readonly UserHistoryFavouriteRepository _userHistoryFavouriteRepository;
        private readonly IRepository<int,CertificationTest> _certificationTestRepository;
        private readonly UserFavouriteTestRepository _userFavouriteTestRepository;
        private readonly ITransactionService _transactionService;
        private readonly UserHistoryTest _userHistoryTest;
        public UserService(IRepository<int, CertificationTest> certificateTestRepository,
            IRepository<int,TestHistory> testHistory,
            IRepository<int,User> userRepository,
            UserTestHIstoryRepository userTestHIstory,
             ITransactionService transactionService,
             IRepository<int,Favourite> favouriteRepository,
             UserHistoryFavouriteRepository userHistoryFavourite,
             UserFavouriteRepository userFavourite,
             UserHistoryTest userHistoryTest,
             UserFavouriteTestRepository userFavouriteTestRepository
            )
        {
            _certificationTestRepository = certificateTestRepository;
            _userRepository = userRepository;
            _testHistoryRepo = testHistory;
            _userTestHIstory = userTestHIstory;
            _transactionService = transactionService;
            _favouriteRepository=favouriteRepository;
            _userHistoryFavouriteRepository = userHistoryFavourite;
            _userFavouriteRepository = userFavourite;
            _userHistoryTest = userHistoryTest;
            _userFavouriteTestRepository= userFavouriteTestRepository;
        }

        public async Task<List<AllTestsDTO>> GetAllTestsWithUsers(int userId)
        {
            try
            {
                var allTests = (await _certificationTestRepository.Get()).Where(t=>t.IsActive);
                var user = await _userHistoryFavouriteRepository.Get(userId);
                var history = user.TestHistories.ToDictionary(h => h.TestId);
                var favourites = user.Favourites.ToHashSet();

                var allTestsDTOs = allTests.Select(test =>
                {
                    var isAttend = history.TryGetValue(test.TestId, out var testHistory);
                    var isPassed = isAttend && testHistory.IsPassed;
                    var isResume = isAttend && testHistory.LatesttestEndTime > DateTime.Now;
                    var isPending = isAttend && testHistory.LatesttestEndTime < DateTime.Now && !testHistory.LatestIsSubmited;
                    var favourite = favourites.FirstOrDefault(f => f.TestId == test.TestId);
                    var isLiked = favourite != null;
                    var isWait = isAttend && (testHistory.LatesttestEndTime).AddDays(test.RetakeWaitDays) > DateTime.Now;
                    string testDifficult = test.dificultyLeavel;
                    if (test.TestTakenCount > 5)
                    {
                        if (test.PassCount == 0)
                        {
                            testDifficult = "Hard";
                        }
                        else
                        {
                            double percent = (double)test.PassCount / test.TestTakenCount * 100;
                            if (percent>=80)
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
                    

                    return new AllTestsDTO
                    {
                        TestDifficult = testDifficult,
                        Description = test.TestDescription,
                        Duration = test.TestDurationMinutes,
                        IsAttend = isAttend,
                        IsPassed = isPassed,
                        IsPending = isPending,
                        IsResume = isResume,
                        PassCount = test.PassCount,
                        TestTakenCount = test.TestTakenCount,
                        TestId = test.TestId,
                        TestName = test.TestName,
                        IsFavourite = isLiked,
                        IsWait=isWait,
                        ImageUrl= test.ImageUrl,    
                        FavouriteId=favourite?.FavouriteId ?? 0,
                    };
                }).ToList();

                return allTestsDTOs;
            }
            catch
            {
                throw;
            }
        }

        public async Task<UpdateFavouriteDTO> AddToFavourate(int userId,int TestId)
        {
             try
              {
                    var user= await _userFavouriteRepository.Get(userId);
                    var favourites = user.Favourites;
            
                    foreach(var favourite in favourites)
                    {
                        if(favourite.TestId == TestId)
                        {
                            throw new DuplicateValueException("Duplicate favourite found");
                        }
                        
                    }
                    Favourite favourite1 = new Favourite()
                    {
                        AddedDate=DateTime.Now,
                        TestId=TestId,
                        UserId=userId,
                        
                    };
                   await _favouriteRepository.Add(favourite1);
                return new UpdateFavouriteDTO()
                {
                    Code = 200,
                    FavouriteId = favourite1.FavouriteId,
                    IsFavourite = true,
                };
                   
                }
                catch
                {
                      throw;

                };
            
        }
        public async Task<UpdateFavouriteDTO> RemoveFromFavourite(int userId ,int favouriteId)
        {
            try
            {
               var Favourite=await _favouriteRepository.Get(favouriteId);
                if (Favourite.UserId != userId)
                {
                    throw new FavouriteNotFoundException("The user does not have such favouriteId");
                }
                await _favouriteRepository.Delete(favouriteId);
                return new UpdateFavouriteDTO()
                {
                    Code = 200,
                    FavouriteId = 0,
                    IsFavourite = false,
                };
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<AllTestsDTO>> GetUserHistory(int userId)
        {
            try
            {
               var user= await _userHistoryTest.Get(userId);
                var favourites=user.Favourites.ToHashSet();
                var histories = user.TestHistories;
                List<AllTestsDTO> result = new List<AllTestsDTO>();
                foreach (var history in histories)
                {
                    var test = history.CertificationTest;
                    var isAttend = true;
                    var isPassed = isAttend && history.IsPassed;
                    var isPending=isAttend && history.LatesttestEndTime<DateTime.Now && !history.LatestIsSubmited;
                    var isResume = isAttend && history.LatesttestEndTime > DateTime.Now;
                    var favourite = favourites.FirstOrDefault(f => f.TestId == test.TestId);
                    var isLiked = favourite != null;
                    var isWait = isAttend && (history.LatesttestEndTime).AddDays(test.RetakeWaitDays) > DateTime.Now;
                    string testDifficult = test.dificultyLeavel;

                    if (test.TestTakenCount > 5)
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


                    AllTestsDTO allTestsDTO= new AllTestsDTO
                    {
                        TestDifficult = testDifficult,
                        Description = test.TestDescription,
                        Duration = test.TestDurationMinutes,
                        IsAttend = isAttend,
                        IsPassed = isPassed,
                        IsResume = isResume,
                        PassCount = test.PassCount,
                        TestTakenCount = test.TestTakenCount,
                        TestId = test.TestId,
                        TestName = test.TestName,
                        IsFavourite = isLiked,
                        IsWait = isWait,
                        IsPending=isPending,
                        FavouriteId = favourite?.FavouriteId ?? 0,
                        IsActive=test.IsActive,
                        ImageUrl = test.ImageUrl,
                    };
                    result.Add(allTestsDTO);
                    
                }
                return result;


            }
            catch
            {
                throw;
            }
        }
        public async Task<List<AllTestsDTO>> GetMyFavourite(int userId)
        {
            try
            {
                var user = await _userFavouriteTestRepository.Get(userId);
               
                var favourites = user.Favourites.Where(f=>f.CertificationTest.IsActive);
                if (favourites.Count() == 0)
                {
                    throw new EmptyItemException("The Favourite is Empty");
                }
                var histories = user.TestHistories.ToDictionary(h => h.TestId);
                List<AllTestsDTO> result = new List<AllTestsDTO>();
                foreach (var favouriteitem in favourites)
                {
                    var test=favouriteitem.CertificationTest;                    
                    var isAttend = histories.TryGetValue(test.TestId, out var testHistory);
                    var isPassed = isAttend && testHistory.IsPassed;
                    var isResume = isAttend && testHistory.LatesttestEndTime > DateTime.Now;
                    var favourite = favourites.FirstOrDefault(f => f.TestId == test.TestId);
                    var isLiked = favourite != null;
                    var isPending = isAttend && testHistory.LatesttestEndTime < DateTime.Now && !testHistory.LatestIsSubmited;
                    var isWait = isAttend && (testHistory.LatesttestEndTime).AddDays(test.RetakeWaitDays) > DateTime.Now;
                    string testDifficult = test.dificultyLeavel;
                    if (test.TestTakenCount > 5)
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


                    AllTestsDTO allTestsDTO = new AllTestsDTO
                    {
                        TestDifficult = testDifficult,
                        Description = test.TestDescription,
                        Duration = test.TestDurationMinutes,
                        IsAttend = isAttend,
                        IsPassed = isPassed,
                        IsResume = isResume,
                        PassCount = test.PassCount,
                        TestTakenCount = test.TestTakenCount,
                        TestId = test.TestId,
                        TestName = test.TestName,
                        IsFavourite = isLiked,
                        IsWait = isWait,
                        IsPending= isPending,
                        FavouriteId = favourite?.FavouriteId ?? 0,
                        ImageUrl=test.ImageUrl,
                    };
                    result.Add(allTestsDTO);

                }
                return result;


            }
            catch
            {
                throw;
            }
        }
        public async Task<List<AllTestsDTO>> GetPopularTests(int userId)
        {
            try
            {
                var allTests = (await _certificationTestRepository.Get()).Where(t=>t.IsActive).OrderByDescending(t=>t.TestTakenCount).Take(4);
                var user = await _userHistoryFavouriteRepository.Get(userId);
                var history = user.TestHistories.ToDictionary(h => h.TestId);
                var favourites = user.Favourites.ToHashSet();

                var allTestsDTOs = allTests.Select(test =>
                {
                    var isAttend = history.TryGetValue(test.TestId, out var testHistory);
                    var isPassed = isAttend && testHistory.IsPassed;
                    var isResume = isAttend && testHistory.LatesttestEndTime > DateTime.Now;
                    var isPending = isAttend && testHistory.LatesttestEndTime < DateTime.Now && !testHistory.LatestIsSubmited;
                    var favourite = favourites.FirstOrDefault(f => f.TestId == test.TestId);
                    var isLiked = favourite != null;
                    var isWait = isAttend && (testHistory.LatesttestEndTime).AddDays(test.RetakeWaitDays) > DateTime.Now;
                    string testDifficult = test.dificultyLeavel;
                    if (test.TestTakenCount > 5)
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


                    return new AllTestsDTO
                    {
                        TestDifficult = testDifficult,
                        Description = test.TestDescription,
                        Duration = test.TestDurationMinutes,
                        IsAttend = isAttend,
                        IsPassed = isPassed,
                        IsResume = isResume,
                        PassCount = test.PassCount,
                        TestTakenCount = test.TestTakenCount,
                        TestId = test.TestId,
                        TestName = test.TestName,
                        IsFavourite = isLiked,
                        IsPending= isPending,
                        IsWait = isWait,
                        FavouriteId = favourite?.FavouriteId ?? 0,
                        ImageUrl=test.ImageUrl
                    };
                }).ToList();

                return allTestsDTOs;
            }
            catch
            {
                throw;
            }
        }
    }
}
