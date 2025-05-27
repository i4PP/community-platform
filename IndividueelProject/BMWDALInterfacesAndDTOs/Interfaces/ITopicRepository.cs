using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDALInterfacesAndDTOs.Interfaces;

public interface ITopicRepository
{
    public List<TopicDTO> GetAllTopic();

    public TopicDTO GetTopicById(int id);
    
}