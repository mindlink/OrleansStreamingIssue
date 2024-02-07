namespace OrleansStreamingIssue
{
    using Orleans;

    internal interface IGrainB : IGrainWithGuidKey
    {
        Task Subscribe(Guid id);
    }
}
