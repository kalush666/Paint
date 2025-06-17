namespace Client.Commands
{
    public abstract class DrawingCommand
    {
        public abstract void Execute();
        public virtual bool CanExecute() => true;
    }
}