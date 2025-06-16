using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Role_Based_Invitation.Data;
using Role_Based_Invitation.Models;
using Role_Based_Invitation.Services;
using Role_Based_Invite.Services;

namespace Role_Based_Invitation.Controllers
{
    public class InviteController : Controller
    {
        private readonly AppDbContext _db;
        private readonly AuthService _auth;
        private readonly EmailService _email;
        public InviteController(AppDbContext db, AuthService auth, EmailService email)
        {
            _db = db;
            _auth = auth;
            _email = email;
        }

        public IActionResult Invite() => View();
        [HttpPost]
        public async Task<IActionResult> InviteUser(string receiverEmail)
        {
            int inviterId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var invite = new Invite
            {
                InviterId = inviterId,
                ReceiverEmail = receiverEmail,
                Status = "Pending"
            };

            _db.Invites.Add(invite);
            _db.SaveChanges();

            var inviteLink = Url.Action("RespondToInvite", "Invite", new { id = invite.Id }, Request.Scheme);

            string emailBody = $@"
        <h3>You’ve been invited to join Pitbox!</h3>
        <p>Click below to accept and create your account:</p>
        <p><a href='{inviteLink}'>Accept Invite</a></p>
        <p>This link is valid for one-time registration. Do not share it.</p>
    ";

            await _email.SendEmailAsync(receiverEmail, "You've Been Invited!", emailBody);

            return View("InviteConfirmation");
        }


        [HttpGet]
        public IActionResult AcceptInvite(int id)
        {
            var invite = _db.Invites.Include(i => i.Inviter).FirstOrDefault(i => i.Id == id);
            if (invite == null || invite.Status != "Pending") return NotFound();

            var user = new User
            {
                Email = invite.ReceiverEmail,
                PasswordHash = invite.GeneratedPassword,
                Role = "User",
                OrganizationName = invite.Inviter.OrganizationName
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            invite.ReceiverId = user.Id;
            invite.Status = "Accepted";
            _db.SaveChanges();

            return Content("Invite Accepted! You're now part of the organization.");
        }

        [HttpGet]
        public IActionResult DeclineInvite(int id)
        {
            var invite = _db.Invites.FirstOrDefault(i => i.Id == id);
            if (invite == null) return NotFound();

            invite.Status = "Declined";
            _db.SaveChanges();

            return Content("Invite Declined.");
        }

        [HttpGet]
        public IActionResult RespondToInvite(int id)
        {
            var invite = _db.Invites.Include(i => i.Inviter).FirstOrDefault(i => i.Id == id);
            if (invite == null || invite.Status != "Pending") return NotFound();
            return View(invite);
        }

        [HttpPost]
        public IActionResult HandleInviteResponse(int inviteId, string response, string password)
        {
            var invite = _db.Invites.FirstOrDefault(i => i.Id == inviteId);
            if (invite == null) return NotFound();

            if (response == "Decline")
            {
                invite.Status = "Declined";
                _db.SaveChanges();
                return View("InviteDeclined");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Password is required.");
                return View("RespondToInvite", invite);
            }

            // Check if user already exists
            if (_db.Users.Any(u => u.Email == invite.ReceiverEmail))
            {
                ModelState.AddModelError("", "A user with this email already exists.");
                return View("RespondToInvite", invite);
            }

            var inviter = _db.Users.FirstOrDefault(u => u.Id == invite.InviterId);

            var user = new User
            {
                Email = invite.ReceiverEmail,
                PasswordHash = _auth.HashPassword(password),
                Username = invite.ReceiverEmail.Split('@')[0], // default username
                Role = "User",
                OrganizationName = inviter.OrganizationName
            };

            _db.Users.Add(user);
            invite.Status = "Accepted";
            _db.SaveChanges();

            return RedirectToAction("Login", "Account");
        }

        public IActionResult ManageUsers()
        {
            int? inviterId = HttpContext.Session.GetInt32("UserId");
            if (inviterId == null) return RedirectToAction("Login", "Account");

            var inviter = _db.Users.Find(inviterId);
            if (inviter == null || inviter.Role != "Admin") return Unauthorized();

            var users = _db.Users
                .Where(u => u.OrganizationName == inviter.OrganizationName && u.Role != "Admin")
                .ToList();

            ViewBag.ReceiverEmail = TempData["ReceiverEmail"] ?? "";

            return View(users);
        }
        public IActionResult Welcome()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login", "Account");

            ViewBag.Organization = user.OrganizationName;
            return View("Welcome");
        }
       
        [HttpGet]
        public IActionResult CreateExam()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateExam(string title, string description, DateTime date)
        {
            int adminId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var exam = new PractExam
            {
                Title = title,
                Description = description,
                Date = date,
                CreatedByAdminId = adminId
            };

            _db.PracticalExaminations.Add(exam);
            _db.SaveChanges();

            return RedirectToAction("ManagerUsers");
        }
 
        public IActionResult UserExams()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var invite = _db.Invites.FirstOrDefault(i => i.ReceiverId == userId && i.Status == "Accepted");
            if (invite == null) return Unauthorized();

            var exams = _db.PracticalExaminations
                .Where(e => e.CreatedByAdminId == invite.InviterId)
                .ToList();

            return View(exams);
        }


    }

}
