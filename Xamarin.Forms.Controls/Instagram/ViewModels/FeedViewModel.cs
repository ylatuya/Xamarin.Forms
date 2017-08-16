using System.Collections.Generic;

namespace Xamarin.Forms.Controls.Instagram.ViewModels
{
    public class FeedViewModel : BindableObject
    {
        public FeedViewModel()
        {
            InstagramFeed = new List<Models.FeedItem>()
            {
                new Models.FeedItem()
                {
                    ViewCount = 10517,
                    Comment = "xamarin With dozens of cities already announced, Xamarin Dev Days are active around the globe and we’re ready to announce even more cities all around the world!. #XamarinDevDay",
                    CommentCount = 88,
                    Image = "xamarindevdays.jpg",
                    Name = "xamarin",
                    PostedAt = "1 hour ago"
                },
                new Models.FeedItem()
                {
                    ViewCount = 11598,
                    Comment = "latenightseth Enjoy your well-deserved vacation, Mr President. #LNSM",
                    CommentCount = 45,
                    Image = "latenightsethfeedimage.jpg",
                    Name = "latenightseth",
                    PostedAt = "2 hour ago"
                }
            };
        }

        public List<Models.FeedItem> InstagramFeed { get; set; }
    }
}