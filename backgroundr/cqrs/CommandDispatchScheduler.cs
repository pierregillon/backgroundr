using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backgroundr.application;
using backgroundr.domain;

namespace backgroundr.cqrs
{
    public class CommandDispatchScheduler
    {
        private readonly IClock _clock;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IList<CommandDispatchSchedule> _schedules = new List<CommandDispatchSchedule>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public CommandDispatchScheduler(IClock clock, ICommandDispatcher commandDispatcher)
        {
            _clock = clock;
            _commandDispatcher = commandDispatcher;
        }

        public async Task Schedule<T>(T command, DateTime when) where T : ICommand
        {
            if (_clock.Now() >= when) {
                await _commandDispatcher.Dispatch(command);
            }
            else {
                var task = Task.Run(async () => {
                    await Task.Delay(when - _clock.Now(), _cancellationTokenSource.Token);
                    await _commandDispatcher.Dispatch(command);
                    var schedule = _schedules.FirstOrDefault(x => x.Command == (ICommand) command);
                    if (schedule != null) {
                        _schedules.Remove(schedule);
                    }
                }, _cancellationTokenSource.Token);

                _schedules.Add(new CommandDispatchSchedule {
                    Command = command,
                    Task = task
                });
            }
        }

        public async Task Clear()
        {
            _cancellationTokenSource.Cancel();
            try {
                await Task.WhenAll(_schedules.Select(x => x.Task));
            }
            catch (TaskCanceledException) { }
        }

        private class CommandDispatchSchedule
        {
            public ICommand Command { get; set; }
            public Task Task { get; set; }
        }
    }
}