using Khooversoft.Toolbox.Standard;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageHub.Management
{
    public class MemoryRegisterStore : IRegisterStore
    {
        private readonly ConcurrentDictionary<string, NodeRegistrationModel> _data = new ConcurrentDictionary<string, NodeRegistrationModel>(StringComparer.OrdinalIgnoreCase);

        public MemoryRegisterStore()
        {
        }

        public Task<NodeRegistrationModel?> Get(IWorkContext context, string path)
        {
            if (_data.TryGetValue(path, out NodeRegistrationModel model))
            {
                return Task.FromResult<NodeRegistrationModel?>(model);
            }

            return Task.FromResult<NodeRegistrationModel?>(null);
        }

        public Task<IReadOnlyList<NodeRegistrationModel>> List(IWorkContext context)
        {
            return Task.FromResult<IReadOnlyList<NodeRegistrationModel>>(_data.Values.ToList());
        }

        public Task Remove(IWorkContext context, string path)
        {
            _data.Remove(path, out NodeRegistrationModel _);
            return Task.FromResult(0);
        }

        public Task Set(IWorkContext context, string path, NodeRegistrationModel nodeRegistrationModel)
        {
            nodeRegistrationModel.Verify(nameof(nodeRegistrationModel)).IsNotNull();
            nodeRegistrationModel.NodeId!.Verify(nameof(nodeRegistrationModel.NodeId)).IsNotEmpty();

            _data.AddOrUpdate(nodeRegistrationModel.NodeId!, nodeRegistrationModel, (k, v) => nodeRegistrationModel);
            return Task.CompletedTask;
        }
    }
}
