using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using MicroModels.Description;

namespace MicroModels.Utilities
{
    internal class MicroModelObjectBuilder
    {
        private static readonly Dictionary<TypeSignature, Type> TypesCache = new Dictionary<TypeSignature, Type>();

        private static readonly AssemblyBuilder MicroModelAssemblyBuilder = 
            AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("MicroModels"), AssemblyBuilderAccess.Run);

        private static readonly ModuleBuilder MicroModelModuleBuilder = 
            MicroModelAssemblyBuilder.DefineDynamicModule("MicroModelsModule", true);

        private static readonly ConstructorInfo MicroModelObjectConstructor =
            typeof(MicroModelObject).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                                                    new[] { typeof(MicroModelBase) }, null);

        private static readonly MethodInfo GetValueMethod = 
            typeof(MicroModelObject).GetMethod("GetValue", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo SetValueMethod = 
            typeof(MicroModelObject).GetMethod("SetValue", BindingFlags.Instance | BindingFlags.NonPublic);

        public static Type GetMicroModelObjectType(IEnumerable<PropertyDescriptor> properties)
        {
            Type type;
            var signature = new TypeSignature(properties);

            if (!TypesCache.TryGetValue(signature, out type))
            {
                type = CreateMicroModelObjectType(properties);
                TypesCache.Add(signature, type);
            }

            return type;
        }

        private static Type CreateMicroModelObjectType(IEnumerable<PropertyDescriptor> properties)
        {
            var typeBuilder =
                MicroModelModuleBuilder.DefineType("MicroModelObject_" + Guid.NewGuid(), TypeAttributes.Public, typeof(MicroModelObject));

            CreateConstructor(typeBuilder);

            foreach (var property in properties)
            {
                var propertyBuilder = typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null);

                CreateGetter(typeBuilder, propertyBuilder, property);
                CreateSetter(typeBuilder, propertyBuilder, property);
            }

            return typeBuilder.CreateType();

        }

        private static void CreateConstructor(TypeBuilder typeBuilder)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.HideBySig,
                CallingConventions.Standard,
                MicroModelObjectConstructor.GetParameters().Select(p => p.ParameterType).ToArray());
            
            var constructorIL = constructorBuilder.GetILGenerator();
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Ldarg_1);
            constructorIL.Emit(OpCodes.Call, MicroModelObjectConstructor);
            constructorIL.Emit(OpCodes.Ret);
        }

        private static void CreateGetter(TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, PropertyDescriptor property)
        {
            var getMethodBuilder = typeBuilder.DefineMethod(
                "get_" + property.Name,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                CallingConventions.HasThis,
                property.PropertyType, Type.EmptyTypes);

            var getMethodIL = getMethodBuilder.GetILGenerator();
            getMethodIL.Emit(OpCodes.Ldarg_0);
            getMethodIL.Emit(OpCodes.Ldstr, property.Name);
            getMethodIL.Emit(OpCodes.Callvirt, GetValueMethod.MakeGenericMethod(property.PropertyType));
            getMethodIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
        }

        private static void CreateSetter(TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, PropertyDescriptor property)
        {
            if (property.IsReadOnly)
            {
                return;
            }

            var setMethodBuilder = typeBuilder.DefineMethod(
                "set_" + property.Name,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                CallingConventions.HasThis,
                null, new[]{property.PropertyType});

            var setMethodIL = setMethodBuilder.GetILGenerator();
            setMethodIL.Emit(OpCodes.Ldarg_0);
            setMethodIL.Emit(OpCodes.Ldstr, property.Name);
            setMethodIL.Emit(OpCodes.Ldarg_1);
            setMethodIL.Emit(OpCodes.Callvirt, SetValueMethod.MakeGenericMethod(property.PropertyType));
            setMethodIL.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(setMethodBuilder);
        }
    }
}