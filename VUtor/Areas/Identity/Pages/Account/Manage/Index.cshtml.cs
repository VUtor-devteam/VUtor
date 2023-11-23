
#nullable disable

using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace VUtor.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ProfileEntity> _userManager;
        private readonly SignInManager<ProfileEntity> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<ProfileEntity> userManager,
            SignInManager<ProfileEntity> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }
        public List<TopicEntity> TopicList { get; set; } = new List<TopicEntity>();



        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [AllowNull]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [AllowNull]
            [DataType(DataType.Text)]
            [Display(Name = "Surname")]
            public string Surname { get; set; }

            [AllowNull]
            [Display(Name = "Course Name")]
            public CourseName CourseName { get; set; }

            [AllowNull]
            [Display(Name = "Course Year")]
            public CourseYear CourseYear { get; set; }

            [Display(Name = "Topic To Learn")]
            public List<int> TopicsToLearn { get; set; } = new List<int>();

            [Display(Name = "Topic To Teach")]
            public List<int> TopicsToTeach { get; set; } = new List<int>();

        }

        private async Task LoadAsync(ProfileEntity user)
        {
            TopicList = await _context.Topics.ToListAsync();
            Username = await _userManager.GetUserNameAsync(user);
            var profile = _context.Profiles.Where(p => p.Id == user.Id).Include(p => p.TopicsToLearn).Include(p => p.TopicsToTeach).First();
            Input = new InputModel
            {
                Name = user.Name,
                Surname = user.Surname,
                CourseName = (CourseName)user.CourseInfo.CourseName,
                CourseYear = (CourseYear)user.CourseInfo.CourseYear,
                TopicsToLearn = profile.TopicsToLearn.Select(e => e.Id).ToList(),
                TopicsToTeach = profile.TopicsToTeach.Select(e => e.Id).ToList()
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            TopicList = _context.Topics.ToList();
            var user = await _userManager.GetUserAsync(User);
            var profile = _context.Profiles.Where(p => p.Id == user.Id).Include(p => p.TopicsToLearn).Include(p => p.TopicsToTeach).First();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
            }

            if (Input.Surname != user.Surname)
            {
                user.Surname = Input.Surname;
            }

            var newCourseInfo = new CourseData((int)Input.CourseName, (int)Input.CourseYear);
            if (!user.CourseInfo.Equals(newCourseInfo))
            {
                user.CourseInfo = newCourseInfo;
            }

            // Retrieve the new topics the user has chosen
            var newTopicsToLearn = _context.Topics.Where(t => Input.TopicsToLearn.Contains(t.Id)).ToList();
            var newTopicsToTeach = _context.Topics.Where(t => Input.TopicsToTeach.Contains(t.Id)).ToList();

            // Update the user's topics to learn
            foreach (var topic in user.TopicsToLearn.ToList())
            {
                if (!newTopicsToLearn.Contains(topic))
                {
                    // Remove the user from the topic's learning profiles and remove the topic from the user's topics
                    topic.LearningProfiles.Remove(user);
                    user.TopicsToLearn.Remove(topic);
                }
            }
            foreach (var topic in newTopicsToLearn)
            {
                if (!user.TopicsToLearn.Contains(topic))
                {
                    // Add the user to the topic's learning profiles and add the topic to the user's topics
                    topic.LearningProfiles.Add(user);
                    user.TopicsToLearn.Add(topic);
                }
            }

            // Update the user's topics to teach
            foreach (var topic in user.TopicsToTeach.ToList())
            {
                if (!newTopicsToTeach.Contains(topic))
                {
                    // Remove the user from the topic's teaching profiles and remove the topic from the user's topics
                    topic.TeachingProfiles.Remove(user);
                    user.TopicsToTeach.Remove(topic);
                }
            }
            foreach (var topic in newTopicsToTeach)
            {
                if (!user.TopicsToTeach.Contains(topic))
                {
                    // Add the user to the topic's teaching profiles and add the topic to the user's topics
                    topic.TeachingProfiles.Add(user);
                    user.TopicsToTeach.Add(topic);
                }
            }           

            await _context.SaveChangesAsync();
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}