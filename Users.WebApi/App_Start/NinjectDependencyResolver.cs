﻿using Ninject;
using Ninject.Web.Common;

namespace Users.WebApi.App_Start
{
    using DependencyResolver;

    public static class NinjectDependencyResolver
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Resolve()
        {
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.ResolveDependencies();
            return kernel;
        }
    }
}