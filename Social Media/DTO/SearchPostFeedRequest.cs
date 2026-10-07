using Social_Media.Enums;

namespace Social_Media.DTO
{
    public class SearchPostFeedRequest
    {

        // Pagnation
        private int _pageNumber = 1;
        public int PageNumber
        {
            // To avoid zero
            get => _pageNumber; set => _pageNumber = value < 1 ? 1 : value;
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            // Cap at 50, and avoid zero
            set => _pageSize = value > 50 ? 50 : (value < 1 ? 1 : value); 
        }
    }

    public class SearchPostsAdminDTO
    {

        // Pagnation
        private int _pageNumber = 1;
        public int PageNumber
        {
            // To avoid zero
            get => _pageNumber; set => _pageNumber = value < 1 ? 1 : value;
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            // Cap at 50, and avoid zero
            set => _pageSize = value > 50 ? 50 : (value < 1 ? 1 : value);
        }
        public string? SearchKeyword { get; set; }

        public List<int>? AuthorIds { get; set; } = null;
        public List<PostVisibility>? PostVisibilityTypes { get; set; } = null;

    }
}
