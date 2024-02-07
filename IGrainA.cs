namespace OrleansStreamingIssue
{
    using Orleans;
    using Orleans.Streams;

    internal interface IGrainA : IGrainWithGuidKey
    {
        Task SendMessage(string message);

        Task Subscribe(Guid id);
    }
}
