namespace TT.Domain.ViewModels
{
    public class MovementModel
    {
        public MovementModel(Location location, bool isMobile)
        {
            Location = location;
            IsMobile = isMobile;
        }

        public Location Location { get; }
        public bool IsMobile { get; }
    }
}