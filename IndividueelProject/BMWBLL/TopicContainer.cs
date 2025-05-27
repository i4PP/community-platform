using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDomain.Exceptions;
using BMWDALInterfacesAndDTOs.Exceptions;


namespace BMW.BLL
{
    public class TopicContainer : ITopicContainer
    {
        private readonly ITopicRepository _topicRepository;

        public TopicContainer(ITopicRepository topicRepository)
        {
            this._topicRepository = topicRepository;
        }

        public List<Topic> GetAllTopic()
        {
            try
            {
                var result = _topicRepository.GetAllTopic();
                List<Topic> topics = new List<Topic>();
                foreach (var topic in result)
                {
                    Topic newTopic = new Topic(topic);
                    topics.Add(newTopic);
                }

                return topics;
            }
            catch (NetworkException e)
            {
                throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
            }
            catch (DalException e)
            {
                throw new DataBaseException("error while getting club details 501", e);
            }


        }

        public Topic GetTopicById(int id)
        {


            try
            {

                var result = _topicRepository.GetTopicById(id);

                Topic topic = new Topic(result);


                return topic;
            }
            catch (NetworkException e)
            {
                throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
            }
            catch (DalException e)
            {
                throw new DataBaseException("error while getting topics details 501", e);
            }



        }

    }
}
