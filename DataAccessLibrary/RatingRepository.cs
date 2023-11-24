using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary
{
    public class RatingRepository
    {
        private readonly ApplicationDbContext _context;

        public RatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdateRatingAsync(Rating rating)
        {
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.RecipientId == rating.RecipientId && r.ReviewerId == rating.ReviewerId);

            if (existingRating != null)
            {
                existingRating.Score = rating.Score;
            }
            else
            {
                _context.Ratings.Add(rating);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<double> GetAverageRatingAsync(string recipientId)
        {
            var ratings = await _context.Ratings
                                        .Where(r => r.RecipientId == recipientId)
                                        .ToListAsync();

            if (!ratings.Any())
            {
                return -1; // Or handle as needed if there are no ratings
            }

            return ratings.Average(r => r.Score);
        }

        public async Task<Rating> GetRatingByReviewerAsync(string recipientId, string reviewerId)
        {
            return await _context.Ratings
                                 .FirstOrDefaultAsync(r => r.RecipientId == recipientId && r.ReviewerId == reviewerId);
        }
    }
}
