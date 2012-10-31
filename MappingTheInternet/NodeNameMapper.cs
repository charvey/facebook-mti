﻿using MappingTheInternet.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingTheInternet
{
    public class NodeNameMapper
    {
        private Dictionary<string,Node<ASNode,ConnectionSchedule>> _nodeNameMap;
        private Dictionary<string, string[]> _nodeNameGroups;

        public NodeNameMapper()
        {            
            _nodeNameMap = new Dictionary<string, Node<ASNode, ConnectionSchedule>>();

            var nodeNameGroups = NodeNameGrouper.Build();
            _nodeNameGroups = new Dictionary<string, string[]>();

            foreach (var nodeNameGroup in nodeNameGroups)
            {
                foreach (var name in nodeNameGroup)
                {
                    _nodeNameGroups[name] = nodeNameGroup;
                    _nodeNameMap[name] = null;
                }
            }
        }

        public Node<ASNode, ConnectionSchedule> Get(string name)
        {
            return _nodeNameMap[name];
        }

        public void Set(string name, Node<ASNode, ConnectionSchedule> node)
        {
            var names = _nodeNameGroups[name].ToArray();

            foreach (var n in names)
            {
                _nodeNameMap[n] = node;
                _nodeNameGroups.Remove(n);
            }
        }
    }
}
