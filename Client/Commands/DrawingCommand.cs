namespace Client.Commands
{
    public abstract class DrawingCommand
    {
        public abstract string Key { get; }
        public abstract void Execute();
        public virtual bool CanExecute() => true;
    }
}