﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Hudl.FFmpeg.Common;
using Hudl.FFmpeg.Enums;
using Hudl.FFmpeg.Resources.BaseTypes;
using Hudl.FFmpeg.Resources.Interfaces;

namespace Hudl.FFmpeg.Resources
{
    /// <summary>
    /// this is a list of resource files used to describe a returned collection of files
    /// </summary>
    public class Resource
    {
		private static readonly object AllTypesLock = new object();
        private static readonly List<Type> AllTypes = new List<Type>();

        /// <summary>
        /// Creates a new resource with the full path name provided.
        /// </summary>
        public static TContainer CreateOutput<TContainer>()
            where TContainer : class, IContainer, new()
        {
            var temporaryResource = new TContainer();
            return Create<TContainer>(ResourceManagement.CommandConfiguration.OutputPath, temporaryResource.Name);
        }

        /// <summary>
        /// Creates a new resource with the full path name provided.
        /// </summary>
        public static TContainer Create<TContainer>()
            where TContainer : class, IContainer, new()
        {
            var temporaryResource = new TContainer();
            return Create<TContainer>(ResourceManagement.CommandConfiguration.TempPath, temporaryResource.FullName);
        }

        /// <summary>
        /// Creates a new resource with the full path name provided.
        /// </summary>
        public static TContainer Create<TContainer>(string fullPath)
            where TContainer : class, IContainer, new()
        {
            var fileName = Helpers.GetNameFromFullName(fullPath);
            var filePath = Helpers.GetPathFromFullName(fullPath);

            return Create<TContainer>(filePath, fileName);
        }


        /// <summary>
        /// Creates a new resource with the file path, name provided.
        /// </summary>
        public static TContainer Create<TContainer>(string filePath, string fileName)
            where TContainer : class, IContainer, new()
        {
            var resource = new TContainer
                {
                    Name = fileName,
                    Path = filePath,
                };

            return resource; 
        }


        /// <summary>
        /// Creates a new resource with the full path name provided.
        /// </summary>
        public static IContainer From(string fullPath)
        {
            var fileName = Helpers.GetNameFromFullName(fullPath);
            var filePath = Helpers.GetPathFromFullName(fullPath);
            return From(filePath, fileName); 
        }
        
        /// <summary>
        /// Creates a new resource with the full path name provided.
        /// </summary>
        private static IContainer From(string filePath, string fileName)
        {
            var fileExtension = Helpers.GetExtensionFromFullName(fileName);
			lock (AllTypesLock)
			{
				if (AllTypes.Count == 0)
				{
					AllTypes.AddRange(GetTypes<IContainer>());
				}
			}

            var correctContainer = AllTypes.FirstOrDefault(t => t.Name.ToUpper() == fileExtension.ToUpper());
            if (correctContainer == null)
            {
                throw new InvalidOperationException("Cannot derive resource type from path provided.");
            }

            var newInstance = (IContainer)Activator.CreateInstance(correctContainer);
            newInstance.Path = filePath;
            newInstance.Name = fileName;
            return newInstance;
        }

        private static List<Type> GetTypes<T>()
        {
            // this only works if the assemblies are loaded already :-(
            List<Assembly> assemblies = new List<Assembly>();
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                var assemblyName = AssemblyLoadContext.GetAssemblyName(module.FileName);
                var assembly = Assembly.Load(assemblyName);
                assemblies.Add(assembly);
            }

            return assemblies
                .SelectMany(a =>
                    {
                        try
                        {
                            return a.GetTypes();

                        }
                        catch (Exception)
                        {
                            return new Type[0];
                        }
                    })
                .Where(t => !t.GetTypeInfo().IsAbstract && typeof(T).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null).ToList();
        }
    }
}
