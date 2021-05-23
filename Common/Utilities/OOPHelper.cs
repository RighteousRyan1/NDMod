using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;

namespace NDMod.Common.Utilities
{
    public class OOPHelper
    {
        public static Type[] AllTypes => Assembly.GetExecutingAssembly().GetTypes();
        /// <summary>
        /// Get all classes that extend T.
        /// </summary>
        /// <typeparam name="T">The type you want to check has a subclass.</typeparam>
        /// <returns>A List of all classes that extend T. </returns>
        public static List<T> GetSubclasses<T>() where T : class
        {
            var Types = AllTypes;
            List<T> TypeListBuffer = new List<T>();

            for (int Index = 0; Index < Types.Length; Index++)
            {
                if (Types[Index].IsSubclassOf(typeof(T)))
                {
                    TypeListBuffer.Add(Activator.CreateInstance(Types[Index]) as T);
                }
            }

            return TypeListBuffer;
        }
    }
}