namespace OrleansStreamingIssue
{
    using Orleans;
    using Orleans.Streams;

    internal class GrainB : Grain, IGrainB
    {
        public async Task Subscribe(Guid id)
        {
            var streamProvider = this.GetStreamProvider("SimpleMessaging");

            var otherProducerStream = streamProvider.GetStream<string>(id, GrainA.Namespace);

            await otherProducerStream.SubscribeAsync((s, _) =>
            {
                Console.WriteLine("Grain B received message: " + s);

                return Task.CompletedTask;
            });
        }
    }
}
