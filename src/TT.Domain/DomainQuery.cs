﻿using System.Collections.Generic;
using System.Linq;
using Highway.Data;

namespace TT.Domain
{
    public abstract class DomainQuery<T> : Highway.Data.Query<T>, IDomainQuery<T>
    {
        public abstract IEnumerable<T> Execute(IDataContext context);

        protected virtual void Validate() { }

        protected IEnumerable<T> ExecuteInternal(IDataContext context)
        {
            Validate();
            return base.Execute(context).ToArray();
        }
    }
}