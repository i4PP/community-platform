using BMWDomain.Entities;
namespace BMWDomain.interfaces;


public interface ITopicContainer
{
    public List<Topic> GetAllTopic();

    public Topic GetTopicById(int id);

}