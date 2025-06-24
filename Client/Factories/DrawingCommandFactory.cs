using System;
using System.Collections.Generic;
using System.Linq;
using Client.Commands;
using Client.Handlers;
using Client.Services;
using Common.Enums;

namespace Client.Factories
{
    public interface IDrawingCommandFactory
    {
        IDrawingCommand? Create(Enum key);
    }

    public class DrawingCommandFactory : IDrawingCommandFactory
    {
        private readonly List<IDrawingCommand> _commands = new();
        private readonly ICommandServiceProvider _serviceProvider;

        public DrawingCommandFactory(ICommandServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            AutoRegisterCommands();
        }

        private void AutoRegisterCommands()
        {
            var commandTypes = typeof(IDrawingCommand).Assembly
                .GetTypes()
                .Where(t => typeof(IDrawingCommand).IsAssignableFrom(t) &&
                            !t.IsInterface &&
                            !t.IsAbstract &&
                            t != typeof(ShapeSelectionCommand))
                .ToList();

            foreach (var constructors in commandTypes.Select(type => type.GetConstructors()
                         .OrderByDescending(c => c.GetParameters().Length)))
            {
                foreach (var constructor in constructors)
                {
                    var parameters = constructor.GetParameters();
                    var args = new object[parameters.Length];
                    bool canCreate = true;

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        try
                        {
                            var service = _serviceProvider.GetService(parameters[i].ParameterType);
                            args[i] = service;
                        }
                        catch
                        {
                            canCreate = false;
                            break;
                        }
                    }

                    if (canCreate)
                    {
                        var command = (IDrawingCommand)constructor.Invoke(args);
                        _commands.Add(command);
                        break;
                    }
                }
            }

            RegisterShapeCommands();
        }

        private void RegisterShapeCommands()
        {
            var handler = _serviceProvider.GetService<DrawingHandler>();

            _commands.Add(new ShapeSelectionCommand(handler, BasicShapeType.Line));
            _commands.Add(new ShapeSelectionCommand(handler, BasicShapeType.Rectangle));
            _commands.Add(new ShapeSelectionCommand(handler, BasicShapeType.Circle));

        }

        public IDrawingCommand? Create(Enum commandEnum)
            => _commands.FirstOrDefault(c => c.CanExecute(commandEnum));
    }
}