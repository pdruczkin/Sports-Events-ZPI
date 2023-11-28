using FluentValidation;

namespace Application.Posts.Query.GetAllPosts;

public class GetAllPostQueryValidator : AbstractValidator<GetAllPostsQuery>
{
    private readonly int[] _allowedPageSizes = { 10, 30, 60, 120 };
    
    public GetAllPostQueryValidator()
    {
        RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize).Custom((value, context) =>
        {
            if (!_allowedPageSizes.Contains(value))
            {
                context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", _allowedPageSizes)}]");
            }
        });
    }
}