using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using FluentValidation;
using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMW.ASP.Models;
using BMWDALInterfacesAndDTOs.Exceptions;
using BMWDomain.Exceptions;


namespace BMW.ASP.Controllers;

public class ClubController : Controller
{
    private readonly IClubContainer _clubContainer;
    private readonly IUserContainer _userContainer;
    private readonly IClubRolesContainer _clubRolesContainer;
    private readonly IInviteCodeContainer _inviteCodeContainer;
    private readonly IChatContainer _chatContainer;

    public ClubController(IClubContainer clubContainer, IUserContainer userContainer,
        IClubRolesContainer clubRolesContainer, IInviteCodeContainer inviteCodeContainer, IChatContainer chatContainer)
    {
        _clubContainer = clubContainer;
        _userContainer = userContainer;
        _clubRolesContainer = clubRolesContainer;
        _inviteCodeContainer = inviteCodeContainer;
        _chatContainer = chatContainer;
    }




    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Login");
        }

        try
        {
            List<ClubViewModel> clubs = new List<ClubViewModel>();
            var clubsRaw = _clubContainer.GetUserClub((int)HttpContext.Session.GetInt32("UserId")!);
            foreach (var club in clubsRaw)
            {
                clubs.Add(new ClubViewModel()
                {
                    ClubId = club.ClubId,
                    Name = club.Name,
                    Desc = club.Desc,
                    Land = club.Land,
                    CreatedAt = club.CreatedAt
                });
            }

            return View(clubs);
        }
        catch (BllException e)
        {
            TempData["Error"] = e.Message;
        }
        catch (DataBaseException e)
        {
            TempData["Error"] = e.Message;
        }
        catch (Exception)
        {
            TempData["Error"] = "An error occurred while fetching the clubs.";
        }




        return View();
    }

    public IActionResult Create()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Login");
        }

        return View();
    }

    [HttpPost]
    public IActionResult Create(CreateClubModelView model)
    {
        if (ModelState.IsValid)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Login");
            }

            try
            {
                model.OwnerId = (int)HttpContext.Session.GetInt32("UserId")!;
                CreateClub club = new CreateClub(0, model.Name!, model.Desc!, model.Land!, model.OwnerId);

                _clubContainer.CreateClub(club);

                return RedirectToAction("Index", "Club");

            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
            catch (BllException e)
            {
                TempData["Error"] = e.Message;
            }
            catch (DataBaseException e)
            {
                TempData["Error"] = e.Message;
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while creating the club.";
            }

        }

        return View();
    }

    public IActionResult Detail(int id)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Login");
        }

        try
        {
            ClubDetailViewModel club = new ClubDetailViewModel();
            var clubRaw = _clubContainer.GetClubDetail(id);
            var roles = _clubRolesContainer.GetRoles();
            club.ClubId = clubRaw.ClubId;
            club.Name = clubRaw.Name;
            club.Desc = clubRaw.Desc;
            club.Land = clubRaw.Land;
            club.CreatedAt = clubRaw.CreatedAt;
            club.Members = new List<ClubMembershipViewModel>();
            club.Roles = new List<RolesDetailViewModel>();

            foreach (var role in roles)
            {
                if (role.RoleId == 1)
                {
                    continue;
                }   
                club.Roles.Add(new RolesDetailViewModel()
                {
                    RoleId = role.RoleId,
                    Name = role.RoleName
                });
            }

            foreach (var member in clubRaw.Members!)
            {
                var user = _userContainer.GetUserById(member.UserId);
                var role = _clubRolesContainer.GetRoleById(member.RoleId);
                club.Members.Add(new ClubMembershipViewModel()
                {
                    UserId = member.UserId,
                    Username = user.Name,
                    RoleId = member.RoleId,
                    RoleName = role.RoleName
                });
            }

            return View(club);
        }
        catch (BllException e)
        {
            TempData["Error"] = e.Message;
        }
        catch (DataBaseException e)
        {
            TempData["Error"] = e.Message;
        }
        catch (Exception )
        {
            TempData["Error"] = "An error occurred while fetching the club.";
        }

        return View();
    }

    public IActionResult CreateInvite(int clubId)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Login");
        }

        InviteViewModel model = new InviteViewModel
        {
            ClubId = clubId
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult CreateInvite(InviteViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Login");
            }

            try
            {
                DateTime expirationDate = new DateTime(model.ExpirationDate.Year, model.ExpirationDate.Month,
                    model.ExpirationDate.Day, model.ExpirationTime.Hour, model.ExpirationTime.Minute,
                    model.ExpirationTime.Second);


                InviteCode invite = new InviteCode(model.Code!, model.ClubId, expirationDate, model.MaxUses, 0,
                    (int)HttpContext.Session.GetInt32("UserId")!);
                _inviteCodeContainer.CreateInviteCode(invite);
                return RedirectToAction("ClubInvitesCodes", new { clubId = model.ClubId });
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
            catch (BllException e)
            {
                TempData["Error"] = e.Message;
            }
            catch (DataBaseException e)
            {
                TempData["Error"] = e.Message;
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while creating the invite code.";
            }

        }

        return View();
    }

    public IActionResult ClubInvitesCodes(int clubId)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Login");
        }

        try
        {
            List<InviteCode> inviteCodes = _inviteCodeContainer.GetInviteCodesByClubId(clubId);
            List<InviteWithUserViewModel> invites = new List<InviteWithUserViewModel>();

            foreach (var inviteCode in inviteCodes)
            {
                if (inviteCode.ExpirationDate < DateTime.Now || inviteCode.MaxUses == 0)
                {
                    continue;
                }

                var user = _userContainer.GetUserById(inviteCode.UserId);
                invites.Add(new InviteWithUserViewModel()
                {
                    Id = inviteCode.Id,
                    Code = inviteCode.Code,
                    ClubId = inviteCode.ClubId,
                    MaxUses = inviteCode.MaxUses,
                    ExpirationDate = inviteCode.ExpirationDate!.Value,
                    UserName = user.Name,
                    UserId = inviteCode.UserId
                });
            }

            return View(invites);
        }
        catch (BllException e)
        {
            TempData["Error"] = e.Message;
        }
        catch (DataBaseException e)
        {
            TempData["Error"] = e.Message;
        }
        catch (Exception)
        {
            TempData["Error"] = "An error occurred while creating the invite code.";
        }

        return RedirectToAction("Index", "Club");
    }

    public IActionResult JoinClub()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Login");
        }

        return View();
    }

    [HttpPost]
    public IActionResult JoinClub(JoinClubViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Login");
            }

            try
            {
                var inviteCode = _inviteCodeContainer.GetInviteCodeById(model.InviteCode!);
                ClubMembership clubMembership =
                    new ClubMembership(inviteCode.ClubId, (int)HttpContext.Session.GetInt32("UserId")!, 3);
                _clubContainer.RegisterUserToClub(clubMembership);
                return RedirectToAction("Index", "Club");
            }
            catch (InviteCodeException)
            {
                TempData["Error"] = "The invite code is invalid.";
            }
            catch (BllException e)
            {
                TempData["Error"] = e.Message;
            }
            catch (DataBaseException e)
            {
                TempData["Error"] = e.Message;
            }
            catch (DuplicateEntryException e)
            {
                TempData["Error"] = e.Message;
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while joining a club .";
            }

        }

        return View();

    }

    public IActionResult Chat(int clubId)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Login");
        }

        try
        {
            var club = _clubContainer.GetClubDetail(clubId);
            bool isMember = club.Members!.Exists(membership =>
                membership.UserId == (int)HttpContext.Session.GetInt32("UserId")!);
            if (isMember)
            {
                var messages = _chatContainer.GetClubMessages(clubId);
                List<MessageViewModel> messageViewModels = new List<MessageViewModel>();
                foreach (var message in messages)
                {
                    var user = _userContainer.GetUserById(message.UserId);
                    messageViewModels.Add(new MessageViewModel()
                    {
                        Username = user.Name,
                        Message = message.Content
                    });

                }

                ClubMessages clubMessages = new ClubMessages()
                {
                    ClubId = clubId,
                    Messages = messageViewModels
                };
                return View(clubMessages);
            }
            else
            {
                TempData["Error"] = "You are not a member of this club.";
            }

        }
        catch (BllException e)
        {
            TempData["Error"] = e.Message;
        }
        catch (DataBaseException e)
        {
            TempData["Error"] = e.Message;
        }
        catch (Exception)
        {
            TempData["Error"] = "An error occurred while fetching the chat.";
        }

        return RedirectToAction("Index", "Club");
    }


    [HttpPost]
    public IActionResult ChangeRole(int clubId, int userId, int roleId)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Login");
        }

        try
        {
            ClubMembership clubMembership = new ClubMembership(clubId, userId, roleId);
            _clubContainer.UpdateMembershipRole(clubMembership);
            return RedirectToAction("Detail", new { id = clubId });
        }
        catch (BllException e)
        {
            TempData["Error"] = e.Message;
        }
        catch (DataBaseException e)
        {
            TempData["Error"] = e.Message;
        }
        catch (Exception)
        {
            TempData["Error"] = "An error occurred while changing the role.";
        }

        return RedirectToAction("Detail", new { id = clubId });
    }
}
