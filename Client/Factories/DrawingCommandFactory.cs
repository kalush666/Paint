using System;
using System.Collections.Generic;
using System.Linq;
using Client.Commands;

namespace Client.Factories
{
    public class DrawingCommandFactory
    {
        private readonly List<DrawingCommand> _commands;

        public DrawingCommandFactory()
        {
            _commands = typeof(DrawingCommand).Assembly
                .GetTypes()
                .Where(t => typeof(DrawingCommand).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => (DrawingCommand)Activator.CreateInstance(t)!)
                .ToList();
        }

        public DrawingCommand? Get(string key) =>
            _commands.FirstOrDefault(c => c.Key == key);
    }
}