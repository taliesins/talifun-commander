using System;
using System.ComponentModel.Composition.Hosting;

namespace Talifun.Commander.Command
{
    public sealed class CommandContainer
    {
        private CommandContainer()
        {
        }

        public static readonly CommandContainer Instance = new CommandContainer();

        private CompositionContainer _container;
        public CompositionContainer Container
        {
            get
            {
                if (_container == null)
                {
					var core = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "Talifun.Commander.Command.dll");
					var plugins = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "Talifun.Commander.Command.*.dll");

                    var aggregatecatalogue = new AggregateCatalog(core, plugins);
                    _container = new CompositionContainer(aggregatecatalogue);
                }

                return _container;
            }
        }
    }
}
