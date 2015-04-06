using System;
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

        /// <summary>
        /// Lists the top N users that post in your timeline
        /// </summary>
        /// <param name="numUsers">Top X users to return</param>
        public async Task<List<TwitterUserViewModel>> ListTopUsers(int numUsers)
        {
            var ctx = new TwitterContext(_authoriser);

            var tweets = await
                         (from tweet in ctx.Status
                          where tweet.Type == StatusType.Home
                                && tweet.Count == 800 // This is the max we can query
                          select tweet).ToListAsync();

            var users = (from t in tweets
                         group t by t.User.UserIDResponse
                         into g
                         orderby g.Count() descending
                         select new {UserID = g.Key, Count = g.Count()}).Take(numUsers)
                                                                        .ToDictionary(k => k.UserID, v => v.Count);

            // Now query the user information. We could perhaps do this as part of the tweet query above,
            // using the IncludeUserEntities parameter - however, this would be returning duplicate information,
            // so I'll do another query after the group by. We'd also need a local lookup if we did it that way.
            // Perhaps it's worth reviewing performance / API limits at a later date if this app gets used heavily.

            var userIdList = String.Join(",", from u in users
                                              select u.Key);

            var results = await
                          (from u in ctx.User
                           where u.Type == UserType.Lookup
                                 && u.UserIdList == userIdList
                           select new TwitterUserViewModel
                                  {
                                      UserID = u.UserIDResponse,
                                      ImageUrl = u.ProfileImageUrl,
                                      ScreenName = u.ScreenNameResponse
                                  }).ToListAsync();

            // Update the impact (ie. group by count)
            results.ForEach(x => x.Impact = users[x.UserID]);

            return results;
        }
    }
}
