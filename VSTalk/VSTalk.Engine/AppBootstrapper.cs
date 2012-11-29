using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Reflection;

namespace VSTalk.Engine
{
    public class AppBootstrapper : Bootstrapper
    {
        CompositionContainer container;

        // INSERT THIS CONSTRUCTUR!!!
        public AppBootstrapper() : base(false) 
        { 
        }


        VSTalk.Engine.Core.IWindowsManager _windowManagerFromEngine;
        protected override void Configure()
        {
            ViewLocator.NameTransformer.AddRule
            (
            @"(?<nsbefore>([A-Za-z_]\w*\.)*)?(?<nsvm>ViewModels\.)(?<nsafter>([A-Za-z_]\w*\.)*)(?<basename>[A-Za-z_]\w*)(?<suffix>ViewModel$)",
            @"${nsbefore}Views.${nsafter}${basename}View",
            @"(([A-Za-z_]\w*\.)*)?ViewModels\.([A-Za-z_]\w*\.)*[A-Za-z_]\w*ViewModel$"
            );


            var aggCatalog =
                new AggregateCatalog(
                    AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
                    );

            container = new CompositionContainer(aggCatalog);

            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(AppBootstrapperHelper.windowsManager);
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(container);

            container.Compose(batch);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return AssemblySource.Instance.Any() ?
                           new Assembly[] { } :
                           new[] { typeof(AppBootstrapper).Assembly };
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }
    }
}
