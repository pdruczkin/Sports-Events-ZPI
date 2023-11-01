using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Friends.Queries.SearchUsers;

public class SearchUsersQueryValidator : AbstractValidator<SearchUsersQuery>
{
	public SearchUsersQueryValidator()
	{
		RuleFor(x => x.SearchPhrase)
			.NotEmpty();
	}
}
