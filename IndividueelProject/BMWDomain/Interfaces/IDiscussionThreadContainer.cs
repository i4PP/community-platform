using BMWDALInterfacesAndDTOs.DTOs;
using BMWDomain.Entities;

namespace BMWDomain.interfaces;

    public interface IDiscussionThreadContainer
    {
        public void CreateThread(DiscussionThread thread);

        public List<DiscussionThread> GetAllThreads();

        public DiscussionThread GetThreadById(int id);

        public void DeleteThreadById(int id);

        public List<DiscussionThread> GetThreadsByUserId(int userId);

        public void EditThread(DiscussionThread thread);


    }

