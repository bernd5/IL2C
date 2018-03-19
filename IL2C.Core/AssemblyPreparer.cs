﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using IL2C.ILConveters;
using IL2C.Translators;
using IL2C.Metadata;

namespace IL2C
{
    public static class AssemblyPreparer
    {
        private struct ILBody
        {
            public readonly Label Label;
            public readonly ILConverter ILConverter;
            public readonly CodeInformation Code;

            public ILBody(Label label, ILConverter ilc, CodeInformation code)
            {
                this.Label = label;
                this.ILConverter = ilc;
                this.Code = code;
            }
        }

        private static IEnumerable<ILBody> DecodeAndEnumerateILBodies(
            DecodeContext decodeContext)
        {
            while (true)
            {
                // TODO: Make simplist with Cecil's Instruction type.
                if (decodeContext.MoveNext() == false)
                {
                    break;
                }

                var code = decodeContext.Current;
                if (Utilities.TryGetILConverter(code.OpCode, out var ilc) == false)
                {
                    throw new InvalidProgramSequenceException(
                        "Invalid opcode: Method={0}, OpCode={1}, Offset={2}",
                        decodeContext.Method.FriendlyName,
                        code.OpCode.Name,
                        code.Offset);
                }

                yield return new ILBody(new Label(code.Offset), ilc, code);

                if (ilc.IsEndOfPath)
                {
                    yield break;
                }
            }
        }

        private static readonly SequencePoint[] empty = new SequencePoint[0];

        private static PreparedMethodInformation PrepareMethodBody(
            IMetadataContext metadataContext,
            IPrepareContext prepareContext,
            IMethodInformation method)
        {
            var localVariables = method.LocalVariables
                // If found non named local variable, force named "local[n]__"
                .GroupBy(variable => variable.SymbolName)
                // If contains both named symbol each different scope (in the method by C#'s block), try to named with index number.
                .SelectMany(g =>
                {
                    var list = g.ToArray();
                    return (list.Length >= 2)
                        ? list.Select((variable, index) => new VariableInformation(
                            variable.Index,
                            string.Format("{0}{1}__", g.Key, index),
                            variable.TargetType))
                        : new[] { new VariableInformation(
                            list[0].Index,
                            string.Format("{0}__", g.Key),
                            list[0].TargetType) };
                })
                .OrderBy(e => e.Index)
#if DEBUG
                .Select((e, index) =>
                {
                    Debug.Assert(e.Index == index);
                    return e;
                })
#endif
                .ToArray();

            foreach (var local in localVariables)
            {
                prepareContext.RegisterType(local.TargetType);
            }

            var decodeContext = new DecodeContext(
                metadataContext,
                method,
                prepareContext);

            //////////////////////////////////////////////////////////////////////////////
            // Important:
            //   It's core decoding sequence.
            //   The flow analysis can't predict by sequential path.
            //   So, it reorders by IL offset.

            var generators = decodeContext
                .Traverse(dc => dc.TryDequeueNextPath() ? dc : null, true)
                .SelectMany(dc =>
                    from ilBody in DecodeAndEnumerateILBodies(dc)
                    let generator = ilBody.ILConverter.Apply(ilBody.Code.Operand, dc)
                    select new PreparedILBody(
                        ilBody.Label,
                        generator,
                        dc.UniqueCodeBlockIndex,
                        ilBody.Code,
                        dc.DecodingPathNumber))
                .OrderBy(ilb => ilb.UniqueCodeBlockIndex)
                .ThenBy(ilb => ilb.Label.Offset)
                .ToDictionary(ilb => ilb.Label.Offset, ilb => ilb.Generator);

            //////////////////////////////////////////////////////////////////////////////

            var stacks = decodeContext
                .ExtractStacks()
                .ToArray();

            var labelNames = decodeContext
                .ExtractLabelNames();

            return new PreparedMethodInformation(
                method,
                stacks,
                labelNames,
                generators);
        }

        private static PreparedMethodInformation PrepareMethod(
            IMetadataContext metadataContext,
            IPrepareContext prepareContext,
            IMethodInformation method)
        {
            var returnType = method.ReturnType;
            var parameters = method.Parameters;

            prepareContext.RegisterType(returnType);
            foreach (var parameter in parameters)
            {
                prepareContext.RegisterType(parameter.TargetType);
            }

            if (method.IsAbstract)
            {
                Debug.Assert(!method.HasBody);
                return null;
            }

            // If delegate's method
            if (method.Context.DelegateType.IsAssignableFrom(method.DeclaringType))
            {
                // "extern internalcall"
                Debug.Assert(method.IsExtern);
                Debug.Assert(!method.HasBody);
                return null;
            }

            if (method.IsExtern)
            {
                var pinvokeInfo = method.PInvokeInfo;
                if (pinvokeInfo == null)
                {
                    throw new InvalidProgramSequenceException(
                        "Missing DllImport attribute at P/Invoke entry: Method={0}",
                        method.FriendlyName);
                }

                // TODO: Switch DllImport.Value include direction to library direction.
                if (string.IsNullOrWhiteSpace(pinvokeInfo.Module.Name))
                {
                    throw new InvalidProgramSequenceException(
                        "Not given DllImport attribute argument. Name={0}",
                        method.FriendlyName);
                }

                prepareContext.RegisterPrivateIncludeFile(pinvokeInfo.Module.Name);

                Debug.Assert(!method.HasBody);
                return null;
            }

            Debug.Assert(method.HasBody);

            return PrepareMethodBody(
                metadataContext,
                prepareContext,
                method);
        }

        internal static PreparedMethodInformations Prepare(
            IMetadataContext metadataContext,
            TranslateContext translateContext,
            Func<IMethodInformation, bool> predict)
        {
            IPrepareContext prepareContext = translateContext;

            var allTypes = translateContext.Assembly.Modules
                .SelectMany(module => module.Types)
                .Where(type => type.IsValidDefinition)
                .ToArray();
            var types = allTypes
                .Where(type => !(type.IsPublic || type.IsNestedPublic || type.IsNestedFamily || type.IsNestedFamilyOrAssembly))
                .ToArray();

            // Lookup type references.
            foreach (var type in types)
            {
                prepareContext.RegisterType(type);
            }

            // Lookup fields.
            foreach (var field in types.SelectMany(type => type.Fields))
            {
                prepareContext.RegisterType(field.FieldType);
            }

            // Construct result.
            return new PreparedMethodInformations(
                (from type in allTypes
                 from method in type.DeclaredMethods
                 where predict(method) &&
                    // Exclude delegate's constructor
                    (!method.IsConstructor || !method.DeclaringType.IsDelegate)
                 let preparedMethod = PrepareMethod(metadataContext, prepareContext, method)
                 where preparedMethod != null
                 select preparedMethod)
                .ToDictionary(
                    preparedMethod => preparedMethod.Method,
                    preparedMethod => preparedMethod));
        }

        public static PreparedMethodInformations Prepare(TranslateContext translateContext)
        {
            return Prepare(
                translateContext,
                // Standard methods and instance constructors.
                method => !method.IsConstructor || !method.IsStatic);
        }
    }
}
