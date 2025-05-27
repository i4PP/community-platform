using BMWDALInterfacesAndDTOs.DTOs;
namespace BMWDomain.Entities;

    public class DiscussionThread
    {
       public int ThreadId { get; set; }
       public string Title { get; set; }
       public string Text { get; set; }

       public int OwnerId { get; set; }

       public DateTimeOffset CreatedAt { get; set; }

       public int TopicId { get; set; }

       public bool IsEdited { get; set; }



        public DiscussionThread(int threadId,string title, string text, int ownerId, DateTimeOffset created, int topic, bool isEdited)
        {
            ThreadId = threadId;
              Title = title;
              Text = text;
              OwnerId = ownerId;
              CreatedAt = created;
              TopicId = topic;
              IsEdited = isEdited;

        }
        
        public DiscussionThread(DiscussionThreadDTO dto)
        {
            ThreadId = dto.ThreadId;
            Title = dto.Title ?? throw new ArgumentNullException(nameof(dto));
            Text = dto.Text ?? throw new ArgumentNullException(nameof(dto));
            OwnerId = dto.OwnerId;
            CreatedAt = dto.CreatedAt;
            TopicId = dto.TopicId;
            IsEdited = dto.IsEdited;

        }


        public DiscussionThreadDTO ToDTO()
        {
            return new DiscussionThreadDTO()
            {
                ThreadId = ThreadId,
                Title = Title,
                Text = Text,
                OwnerId = OwnerId,
                CreatedAt = CreatedAt,
                TopicId = TopicId,
                IsEdited = IsEdited
            };
        }




    }

