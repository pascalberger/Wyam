﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wyam.Common;
using Wyam.Common.Configuration;
using Wyam.Common.Documents;
using Wyam.Common.Modules;
using Wyam.Common.Pipelines;

namespace Wyam.Core.Modules
{
    public class OrderBy : IModule
    {
        private readonly DocumentConfig _key;
        private bool _descending;
        private readonly List<ThenBy> _thenByList = new List<ThenBy>();


        public OrderBy(DocumentConfig key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _key = key;
        }

        public OrderBy Descending(bool descending = true)
        {
            if (_thenByList.Count == 0)
                _descending = descending;
            else
                _thenByList.Last().Descending = descending;
            return this;
        }

        public IEnumerable<IDocument> Execute(IReadOnlyList<IDocument> inputs, IExecutionContext context)
        {
            var orderdList = _descending
                ? inputs.OrderByDescending(x => _key(x, context))
                : inputs.OrderBy(x => _key(x, context));
            foreach (var thenBy in _thenByList)
            {
                orderdList = thenBy.Descending
                    ? orderdList.ThenBy(x => thenBy.Key(x, context))
                    : orderdList.ThenByDescending(x => thenBy.Key(x, context));
            }

            return orderdList;
        }

        private class ThenBy
        {
            public DocumentConfig Key { get; }
            public bool Descending { get; set; }
        }
    }
}
