using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace NDMod.Common.Utilities
{
    public sealed class OOPHelper
    {
        private static Type[] AllTypes => ModLoader.Mods.Skip(1).SelectMany(mod => mod.Code.GetTypes()).ToArray();
        /// <summary>
        /// Get all classes that extend T.
        /// </summary>
        /// <typeparam name="T">The type you want to check has a subclass.</typeparam>
        /// <returns>A List of all classes that extend T. </returns>
        public static List<T> GetSubclasses<T>() where T : class
        {
            var types = AllTypes;
            List<T> TypeListBuffer = new List<T>();
            for (int Index = 0; Index < types.Length; Index++)
                if (types[Index].IsSubclassOf(typeof(T)))
                    TypeListBuffer.Add(Activator.CreateInstance(types[Index]) as T);
            return TypeListBuffer;
        }
    }
}