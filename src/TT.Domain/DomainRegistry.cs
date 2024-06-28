using System;
using AutoMapper;
using TT.Domain.Abstract;
using TT.Domain.Services;
using System.Collections.Generic;

namespace TT.Domain
{
    /// <summary>
    /// The DomainRegistry forms a top-level boundary for interacting with entities by 
    /// hosting our Root object and any other cross-domain objects as required in future.
    /// </summary>
    public static class DomainRegistry
    {
        [ThreadStatic]
        private static IRoot _root;

        [ThreadStatic]
        private static IDomainRepository _repository;

        [ThreadStatic]
        private static IAttackNotificationBroker _attackNotificationBroker;

        [ThreadStatic]
        private static IList<Guid> _sentNotifications;

        private static string connectionStringOrName;

        public static IRoot Root
        {
            get { return _root ?? (_root = new Root()); }
            set { _root = value; }
        }

        public static IDomainRepository Repository
        {
            get
            {
                return (_repository == null || _repository.Disposed) ?
                    (_repository = new DomainRepository(new DomainContext(connectionStringOrName ?? "StatsWebConnection"))) :
                    _repository;
            }
            set { _repository = value; }
        }

        public static IAttackNotificationBroker AttackNotificationBroker
        {
            get { return _attackNotificationBroker ?? (_attackNotificationBroker = new AttackNotificationBroker()); }
            set { _attackNotificationBroker = value; }
        }

        public static IList<Guid> SentNotifications
        {
            get { return _sentNotifications ?? (_sentNotifications = new List<Guid>()); }
        }

        public static IMapper Mapper => mapperFunc();

        private static Func<IMapper> mapperFunc;

        public static void SetMapperFunc(Func<IMapper> mapperFunc)
        {
            DomainRegistry.mapperFunc = mapperFunc;
        }

        public static void SetConectionStringOrName(string connectionString)
        {
            connectionStringOrName = connectionString;
        }
    }
}