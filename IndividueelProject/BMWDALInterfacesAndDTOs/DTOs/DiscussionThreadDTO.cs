namespace BMWDALInterfacesAndDTOs.DTOs;
    public class DiscussionThreadDTO
    {
       public int ThreadId { get; set; }
       public string? Title { get; set; }
       public string? Text { get; set; }

        public int OwnerId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public int TopicId { get; set; }

        public bool IsEdited { get; set; }






    }

