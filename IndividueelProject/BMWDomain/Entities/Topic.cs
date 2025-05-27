using BMWDALInterfacesAndDTOs.DTOs;
namespace BMWDomain.Entities;

public class Topic
{
    public int TopicId { get; set; }
    public string Name { get; set; }


    public Topic(int topicId, string name)
    {
        TopicId = topicId;
        Name = name;
    }
    
    public Topic(TopicDTO dto)
    {
        TopicId = dto.TopicId;
        Name = dto.Name ?? throw new ArgumentNullException(nameof(dto));
    }

    public TopicDTO ToDTO()
    {
        return new TopicDTO()
        {
            TopicId = TopicId,
            Name = Name
        };
    }



}