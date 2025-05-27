using FluentValidation;
using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDomain.Exceptions;
using BMWDALInterfacesAndDTOs.Exceptions;

namespace BMW.BLL
{
    public class DiscussionThreadContainer : IDiscussionThreadContainer
    {
        private readonly IDiscussionThreadRepository _discussionThreadRepository;
        private readonly IValidator<DiscussionThread> _discussionThreadValidator;

        public DiscussionThreadContainer(IDiscussionThreadRepository discussionThreadRepository, IValidator<DiscussionThread> discussionThreadValidator)
        {
            this._discussionThreadRepository = discussionThreadRepository;
            this._discussionThreadValidator = discussionThreadValidator;
        }



        public void CreateThread(DiscussionThread thread)
        {  
            _discussionThreadValidator.ValidateAndThrow(thread);
            try
            {
                DiscussionThreadDTO dto = thread.ToDTO();
                _discussionThreadRepository.CreateThread(dto);
                thread.ThreadId = dto.ThreadId;

            }
            catch (NetworkException e)
            {
                throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
            }
            catch (DalException e)
            {
                throw new DataBaseException("error while getting thread details 501", e);
            }
 

                
        }

        public List<DiscussionThread> GetAllThreads()
        {
            try
            {
                var result = _discussionThreadRepository.GetAllThreads();
                List<DiscussionThread> threads = new List<DiscussionThread>();
                foreach (var thread in result)
                {
                    threads.Add(new DiscussionThread(thread));
                }

                return threads;
            }
            catch (NetworkException e)
            {
                throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
            }
            catch (DalException e)
            {
                throw new DataBaseException("error while getting thread details 501", e);
            }

        }

        public DiscussionThread GetThreadById(int id)
        {
            try
            {
                var result = _discussionThreadRepository.GetThreadById(id);

                DiscussionThread thread = new DiscussionThread(result);
                return thread;
            }
            catch (NetworkException e)
            {
                throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
            }
            catch (DalException e)
            {
                throw new DataBaseException("error while getting thread details 501", e);
            }
            catch (Exception e)
            {
                throw new BllException("error while getting thread details 500", e);
            }

        }

        public void DeleteThreadById(int id)
        {
            try
            {
                _discussionThreadRepository.DeleteThreadById(id);

            }
            catch (NetworkException e)
            {
                throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
            }
            catch (DalException e)
            {
                throw new DataBaseException("error while getting thread details 501", e);
            }

        }

        public List<DiscussionThread> GetThreadsByUserId(int userId)
        {
            try
            {

                var result = _discussionThreadRepository.GetThreadsByUserId(userId);
                List<DiscussionThread> threads = new List<DiscussionThread>();
                foreach (var thread in result)
                {
                    threads.Add(new DiscussionThread(thread));
                }

                return threads;
            }
            catch (NetworkException e)
            {
                throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
            }
            catch (DalException e)
            {
                throw new DataBaseException("error while getting thread details 501", e);
            }

        }

        public void EditThread(DiscussionThread thread)
        {
            try
            {
                _discussionThreadValidator.ValidateAndThrow(thread);
                DiscussionThreadDTO dto = thread.ToDTO();
                _discussionThreadRepository.EditThread(dto);
                thread.IsEdited = dto.IsEdited;

            }
            catch (NetworkException e)
            {
                throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
            }
            catch (DalException e)
            {
                throw new DataBaseException("error while getting thread details 501", e);
            }

        }
    }
}