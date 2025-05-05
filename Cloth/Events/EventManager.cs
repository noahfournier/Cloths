namespace Cloth.Events
{
    public class EventManager
    {
        private PeriodicEvents _periodicEvents;

        public EventManager()
        {
            _periodicEvents = new PeriodicEvents();
        }
    }
}
