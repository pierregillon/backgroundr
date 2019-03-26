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
                var cancellationTokenSource = new CancellationTokenSource();
                var task = Task.Run(async () => {
                    await Task.Delay(when - _clock.Now(), cancellationTokenSource.Token);
                    await _commandDispatcher.Dispatch(command);
                    var schedule = _schedules.FirstOrDefault(x => x.Command == (ICommand) command);
                    if (schedule != null) {
                        _schedules.Remove(schedule);
                    }
                }, cancellationTokenSource.Token);

                _schedules.Add(new CommandDispatchSchedule {
                    Command = command,
                    Task = task,
                    CancellationToken = cancellationTokenSource
                });
            }
        }

        public async Task CancelAll()
        {
            foreach (var schedule in _schedules) {
                schedule.CancellationToken.Cancel();
            }
            try {
                await Task.WhenAll(_schedules.Select(x => x.Task));
            }
            catch (TaskCanceledException) { }
        }

        private class CommandDispatchSchedule
        {
            public ICommand Command { get; set; }
            public Task Task { get; set; }
            public CancellationTokenSource CancellationToken { get; set; }
        }
    }
}