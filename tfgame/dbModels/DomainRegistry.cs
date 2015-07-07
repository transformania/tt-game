namespace tfgame.dbModels
{
    /// <summary>
    /// The DomainRegistry forms a top-level boundary for interacting with entities by 
    /// hosting our Root object and any other cross-domain objects as required in future.
    /// </summary>
    public static class DomainRegistry
    {
        public static Root Root { get; set; }
    }
}