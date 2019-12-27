using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Khooversoft.MessageNet.Management;
using Khooversoft.Toolbox.Standard;

namespace MessageNet.Management.Test.RouteManagement
{
    public class BlobRepositoryFake : IBlobRepository
    {
        private readonly BlobStoreConnection _blobStoreConnection;
        private readonly Dictionary<string, string> _repository = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public BlobRepositoryFake(BlobStoreConnection blobStoreConnection)
        {
            _blobStoreConnection = blobStoreConnection;
        }

        public Task ClearAll(IWorkContext context)
        {
            _repository.Clear();
            return Task.CompletedTask;
        }

        public Task CreateContainer(IWorkContext context)
        {
            return Task.CompletedTask;
        }

        public Task Delete(IWorkContext context, string path)
        {
            _repository.Remove(path);
            return Task.CompletedTask;
        }

        public Task DeleteContainer(IWorkContext context)
        {
            return Task.CompletedTask;
        }

        public Task<string?> Get(IWorkContext context, string path)
        {
            if (!_repository.ContainsKey(path)) return Task.FromResult((string?)null);

            return Task.FromResult<string?>(_repository[path]);
        }

        public Task<IReadOnlyList<string>> List(IWorkContext context, string search)
        {
            var searchTest = new NodeIdCompare(search);

            var result = _repository
                .Where(x => searchTest.Test(x.Key))
                .Select(x => x.Key)
                .ToList();

            return Task.FromResult<IReadOnlyList<string>>(result);
        }

        public Task Set(IWorkContext context, string path, string data)
        {
            _repository[path] = data;
            return Task.CompletedTask;
        }
    }
}
