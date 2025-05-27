using Microsoft.AspNetCore.Mvc;
using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMW.ASP.Models;

namespace BMW.ASP.Controllers
{
    public class ProfileController : Controller
    {
        
        private readonly ITopicContainer _topicContainer;
        private readonly IDiscussionThreadContainer _discussionThreadContainer;

        public ProfileController(ITopicContainer topicContainer,
            IDiscussionThreadContainer discussionThreadContainer)
        {
            this._topicContainer = topicContainer;
            this._discussionThreadContainer = discussionThreadContainer;
        }
        public IActionResult Index()
        {


            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            List<DiscussionThread> rawThreads = _discussionThreadContainer.GetThreadsByUserId(userId.Value);

            List<ProfileViewModel> threads = new List<ProfileViewModel>();



            foreach (var rawThread in rawThreads)
            {
                var topic = _topicContainer.GetTopicById(rawThread.TopicId);

                threads.Add(new ProfileViewModel
                {
                    ThreadId = rawThread.ThreadId,
                    Title = rawThread.Title,
                    Text = rawThread.Text,
                    CreatedAt = rawThread.CreatedAt.Date.ToString("yyyy-MM-dd"),
                    TopicId = rawThread.TopicId,
                    TopicName = topic.Name,
                    OwnerId = rawThread.OwnerId,
                    UserName = HttpContext.Session.GetString("UserName"),

                });
                
            }


            return View(threads);
        }


    }
}
