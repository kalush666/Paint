using System;
using Client.Enums;

namespace Client.Commands
{
    public interface  IDrawingCommand
    {
        Enum Key { get; }
        void Execute();

        bool CanExecute(Enum key)
        {
            return key.Equals(Key);
        }
    }
}