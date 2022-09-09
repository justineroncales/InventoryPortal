namespace InvoiceManagement.Data
{
    public interface IRabitMQProducer
    {
        void SendProductMessage<T>(T message);
    }
}