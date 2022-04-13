namespace GameAccessService_API.Helpers
{
    public class HasAccess
    {
        public HasAccess(bool hasAccess)
        {
            this.hasAccess = hasAccess;
        }

        public bool hasAccess { get; init; }
    }
}