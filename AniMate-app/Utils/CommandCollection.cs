﻿namespace AniMate_app.Utils
{
    public class Command<T>
    {
        private T Paremeter { get; }

        private readonly Action<T> _action;

        private readonly Func<T, bool> _checkFunc;

        public bool CanExecute() => _checkFunc.Invoke(Paremeter);

        public void Execute() => _action.Invoke(Paremeter);

        public Command(T paremeter, Action<T> action, Func<T, bool> checkFunc)
        {
            Paremeter = paremeter;
            _action = action;
            _checkFunc = checkFunc;
        }
    }

    class CommandCollection<T>
    {
        private Queue<Command<T>> _commands = new();

        private Task _task;

        private CancellationTokenSource _cts = null;

        private void Start()
        {
            _task = Task.Factory.StartNew(() =>
            {
                while(_commands.Count > 0)
                {
                    var command = _commands.Dequeue();

                    while (!command.CanExecute()) 
                    {
                        if (_cts.Token.IsCancellationRequested)
                        {
                            return;
                        }
                    }

                    if (_cts.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    command.Execute();
                }
            });
        }

        public void Add(Command<T> command)
        {
            _commands.Enqueue(command);

            if(_task is null || _task.IsCompleted)
            {
                _cts = new CancellationTokenSource();

                Start();
            }       
        }

        public void Clear()
        {
            _commands.Clear();

            if(_task is not null)
                _cts?.Cancel();
        }
    }
}
