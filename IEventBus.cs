public interface IEventBus
{
    void Publish(IntegrationEvent @event);
    void Subscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IEventHandler<TEvent>;
    void Unsubscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IEventHandler<TEvent>;
}
