namespace TT.Domain
{
    /// <summary>
    /// Base class for executing updates to entities with no return type
    /// </summary>
    public abstract class Command
    {
        public void Execute()
        {
            DomainRegistry.Root.Execute(this);
        }

        internal abstract void InternalExecute();
    }

    /// <summary>
    /// Base class for executing updates to entities with a return type
    /// </summary>
    public abstract class Command<T>
    {
        public void Execute()
        {
            DomainRegistry.Root.Execute(this);
        }

        internal abstract T InternalExecute();
    }
}