using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary
{
    public class StudyGroupRepository
    {

        private readonly ApplicationDbContext _context;

        public StudyGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudyGroup>> GetAllStudyGroupsAsync()
        {
            return await _context.StudyGroups.ToListAsync();
        }

        public int GetMemberCount(int studyGroupId)
        {
            return _context.StudyGroupMembers
              .Count(member => member.StudyGroupId == studyGroupId);
        }

        public async Task AddStudyGroupAsync(StudyGroup group)
        {
            _context.StudyGroups.Add(group);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsMember(int studyGroupId, string userId)
        {
            return await _context.StudyGroupMembers
                                 .AnyAsync(m => m.StudyGroupId == studyGroupId && m.ParticipantId == userId);
        }

        public async Task AddMemberAsync(int studyGroupId, string userId)
        {
            var member = new StudyGroupMember { StudyGroupId = studyGroupId, ParticipantId = userId };
            _context.StudyGroupMembers.Add(member);
            await _context.SaveChangesAsync();
        }

    }
}
