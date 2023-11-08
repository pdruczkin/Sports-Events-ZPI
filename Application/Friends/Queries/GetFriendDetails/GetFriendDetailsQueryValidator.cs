using FluentValidation;

namespace Application.Friends.Queries.GetFriendDetails;

public class GetFriendDetailsQueryValidator : AbstractValidator<GetFriendDetailsQuery>
{
    public GetFriendDetailsQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}