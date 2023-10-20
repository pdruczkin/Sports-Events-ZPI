using System.Data;
using FluentValidation;

namespace Application.Friends.Commands.BlockUser;

public class BlockUserCommandValidator : AbstractValidator<BlockUserCommand>
{
    public BlockUserCommandValidator()
    {
        RuleFor(x => x.UserToBlockId)
            .NotEmpty();
    }
}