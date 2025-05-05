namespace Cloth.Events
{
    public class EventManager
    {
        private PeriodicEvents _periodicEvents { get; }

        public EventManager()
        {
            _periodicEvents = new PeriodicEvents();
        }
    }
}
