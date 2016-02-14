using Highway.Data;

namespace TT.Domain
{
    public class DomainRepository : Repository, IDomainRepository
    {
        private readonly IDataContext _context;

        public DomainRepository(IDataContext context) 
            : base(context)
        {
            _context = context;
        }

        public T Execute<T>(IDomainCommand<T> command)
        {
            return command.Execute(_context);
        }
    }
}