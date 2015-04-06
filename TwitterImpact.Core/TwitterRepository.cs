using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace TwitterImpact.Core
{
    public class TwitterRepository
    {
        private readonly IAuthorizer _authoriser;

        public TwitterRepository(IAuthorizer authoriser)
        {
            _authoriser = authoriser;
        }

        public async Task<List<TweetViewModel>> ListTweets()
        {
            var ctx = new TwitterContext(_authoriser);

            return await
                   (from tweet in ctx.Status
                    where tweet.Type == StatusType.Home
                    select new TweetViewModel
                           {
                               ImageUrl = tweet.User.ProfileImageUrl,
                               ScreenName = tweet.User.ScreenNameResponse,
                               Text = tweet.Text
                           })
                       .ToListAsync();
        }
    }
}
