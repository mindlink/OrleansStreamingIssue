namespace OrleansStreamingIssue
{
    using Orleans;
    using Orleans.Streams;

    [ImplicitStreamSubscription(Namespace)] // Remove this and re-run to show expected flow.
    internal class GrainA : Grain, IGrainA
    {
        public const string Namespace = "ns";

        private IAsyncStream<string>? stream;

        public override async Task OnActivateAsync()
        {
            var streamProvider = this.GetStreamProvider("SimpleMessaging");

            this.stream = streamProvider.GetStream<string>(this.GetPrimaryKey(), Namespace);

            await this.stream.SubscribeAsync((s, _) =>
            {
                Console.WriteLine("Grain A received its own message: " + s);

                return Task.CompletedTask;
            });
        }

        public async Task SendMessage(string message)
        {
            await stream!.OnNextAsync(message);
        }

        public async Task Subscribe(Guid id)
        {
            var streamProvider = this.GetStreamProvider("SimpleMessaging");

            var otherGrainAStream = streamProvider.GetStream<string>(id, Namespace);

            await otherGrainAStream.SubscribeAsync((s, _) =>
            {
                Console.WriteLine("Other Grain A received message: " + s);

                return Task.CompletedTask;
            });
        }
    }
}
