public interface IEventHandler<in TEvent> where TEvent : IntegrationEvent
{
    Task Handle(TEvent @event);
}
