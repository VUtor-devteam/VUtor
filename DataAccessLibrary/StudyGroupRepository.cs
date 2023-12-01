using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<StudyGroup> GetStudyGroupByIdAsync(int studyGroupId)
        {
            return await _context.StudyGroups.FindAsync(studyGroupId);
        }

        public async Task UpdateStudyGroupAsync(StudyGroup studyGroup)
        {
            _context.StudyGroups.Update(studyGroup);
            await _context.SaveChangesAsync();
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

        public async Task RemoveMemberAsync(int studyGroupId, string userId)
        {
            var member = await _context.StudyGroupMembers
                                       .FirstOrDefaultAsync(m => m.StudyGroupId == studyGroupId && m.ParticipantId == userId);
            if (member != null)
            {
                _context.StudyGroupMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
        }

        // Method to get study groups with membership status
        public async Task<List<(StudyGroup studyGroup, bool isMember)>> GetStudyGroupsWithMembershipStatus(string userId)
        {
            return await _context.StudyGroups
                .Select(group => new
                {
                    StudyGroup = group,
                    IsMember = _context.StudyGroupMembers.Any(member => member.StudyGroupId == group.Id && member.ParticipantId == userId)
                })
                .ToListAsync()
                .ContinueWith(task => task.Result.Select(g => (g.StudyGroup, g.IsMember)).ToList());
        }
    }
}
