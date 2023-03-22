// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Parser;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SimpleIdServer.Scim.Domain
{
    public static class SCIMRepresentationExtensions
    {
        public static void FilterAttributes(this IEnumerable<SCIMRepresentation> representations, IEnumerable<SCIMAttributeExpression> includedAttributes, IEnumerable<SCIMAttributeExpression> excludedAttributes)
        {
            foreach (var representation in representations) representation.FilterAttributes(includedAttributes, excludedAttributes);
        }

        public static void FilterAttributes(this SCIMRepresentation representation, IEnumerable<SCIMAttributeExpression> includedAttributes, IEnumerable<SCIMAttributeExpression> excludedAttributes)
        {
            var queryableAttributes = representation.FlatAttributes.AsQueryable();
            representation.RefreshHierarchicalAttributesCache();
            if (includedAttributes != null && includedAttributes.Any())
            {
                var attrs = queryableAttributes.FilterAttributes(includedAttributes).ToList();
                var includedFullPathLst = (includedAttributes != null && includedAttributes.Any()) ? includedAttributes.Where(i => i is SCIMComplexAttributeExpression).Select(i => i.GetFullPath()) : new List<string>();
                representation.FlatAttributes = attrs.Where(a => a != null).SelectMany(_ =>
                {
                    var lst = new List<SCIMRepresentationAttribute> { _ };
                    lst.AddRange(_.Children.Where(c => includedFullPathLst.Any(f => c.FullPath.StartsWith(f))));
                    return lst;
                }).ToList();
            }
            else if (excludedAttributes != null && excludedAttributes.Any())
                representation.FlatAttributes = queryableAttributes.FilterAttributes(excludedAttributes, false).ToList();
        }

        public static IQueryable<SCIMRepresentationAttribute> FilterAttributes(this IQueryable<SCIMRepresentationAttribute> representationAttributes, IEnumerable<SCIMAttributeExpression> attributes, bool isIncluded = true, string propertyName = "CachedChildren")
        {
            var enumarableType = typeof(Queryable);
            var whereMethod = enumarableType.GetMethods()
                 .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                 .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentationAttribute));
            var representationParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), "rp");
            var startWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
            var fullPathProperty = Expression.Property(representationParameter, "FullPath");
            var namespaceProperty = Expression.Property(representationParameter, "Namespace");
            Expression expression = null;
            foreach (var attribute in attributes)
            {
                Expression record = null;
                if (attribute.TryContainsGroupingExpression(out SCIMComplexAttributeExpression complexAttributeExpression))
                {
                    record = attribute.EvaluateAttributes(representationParameter, propertyName);
                }
                else
                {
                    if (isIncluded)
                    {
                        record = FilterIncludedAttributes(fullPathProperty, attribute, null, attribute.GetFullPath(false));
                    }
                    else
                    {
                        record = Expression.Call(fullPathProperty, startWith, Expression.Constant(attribute.GetFullPath(false)));
                    }
                }

                var ns = attribute.GetNamespace();
                if (!string.IsNullOrWhiteSpace(ns))
                {
                    record = Expression.And(record, Expression.Equal(namespaceProperty, Expression.Constant(ns)));
                }

                if (expression == null)
                {
                    expression = record;
                }
                else
                {
                    expression = Expression.Or(expression, record);
                }
            }

            if (expression == null)
            {
                return representationAttributes;
            }

            if (!isIncluded)
            {
                expression = Expression.Not(expression);
            }

            var equalLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(expression, representationParameter);
            var whereExpr = Expression.Call(whereMethod, Expression.Constant(representationAttributes), equalLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentationAttribute>), "f");
            var finalSelectRequestBody = Expression.Lambda(whereExpr, new ParameterExpression[] { finalSelectArg });
            var filteredAttrs = (IQueryable<SCIMRepresentationAttribute>)finalSelectRequestBody.Compile().DynamicInvoke(representationAttributes);
            return filteredAttrs;
        }

        private static Expression FilterIncludedAttributes(MemberExpression member, SCIMAttributeExpression attr, string parentPath, string fullPath)
        {
            var fp = string.IsNullOrWhiteSpace(parentPath) ? attr.Name : $"{parentPath}.{attr.Name}";
            fp = SCIMAttributeExpression.RemoveNamespace(fp);
            Expression equal = null;
            if (fullPath == fp)
            {
                var startWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                equal = Expression.Call(member, startWith, Expression.Constant(fullPath));
            }
            else
            {
                equal = Expression.Equal(member, Expression.Constant(fp));
            }

            Expression sub = null;
            if (attr.Child != null)
            {
                sub = FilterIncludedAttributes(member, attr.Child, fp, fullPath);
            }

            return sub == null ? equal : Expression.Or(equal, sub);
        }

        public static void ApplyEmptyArray(this SCIMRepresentation representation)
        {
            foreach(var schema in representation.Schemas)
            {
                var attrs = schema.HierarchicalAttributes.Select(a => a.Leaf).Where(a => a.MultiValued);
                foreach (var attr in attrs)
                {
                    if (!representation.FlatAttributes.Any(a => a.SchemaAttributeId == attr.Id))
                    {
                        representation.FlatAttributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), attr, schema.Id));
                    }
                }
            }
        }

        public static List<SCIMPatchResult> ApplyPatches(this SCIMRepresentation representation, ICollection<PatchOperationParameter> patches, IEnumerable<SCIMAttributeMapping> attributeMappings, bool ignoreUnsupportedCanonicalValues)
        {
            var result = new List<SCIMPatchResult>();
            foreach (var patch in patches)
            {
                var scimFilter = SCIMFilterParser.Parse(patch.Path, representation.Schemas);
                var schemaAttributes = representation.Schemas.SelectMany(_ => _.Attributes);
                List<SCIMRepresentationAttribute> attributes = null;
                string fullPath = null;
                SCIMRepresentationAttribute emptyParent = null;
                if (scimFilter != null)
                {
                    var scimExpr = scimFilter as SCIMAttributeExpression;
                    if (scimExpr == null)
                    {
                        throw new SCIMAttributeException(Global.InvalidAttributeExpression);
                    }

                    fullPath = scimExpr.GetFullPath();
                    schemaAttributes = representation.Schemas
                        .Select(s => s.GetAttribute(fullPath))
                        .Where(s => s != null);
                    fullPath = SCIMAttributeExpression.RemoveNamespace(fullPath);
                    attributes = scimExpr.EvaluateAttributes(representation.HierarchicalAttributes.AsQueryable(), true).ToList();
                    emptyParent = scimExpr.BuildEmptyAttributes().FirstOrDefault();
                }
                else
                {
                    attributes = representation.FlatAttributes.Where(h => h.IsLeaf()).ToList();
                }

                var removeCallback = new Action<ICollection<SCIMRepresentationAttribute>>((attrs) =>
                {
                    foreach (var a in attrs)
                    {
                        var removedAttrs = representation.RemoveAttributeById(a);
                        foreach (var removedAttr in removedAttrs)
                        {
                            result.Add(new SCIMPatchResult { Attr = removedAttr, Operation = SCIMPatchOperations.REMOVE, Path = removedAttr.FullPath });
                        }
                    }
                });
                switch (patch.Operation)
                {
                    case SCIMPatchOperations.ADD:
                        {
                            bool hasExternalId = false;
                            if ((hasExternalId = TryGetExternalId(patch, out string externalId)))
                            {
                                representation.ExternalId = externalId;
                                result.Add(new SCIMPatchResult { Attr = new SCIMRepresentationAttribute(), Operation = SCIMPatchOperations.ADD, Path = StandardSCIMRepresentationAttributes.ExternalId });
                            }

                            if (!hasExternalId && (schemaAttributes == null || !schemaAttributes.Any()))
                            {
                                throw new SCIMNoTargetException(string.Format(Global.AttributeIsNotRecognirzed, patch.Path));
                            }

                            var newAttributes = ExtractRepresentationAttributesFromJSON(representation.Schemas, schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                            newAttributes = RemoveStandardReferenceProperties(newAttributes, attributeMappings);
                            newAttributes = FilterDuplicate(attributes, newAttributes);
                            if(newAttributes.Any()) removeCallback(attributes.Where(a => !a.SchemaAttribute.MultiValued && a.FullPath == fullPath).ToList());
                            var isAttributeExits = !string.IsNullOrWhiteSpace(fullPath) && attributes.Any(a => a.FullPath == fullPath);
                            var parentExists = representation.FlatAttributes.Any(fa => fa.SchemaAttributeId == emptyParent?.SchemaAttribute?.Id);
                            if(newAttributes.Any(a => a.GetLevel() == 1) || attributes.Any() || parentExists)
                                foreach (var newAttribute in newAttributes.OrderBy(a => a.GetLevel()))
                                {
                                    var path = newAttribute.FullPath;
                                    var schemaAttr = newAttribute.SchemaAttribute;
                                    IEnumerable<SCIMRepresentationAttribute> parentAttributes = null;
                                    if (fullPath == path)
                                    {
                                        var attr = attributes.FirstOrDefault(a => a.FullPath == fullPath);
                                        if (attr != null)
                                        {
                                            var parent = representation.GetParentAttribute(attr);
                                            if (parent != null)
                                            {
                                                parentAttributes = new[] { parent };
                                            }
                                        }
                                        else
                                        {
                                            parentAttributes = representation.GetParentAttributesByPath(fullPath).ToList();
                                        }
                                    }

                                    if (schemaAttr.MultiValued && schemaAttr.Type != SCIMSchemaAttributeTypes.COMPLEX)
                                    {
                                        var filteredAttribute = attributes.FirstOrDefault(_ => _.FullPath == path);
                                        if (filteredAttribute != null)
                                        {
                                            newAttribute.AttributeId = filteredAttribute.AttributeId;
                                        }

                                        representation.AddAttribute(newAttribute);
                                    }
                                    else if (parentAttributes != null && parentAttributes.Any())
                                    {
                                        foreach (var parentAttribute in parentAttributes)
                                        {
                                            var attr = parentAttribute.CachedChildren.FirstOrDefault(c => c.SchemaAttributeId == newAttribute.SchemaAttributeId);
                                            if (attr != null) representation.FlatAttributes.Remove(attr);
                                            representation.AddAttribute(parentAttribute, newAttribute);
                                        }
                                    }
                                    else
                                    {
                                        representation.FlatAttributes.Add(newAttribute);
                                    }

                                    attributes.Add(newAttribute);
                                    result.Add(new SCIMPatchResult { Attr = newAttribute, Operation = SCIMPatchOperations.ADD, Path = fullPath });
                                }
                            else
                            {
                                var flatAttrs = emptyParent.ToFlat();
                                foreach(var newAttribute in newAttributes)
                                {
                                    var attrsToRemove = flatAttrs.Where(a => a.SchemaAttribute.Id == newAttribute.SchemaAttribute.Id).ToList();
                                    foreach (var attrToRemove in attrsToRemove)
                                    {
                                        var parentAttr = flatAttrs.First(a => a.Id == attrToRemove.ParentAttributeId);
                                        parentAttr.Children.Remove(attrToRemove);
                                        parentAttr.Children.Add(newAttribute);
                                        newAttribute.ParentAttributeId = parentAttr.Id;
                                    }

                                    flatAttrs = emptyParent.ToFlat();
                                    foreach (var attr in flatAttrs)
                                    {
                                        attr.RepresentationId = representation.Id;
                                        representation.AddAttribute(attr);
                                    }

                                    result.Add(new SCIMPatchResult { Attr = emptyParent, Operation = SCIMPatchOperations.ADD, Path = fullPath });
                                }
                            }
                        }
                        break;
                    case SCIMPatchOperations.REMOVE:
                        {
                            if (scimFilter == null)
                            {
                                throw new SCIMNoTargetException(string.Format(Global.InvalidPath, patch.Path));
                            }

                            if (SCIMFilterParser.DontContainsFilter(patch.Path) && patch.Value != null)
                            {
                                var excludedAttributes = ExtractRepresentationAttributesFromJSON(representation.Schemas, schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                                excludedAttributes = RemoveStandardReferenceProperties(excludedAttributes, attributeMappings);
                                excludedAttributes = SCIMRepresentation.BuildHierarchicalAttributes(excludedAttributes);
                                attributes = attributes.Where(a => excludedAttributes.Any(ea => ea.IsSimilar(a, true))).ToList();
                            }

                            var removedRequiredAttributes = attributes.Where(a => a.SchemaAttribute.Required);
                            if(removedRequiredAttributes.Any())
                                throw new SCIMImmutableAttributeException(string.Format(string.Format(Global.RequiredAttributesCannotBeRemoved, string.Join(",", removedRequiredAttributes.Select(r => r.FullPath)))));

                            var removedReadOnlyAttributes = attributes.Where(a => a.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.READONLY);
                            if (removedReadOnlyAttributes.Any())
                                throw new SCIMImmutableAttributeException(string.Format(string.Format(Global.ReadOnlyAttributesCannotBeRemoved, string.Join(",", removedReadOnlyAttributes.Select(r => r.FullPath)))));

                            removeCallback(attributes);
                        }
                        break;
                    case SCIMPatchOperations.REPLACE:
                        {
                            bool hasExternalId = false;
                            if ((hasExternalId = TryGetExternalId(patch, out string externalId)))
                            {
                                representation.ExternalId = externalId;
                                result.Add(new SCIMPatchResult { Attr = new SCIMRepresentationAttribute(), Operation = SCIMPatchOperations.ADD, Path = StandardSCIMRepresentationAttributes.ExternalId });
                            }

                            if (!hasExternalId && (schemaAttributes == null || !schemaAttributes.Any()))
                            {
                                throw new SCIMNoTargetException(string.Format(Global.AttributeIsNotRecognirzed, patch.Path));
                            }

                            var complexAttr = scimFilter as SCIMComplexAttributeExpression;
                            if (complexAttr != null && !attributes.Any() && complexAttr.GroupingFilter != null)
                            {
                                throw new SCIMNoTargetException(Global.PatchMissingAttribute);
                            }

                            List<SCIMRepresentationAttribute> parents = new List<SCIMRepresentationAttribute>();
                            if (complexAttr != null)
                            {
                                var attr = attributes.First(a => a.FullPath == fullPath);
                                var parent = string.IsNullOrWhiteSpace(attr.ParentAttributeId) ? attr : representation.GetParentAttribute(attributes.First(a => a.FullPath == fullPath));
                                if (parent != null)
                                {
                                    parents = new List<SCIMRepresentationAttribute> { parent };
                                }
                            }
                            else
                            {
                                parents = representation.GetParentAttributesByPath(fullPath).ToList();
                            }

                            if (scimFilter != null && parents.Any())
                            {
                                foreach (var parent in parents)
                                {
                                    var flatHiearchy = representation.GetFlatHierarchicalChildren(parent).ToList();
                                    var scimAttributeExpression = scimFilter as SCIMAttributeExpression;
                                    var newAttributes = ExtractRepresentationAttributesFromJSON(representation.Schemas, schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                                    var filteredAttrs = attributes.Where(a => a.ParentAttributeId == parent.Id);
                                    if (IsExactlySimilar(filteredAttrs, newAttributes)) break;
                                    foreach (var newAttribute in newAttributes.OrderBy(l => l.GetLevel()))
                                    {
                                        if (!flatHiearchy.Any(a => a.FullPath == newAttribute.FullPath))
                                        {
                                            var parentPath = SCIMRepresentation.GetParentPath(newAttribute.FullPath);
                                            var p = flatHiearchy.FirstOrDefault(a => a.FullPath == parentPath);
                                            if (p != null)
                                            {
                                                representation.AddAttribute(p, newAttribute);
                                            }
                                            else
                                            {
                                                representation.AddAttribute(newAttribute);
                                            }

                                            result.Add(new SCIMPatchResult { Attr = newAttribute, Operation = SCIMPatchOperations.ADD, Path = fullPath });
                                        }
                                    }

                                    result.AddRange(Merge(flatHiearchy, newAttributes, fullPath));
                                }
                            }
                            else
                            {
                                representation.RefreshHierarchicalAttributesCache();
                                result.AddRange(ReplaceComplexMultiValuedAttribute(representation, attributes, schemaAttributes, patch, attributeMappings, ignoreUnsupportedCanonicalValues));
                            }
                        }
                        break;
                }
            }

            var displayNameAttr = representation.FlatAttributes.FirstOrDefault(a => a.FullPath == "displayName");
            if (displayNameAttr != null)
            {
                representation.SetDisplayName(displayNameAttr.ValueString);
            }

            return result;
        }

        private static List<SCIMPatchResult> ReplaceComplexMultiValuedAttribute(SCIMRepresentation representation, List<SCIMRepresentationAttribute> attributes, IEnumerable<SCIMSchemaAttribute> schemaAttributes, PatchOperationParameter patch, IEnumerable<SCIMAttributeMapping> attributeMappings, bool ignoreUnsupportedCanonicalValues)
        {
            var result = new List<SCIMPatchResult>();
            var newAttributes = ExtractRepresentationAttributesFromJSON(representation.Schemas, schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
            newAttributes = RemoveStandardReferenceProperties(newAttributes, attributeMappings);
            var newHierarchicalAttributes = SCIMRepresentation.BuildHierarchicalAttributes(newAttributes);
            var allFullPath = newHierarchicalAttributes.Select(h => h.FullPath);
            var flatAttrs = attributes.SelectMany(a => a.ToFlat()).ToList();
            var targetedAttrs = flatAttrs.Where(a => allFullPath.Contains(a.FullPath)).ToList();
            if (!newHierarchicalAttributes.Any() || IsExactlySimilar(targetedAttrs, newHierarchicalAttributes)) return result;

            var existingAttributesToRemove = targetedAttrs.Where(a => !newHierarchicalAttributes.Any(nha => nha.FullPath == a.FullPath && nha.IsSimilar(a, true)));
            foreach (var existingAttributeToRemove in existingAttributesToRemove)
            {
                representation.RemoveAttributeById(existingAttributeToRemove);
                foreach (var flatAttr in existingAttributeToRemove.ToFlat()) result.Add(new SCIMPatchResult { Attr = flatAttr, Operation = SCIMPatchOperations.REMOVE, Path = flatAttr.FullPath });
            }

            var newAttributesToAdd = newHierarchicalAttributes.Where(na => !targetedAttrs.Any(a => a.FullPath == na.FullPath && na.IsSimilar(a, true)));
            foreach (var newAttributeToAdd in newAttributesToAdd)
            {
                IEnumerable<SCIMRepresentationAttribute> parents = null;
                if(newAttributeToAdd.GetLevel() > 1)
                    parents = representation.GetAttributesByPath(newAttributeToAdd.GetParentFullPath()).ToList();

                if(parents != null)
                {
                    foreach(var parent in parents)
                        representation.AddAttribute(parent, newAttributeToAdd);
                    result.Add(new SCIMPatchResult { Attr = newAttributeToAdd, Operation = SCIMPatchOperations.ADD, Path = newAttributeToAdd.FullPath });
                }
                else
                {
                    foreach (var newFlatAttr in newAttributeToAdd.ToFlat())
                    {
                        representation.AddAttribute(newFlatAttr);
                        result.Add(new SCIMPatchResult { Attr = newFlatAttr, Operation = SCIMPatchOperations.ADD, Path = newFlatAttr.FullPath });
                    }
                }
            }

            return result;
        }

        private static bool IsExactlySimilar(IEnumerable<SCIMRepresentationAttribute> existingAttributes, ICollection<SCIMRepresentationAttribute> newFlatAttributes)
        {
            var excludedAttributeIds = new List<string>();
            var rootAttributes = SCIMRepresentation.BuildHierarchicalAttributes(newFlatAttributes);
            if (rootAttributes.Count() != existingAttributes.Count()) return false;
            foreach(var rootAttribute in rootAttributes)
            {
                var attr = existingAttributes.FirstOrDefault(e => !excludedAttributeIds.Contains(e.Id) && e.IsSimilar(rootAttribute, true));
                if (attr == null) return false;
                excludedAttributeIds.Add(attr.Id);
            }

            return true;
        }

        private static ICollection<SCIMRepresentationAttribute> FilterDuplicate(IEnumerable<SCIMRepresentationAttribute> existingAttributes, ICollection<SCIMRepresentationAttribute> newFlatAttributes)
        {
            var result = new List<SCIMRepresentationAttribute>();
            var rootAttributes = SCIMRepresentation.BuildHierarchicalAttributes(newFlatAttributes);
            foreach(var newAttribute in rootAttributes)
            {
                if (existingAttributes.Any(a => a.IsSimilar(newAttribute, true)))
                {
                    continue;
                }

                result.Add(newAttribute);
            }

            return SCIMRepresentation.BuildFlatAttributes(result);
        }

        private static ICollection<SCIMRepresentationAttribute> RemoveStandardReferenceProperties(ICollection<SCIMRepresentationAttribute> newFlatAttributes, IEnumerable<SCIMAttributeMapping> attributeMappings)
        {
            return newFlatAttributes.Where((nfa) =>
            {
                var parentAttr = newFlatAttributes.FirstOrDefault(a => a.Id == nfa.ParentAttributeId);
                if (parentAttr == null || !attributeMappings.Any(am => am.SourceAttributeId == parentAttr.SchemaAttributeId)) return true;
                if (nfa.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Type || nfa.SchemaAttribute.Name == SCIMConstants.StandardSCIMReferenceProperties.Display) return false;
                return true;
            }).ToList();
        }

        public static JObject ToResponse(this SCIMRepresentation representation, string location, bool isGetRequest = false, bool includeStandardAttributes = true, bool addEmptyArray = false, bool mergeExtensionAttributes = false)
        {
            var jObj = new JObject();
            if (!string.IsNullOrEmpty(representation.Id)) jObj.Add(StandardSCIMRepresentationAttributes.Id, representation.Id);
            if (includeStandardAttributes)
            {
                representation.AddStandardAttributes(location, new List<string> { }, ignore: true);
            }

            if(addEmptyArray)
            {
                representation.ApplyEmptyArray();
            }

            var attributes = representation.HierarchicalAttributes.Select(a =>
            {
                var schema = representation.GetSchema(a);
                var order = 1;
                if (schema != null && schema.IsRootSchema)
                {
                    order = 0;
                }

                return new EnrichParameter(schema, order, a);
            });
            EnrichResponse(attributes, jObj, mergeExtensionAttributes, isGetRequest);
            return jObj;
        }

        private static bool TryGetExternalId(PatchOperationParameter patchOperation, out string externalId)
        {
            externalId = null;
            if (patchOperation.Value == null)
            {
                return false;
            }

            var jObj = patchOperation.Value as JObject;
            if (patchOperation.Path == StandardSCIMRepresentationAttributes.ExternalId && 
                (patchOperation.Value.GetType() == typeof(string) || patchOperation.Value.GetType() == typeof(JValue)))
            {
                externalId = patchOperation.Value.ToString();
                return true;
            }

            if (jObj != null && jObj.ContainsKey(StandardSCIMRepresentationAttributes.ExternalId))
            {
                externalId = jObj[StandardSCIMRepresentationAttributes.ExternalId].ToString();
                return true;
            }

            return false;
        }

        private static List<SCIMPatchResult> Merge(List<SCIMRepresentationAttribute> attributes, ICollection<SCIMRepresentationAttribute> newAttributes, string fullPath)
        {
            var result = new List<SCIMPatchResult>();
            foreach (var attr in attributes)
            {
                var newAttr = newAttributes.FirstOrDefault(na => na.FullPath == attr.FullPath);
                if (newAttr == null)
                {
                    continue;
                }

                attr.ValueString = newAttr.ValueString;
                attr.ValueBoolean = newAttr.ValueBoolean;
                attr.ValueDateTime = newAttr.ValueDateTime;
                attr.ValueDecimal = newAttr.ValueDecimal;
                attr.ValueInteger = newAttr.ValueInteger;
                attr.ValueReference = newAttr.ValueReference;
                attr.ValueBinary = newAttr.ValueBinary;
                result.Add(new SCIMPatchResult { Attr = attr, Operation = SCIMPatchOperations.ADD, Path = fullPath });
            }

            return result;
        }

        private static ICollection<SCIMRepresentationAttribute> ExtractRepresentationAttributesFromJSON(
            ICollection<SCIMSchema> schemas,
            ICollection<SCIMSchemaAttribute> schemaAttributes,
            object obj,
            bool ignoreUnsupportedCanonicalValues)
        {
            var result = new List<SCIMRepresentationAttribute>();
            var jArr = obj as JArray;
            var jObj = obj as JObject;
            if (jObj != null && schemaAttributes != null &&
                schemaAttributes.Any() &&
                schemaAttributes.Count() == 1 &&
                schemaAttributes.First().Type == SCIMSchemaAttributeTypes.COMPLEX)
            {
                jArr = new JArray();
                jArr.Add(jObj);
            }

            var mainSchema = schemas.First(s => s.IsRootSchema);
            var extensionSchemas = schemas.Where(s => !s.IsRootSchema).ToList();
            if (jArr != null)
            {
                var schemaAttr = schemaAttributes.First();
                var schema = schemas.FirstOrDefault(s => s.HasAttribute(schemaAttr));
                result.AddRange(SCIMRepresentationHelper.BuildAttributes(jArr, schemaAttr, schema, ignoreUnsupportedCanonicalValues));
            }
            else if (jObj != null)
            {
                var resolutionResult = SCIMRepresentationHelper.Resolve(jObj, mainSchema, extensionSchemas);
                result.AddRange(SCIMRepresentationHelper.BuildRepresentationAttributes(resolutionResult, resolutionResult.AllSchemaAttributes, ignoreUnsupportedCanonicalValues, true));
            }
            else if (schemaAttributes.Any() && schemaAttributes.Count() == 1)
            {
                var schemaAttribute = schemaAttributes.First();
                switch (schemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        var valueBoolean = false;
                        if (obj == null) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidBoolean, schemaAttribute.FullPath));
                        if (!bool.TryParse(obj.ToString(), out valueBoolean)) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidBoolean, schemaAttribute.FullPath));
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, schemaAttribute.SchemaId, valueBoolean: valueBoolean));
                        break;
                    case SCIMSchemaAttributeTypes.STRING:
                        if (obj == null) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidString, schemaAttribute.FullPath));
                        if (schemaAttribute.Required && string.IsNullOrWhiteSpace(obj.ToString())) throw new SCIMSchemaViolatedException(string.Format(Global.RequiredAttributesAreMissing, schemaAttribute.FullPath));
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, schemaAttribute.SchemaId, valueString: obj.ToString()));
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        int valueInteger;
                        if (obj == null) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidInteger, schemaAttribute.FullPath));
                        if (!int.TryParse(obj.ToString(), out valueInteger)) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidInteger, schemaAttribute.FullPath));
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, schemaAttribute.SchemaId, valueInteger: valueInteger));
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        DateTime dt;
                        if (obj == null) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidDateTime, schemaAttribute.FullPath));
                        if (!DateTime.TryParse(obj.ToString(), out dt)) throw new SCIMSchemaViolatedException(string.Format(Global.NotValidDateTime, schemaAttribute.FullPath));
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, schemaAttribute.SchemaId, valueDateTime: dt));
                        break;
                    case SCIMSchemaAttributeTypes.COMPLEX:
                        throw new SCIMSchemaViolatedException(string.Format(Global.NotValidJSON, schemaAttribute.FullPath));
                }
            }

            return result;
        }

        public static void EnrichResponse(IEnumerable<EnrichParameter> attributes, JObject jObj, bool mergeExtensionAttributes = false, bool isGetRequest = false)
        {
            foreach (var kvp in attributes.OrderBy(at => at.Order).GroupBy(a => a.AttributeNode.SchemaAttribute.Id))
            {
                var record = jObj;
                var records = kvp.ToList();
                var firstRecord = records.First();
                if (!firstRecord.AttributeNode.IsReadable(isGetRequest))
                {
                    continue;
                }

                var attributeName = firstRecord.AttributeNode.SchemaAttribute.Name;
                if (firstRecord.Schema != null && !firstRecord.Schema.IsRootSchema && 
                    ((mergeExtensionAttributes && jObj.ContainsKey(attributeName)) || (!mergeExtensionAttributes && string.IsNullOrWhiteSpace(firstRecord.AttributeNode.ParentAttributeId)))
                )
                {
                    if (jObj.ContainsKey(firstRecord.Schema.Id))
                    {
                        record = jObj[firstRecord.Schema.Id] as JObject;
                    }
                    else
                    {
                        record = new JObject();
                        jObj.Add(firstRecord.Schema.Id, record);
                    }
                }

                switch (firstRecord.AttributeNode.SchemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.STRING:
                        var valuesStr = records.Select(r => r.AttributeNode.ValueString).Where(r => r != null);
                        if (valuesStr.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesStr) : valuesStr.First());
                        break;
                    case SCIMSchemaAttributeTypes.REFERENCE:
                        var valuesRef = records.Select(r => r.AttributeNode.ValueReference).Where(r => r != null);
                        if (valuesRef.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesRef) : valuesRef.First());
                        break;
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        var valuesBoolean = records.Select(r => r.AttributeNode.ValueBoolean).Where(r => r != null);
                        if (valuesBoolean.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesBoolean) : valuesBoolean.First());
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        var valuesInteger = records.Select(r => r.AttributeNode.ValueInteger).Where(r => r != null);
                        if (valuesInteger.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesInteger) : valuesInteger.First());
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        var valuesDateTime = records.Select(r => r.AttributeNode.ValueDateTime).Where(r => r != null);
                        if (valuesDateTime.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesDateTime) : valuesDateTime.First());
                        break;
                    case SCIMSchemaAttributeTypes.COMPLEX:
                        if (firstRecord.AttributeNode.SchemaAttribute.MultiValued == false)
                        {
                            var jObjVal = new JObject();
                            var children = firstRecord.AttributeNode.CachedChildren;
                            EnrichResponse(children.Select(v => new EnrichParameter(firstRecord.Schema, 0, v)), jObjVal, mergeExtensionAttributes, isGetRequest);
                            if (jObjVal.Children().Any())
                            {
                                record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, jObjVal);
                            }
                        }
                        else
                        {
                            var jArr = new JArray();
                            foreach (var attr in records)
                            {
                                var jObjVal = new JObject();
                                var children = attr.AttributeNode.CachedChildren;
                                EnrichResponse(children.Select(v => new EnrichParameter(firstRecord.Schema, 0, v)), jObjVal, mergeExtensionAttributes, isGetRequest);
                                if (jObjVal.Children().Any())
                                {
                                    jArr.Add(jObjVal);
                                }
                            }

                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, jArr);
                        }
                        break;
                    case SCIMSchemaAttributeTypes.DECIMAL:
                        var valuesDecimal = records.Select(r => r.AttributeNode.ValueDecimal).Where(r => r != null);
                        if (valuesDecimal.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesDecimal) : valuesDecimal.First());
                        break;
                    case SCIMSchemaAttributeTypes.BINARY:
                        var valuesBinary = records.Select(r => r.AttributeNode.ValueBinary).Where(r => r != null);
                        if (valuesBinary.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesBinary) : valuesBinary.First());
                        break;
                }
            }
        }

        public class EnrichParameter
        {
            public EnrichParameter(SCIMSchema schema, int order, SCIMRepresentationAttribute attributeNode)
            {
                Schema = schema;
                Order = order;
                AttributeNode = attributeNode;
            }

            public SCIMSchema Schema { get; set; }
            public int Order { get; set; }
            public SCIMRepresentationAttribute AttributeNode { get; set; }
        }
    }
}
