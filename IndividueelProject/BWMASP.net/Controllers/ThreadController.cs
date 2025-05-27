using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMW.ASP.Models;
using BMWDomain.Exceptions;


namespace BMW.ASP.Controllers
{
    public class ThreadController : Controller
    {
        private readonly IDiscussionThreadContainer _discussionThreadContainer;
        private readonly IUserContainer _userContainer;
        private readonly ITopicContainer _topicContainer;
        private readonly ICommentContainer _commentContainer;

        public ThreadController(IDiscussionThreadContainer discussionThreadContainer, IUserContainer userContainer, ITopicContainer topicContainer, ICommentContainer commentContainer)
        {
            this._discussionThreadContainer = discussionThreadContainer;
            this._userContainer = userContainer;
            this._topicContainer = topicContainer;
            this._commentContainer = commentContainer;
        }

        public IActionResult Index()
        {
            List<ThreadViewModel> threads = new List<ThreadViewModel>();

            try
            {
                List<DiscussionThread> rawThreads = _discussionThreadContainer.GetAllThreads();

                foreach (var thread in rawThreads)
                {
                    User user = _userContainer.GetUserById(thread.OwnerId);
                    var topic = _topicContainer.GetTopicById(thread.TopicId);

                    threads.Add(new ThreadViewModel
                    {
                        ThreadId = thread.ThreadId,
                        Title = thread.Title,
                        Text = thread.Text,
                        OwnerId = thread.OwnerId,
                        UserName = user.Name,
                        CreatedAt = thread.CreatedAt.Date.ToString("yyyy-MM-dd"),
                        TopicName = topic.Name,
                        IsEdited = thread.IsEdited
                    });
                }
            }
            catch (BllException e)
            {
                TempData["Error"] = e.Message ;
            }
            catch(DataBaseException e)
            {
                TempData["Error"] =e.Message ;
            }
            catch (Exception )
            {
                TempData["Error"] = "An error occurred while getting the threads.";
            }
            return View(threads);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                try
                {
                    List<Topic> rawTopics = _topicContainer.GetAllTopic();

                    ThreadViewModel thread = new ThreadViewModel
                    {
                        Topics = rawTopics
                    };

                    return View(thread);

                }
                catch (BllException e)
                {
                    TempData["Error"] = e.Message ;
                }
                catch(DataBaseException e)
                {
                    TempData["Error"] = e.Message ;
                }
                catch (Exception )
                {
                    TempData["Error"] = "An error occurred while getting the topics.";

                }

                return View();



            }
        }

        [HttpPost]
        public IActionResult Create(ThreadViewModel thread)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    DateTimeOffset parsedDate;
                    DateTimeOffset.TryParse(thread.CreatedAt, out parsedDate);
                    
                    int? ownerid = HttpContext.Session.GetInt32("UserId");
                    
                    if (ownerid == null)
                    {
                        return RedirectToAction("Login", "Login");
                    }
                    
                    


                    DiscussionThread createdThread = new DiscussionThread
                    (    0,
                        thread.Title!,
                        thread.Text!,
                        ownerid.Value,
                        parsedDate,
                        thread.TopicId,
                        false
                    );



                    _discussionThreadContainer.CreateThread(createdThread);

                    User user = _userContainer.GetUserById(createdThread.OwnerId);

                    var topic = _topicContainer.GetTopicById(thread.TopicId);


                    ThreadViewModel newThread = new ThreadViewModel
                    {
                        ThreadId = createdThread.ThreadId,
                        Title = createdThread.Title,
                        Text = createdThread.Text,
                        OwnerId = createdThread.OwnerId,
                        UserName = user.Name,
                        CreatedAt = thread.CreatedAt,
                        TopicName = topic.Name,
                        IsEdited = createdThread.IsEdited



                    };


                    return RedirectToAction("ReadMore", new { threadId = newThread.ThreadId });
                }
                catch (ValidationException ex)
                {

                    foreach (var error in ex.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }

                }
                catch (BllException e)
                {
                    TempData["Error"] = e.Message ;
                }
                catch(DataBaseException e)
                {
                    TempData["Error"] = e.Message ;
                }
                catch (Exception)
                {
                    TempData["Error"] = "An error occurred while creating the comment.";
                }


            }
            thread.Topics = _topicContainer.GetAllTopic();
            return View(thread);
        }

        public IActionResult ReadMore(int threadId)
        {
            DiscussionThread rawThread = _discussionThreadContainer.GetThreadById(threadId);

            User user = _userContainer.GetUserById(rawThread.OwnerId);

            var topic = _topicContainer.GetTopicById(rawThread.TopicId);

            ThreadViewModel thread = new ThreadViewModel
            {
                ThreadId = rawThread.ThreadId,
                Title = rawThread.Title,
                Text = rawThread.Text,
                OwnerId = rawThread.OwnerId,
                UserName = user.Name,
                TopicName = topic.Name,
                CreatedAt = rawThread.CreatedAt.Date.ToString("yyyy-MM-dd"),
                IsEdited = rawThread.IsEdited
            };
            
            var rawComments = _commentContainer.GetCommentsByThreadId(threadId);
            
            List<RootCommentViewModel> comments = new List<RootCommentViewModel>();

            foreach (var rootComment in rawComments.RootComments)
            {
                User commentOwner = _userContainer.GetUserById(rootComment.OwnerId);
                RootCommentViewModel comment = new RootCommentViewModel
                {
                    CommentId = rootComment.CommentId,
                    OwnerName = commentOwner.Name,
                    Id = rootComment.CommentId,
                    Text = rootComment.Text,
                    OwnerId = rootComment.OwnerId,
                    ThreadId = rootComment.ThreadId,
                    DateCreated = rootComment.CreatedAt.ToString("yyyy-MM-dd")
                };

                foreach (var childComment in rawComments.ChildComments)
                {
                    if (childComment.ParentId == rootComment.CommentId)
                    {
                        User childCommentOwner = _userContainer.GetUserById(childComment.OwnerId);
                        CommentViewModel child = new CommentViewModel
                        {
                            CommentId = childComment.CommentId,
                            OwnerName = childCommentOwner.Name,
                            Id = childComment.CommentId,
                            Text = childComment.Text,
                            OwnerId = childComment.OwnerId,
                            ThreadId = childComment.ThreadId,
                            DateCreated = childComment.CreatedAt.ToString("yyyy-MM-dd")
                        };
                        
                        comment.ChildComments.Add(child);
            
                    }
                }
                
                comments.Add(comment);
            }
            
            
            
            
            ThreadAndCommentsViewModel threadAndComments = new ThreadAndCommentsViewModel
            {
                Thread = thread,
                Comments = comments
            };
            
            return View(threadAndComments);
        }

        [HttpPost]
        public IActionResult Delete(int threadId)
        {
            try
            {
                _discussionThreadContainer.DeleteThreadById(threadId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the thread: " + ex.Message);
            }

            return RedirectToAction("ReadMore", new {  threadId });
        }


        [HttpGet]
        public IActionResult Edit(int threadId)
        {
            DiscussionThread rawThread = _discussionThreadContainer.GetThreadById(threadId);

            User user = _userContainer.GetUserById(rawThread.OwnerId);

            var topic = _topicContainer.GetTopicById(rawThread.TopicId);

            var topics = _topicContainer.GetAllTopic();

            ThreadViewModel thread = new ThreadViewModel
            {
                ThreadId = rawThread.ThreadId,
                Title = rawThread.Title,
                Text = rawThread.Text,
                OwnerId = rawThread.OwnerId,
                UserName = user.Name,
                TopicName = topic.Name,
                Topics = topics,
                CreatedAt = rawThread.CreatedAt.Date.ToString("yyyy-MM-dd"),
                IsEdited = rawThread.IsEdited
            };


            return View(thread);
        }

        [HttpPost]
        public IActionResult Edit(ThreadViewModel thread)
        {
            if (ModelState.IsValid)
            {
                int? threadId = thread.ThreadId;
                
                if (threadId == null)
                {
                    return RedirectToAction("Index");
                }
                int? ownerId = HttpContext.Session.GetInt32("UserId");
                if (ownerId == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                
                
                DiscussionThread editedThread = new DiscussionThread
                (
                    threadId.Value,
                    thread.Title!,
                    thread.Text!,
                    ownerId.Value,
                    DateTimeOffset.Now,
                    thread.TopicId,
                    true
                );
                try
                {
                    _discussionThreadContainer.EditThread(editedThread);

                    return RedirectToAction("ReadMore", new { threadId = editedThread.ThreadId });
                }
                catch (ValidationException ex)
                {
                    foreach (var error in ex.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
                catch (BllException e)
                {
                    TempData["Error"] = e.Message ;
                }
                catch(DataBaseException e)
                {
                    TempData["Error"] = e.Message ;
                }
                catch (Exception)
                {
                    TempData["Error"] = "An error occurred while editing the comment.";
                }
            }
            thread.Topics = _topicContainer.GetAllTopic();
            return View(thread);
        }

        [HttpPost]
        public IActionResult CreateComment(CreateCommentViewModel createComment)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int? ownerId = HttpContext.Session.GetInt32("UserId");
                    if (ownerId == null)
                    {
                        return RedirectToAction("Login", "Login");
                    }
                    
                    DateTimeOffset parsedDate;
                    DateTimeOffset.TryParse(createComment.DateCreated, out parsedDate);
                    
                    Comment newComment = new Comment
                    (
                        0,
                        createComment.Text!,
                        parsedDate,
                        ownerId.Value,
                        createComment.ThreadId,
                        createComment.ParentId
                    );

                    _commentContainer.CreateComment(newComment);

                    return RedirectToAction("ReadMore", new { threadId = newComment.ThreadId });
                }
                catch (ValidationException ex)
                {
                    foreach (var error in ex.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
                catch (BllException e)
                {
                    TempData["Error"] = e.Message ;
                }
                catch(DataBaseException e)
                {
                    TempData["Error"] = e.Message ;
                }
                catch (Exception)
                {
                    TempData["Error"] = "Comment is empty.";
                }
                
            }
            return RedirectToAction("ReadMore", new { threadId = createComment.ThreadId });
        }


        [HttpPost]
        public IActionResult LoadCommentPartial([FromBody] LoadCommentPartialRequest request)
        {
            var viewModel = new CreateCommentViewModel { ThreadId = request.ThreadId, ParentId = request.ParentId };
            return PartialView("_CreateCommentNesting", viewModel);
        }
    }


}
