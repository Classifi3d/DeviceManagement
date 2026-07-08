using Application.Abstraction;
using Application.Abstraction.CQRS;
using Application.DTOs;
using CSharpFunctionalExtensions;
using Domain.Entities.User;

namespace Application.CommandsAndQueries.Users;

public sealed record CreateUserCommand(UserRequestDTO UserDto) : ICommand;

public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserDto is null)
        {
            return Result.Failure("Creating user failed");

        }

        var user = new User();
        user.Name = request.UserDto.Name;
        user.Password = request.UserDto.Password;
        user.Id = Guid.NewGuid();

        await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
