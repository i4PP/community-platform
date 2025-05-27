using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDALInterfacesAndDTOs.Interfaces;
    public interface IDiscussionThreadRepository
    {
        public void CreateThread(DiscussionThreadDTO threadDto);

        public List<DiscussionThreadDTO> GetAllThreads();

        public DiscussionThreadDTO GetThreadById(int id);

        public DeletedResultDTO DeleteThreadById(int id);

        public List<DiscussionThreadDTO> GetThreadsByUserId(int id);

        public void EditThread(DiscussionThreadDTO threadDto);
    }

