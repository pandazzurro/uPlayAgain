using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uPlayAgain
{
    public sealed class OwinContextDisposal : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public OwinContextDisposal(IOwinContext owinContext)
        {
            if (HttpContext.Current == null) return;

            //TODO: Add all owin context disposable types here
            //_disposables.Add(owinContext.Get<MyObject2>());

            HttpContext.Current.DisposeOnPipelineCompleted(this);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}