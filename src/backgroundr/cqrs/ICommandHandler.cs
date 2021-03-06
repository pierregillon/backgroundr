﻿using System.Threading.Tasks;

namespace backgroundr.cqrs
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        Task Handle(T command);
    }

    public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand
    {
        Task<TResult> Handle(TCommand command);
    }
}