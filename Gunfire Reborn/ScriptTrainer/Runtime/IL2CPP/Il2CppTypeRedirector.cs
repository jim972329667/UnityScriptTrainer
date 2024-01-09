using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptTrainer.Runtime
{
    internal static class Il2CppTypeRedirector
    {
        static readonly Dictionary<string, string> redirectors = new Dictionary<string, string>();

        public static string GetAssemblyQualifiedName(Il2CppSystem.Type type)
        {
            StringBuilder sb = new StringBuilder();
            ProcessType(sb, type);
            return sb.ToString();
        }

        static void ProcessType(StringBuilder sb, Il2CppSystem.Type type)
        {
            if (type.IsPrimitive || type.FullName == "System.String")
            {
                sb.Append(type.AssemblyQualifiedName);
                return;
            }

            if (!string.IsNullOrEmpty(type.Namespace))
            {
                if (type.FullName.StartsWith("System."))
                    sb.Append("Il2Cpp");

                sb.Append(type.Namespace)
                  .Append('.');
            }

            int start = sb.Length;
            Il2CppSystem.Type declaring = type.DeclaringType;
            while (declaring != null)
            {
                sb.Insert(start, $"{declaring.Name}+");
                declaring = declaring.DeclaringType;
            }

            sb.Append(type.Name);

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                Il2CppSystem.Type[] genericArgs = type.GetGenericArguments();

                // Process and append each type argument (recursive)
                sb.Append('[');
                int i = 0;
                foreach (Il2CppSystem.Type typeArg in genericArgs)
                {
                    sb.Append('[');
                    ProcessType(sb, typeArg);
                    sb.Append(']');
                    i++;
                    if (i < genericArgs.Length)
                        sb.Append(',');
                }
                sb.Append(']');
            }

            // Append the assembly signature
            sb.Append(", ");

            if (type.FullName.StartsWith("System."))
            {
                if (!redirectors.ContainsKey(type.Assembly.FullName) && !TryRedirectSystemType(type))
                    // No redirect found for type?
                    throw new TypeLoadException($"No Il2CppSystem redirect found for system type: {type.AssemblyQualifiedName}");
                else
                    // Type redirect was set up
                    sb.Append(redirectors[type.Assembly.FullName]);

            }
            else // no redirect required
                sb.Append(type.Assembly.FullName);
        }

        static bool TryRedirectSystemType(Il2CppSystem.Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
                type = type.GetGenericTypeDefinition();

            if (ReflectionUtility.AllTypes.TryGetValue($"Il2Cpp{type.FullName}", out Type il2cppType))
            {
                redirectors.Add(type.Assembly.FullName, il2cppType.Assembly.FullName);
                return true;
            }

            return false;
        }
    }
}
