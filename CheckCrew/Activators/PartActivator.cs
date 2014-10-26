namespace CheckCrew.Activators
{
    public abstract class PartActivator<T> where T: PartModule
    {
        protected T _module;

        protected PartActivator(T item)
        {
            _module = item;
        }

        public abstract void Activate();
        public abstract void Deactivate();
    }
}
