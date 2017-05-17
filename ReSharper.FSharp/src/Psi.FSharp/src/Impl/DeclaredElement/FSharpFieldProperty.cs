﻿using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.FSharp.Impl.Tree;
using JetBrains.ReSharper.Psi.FSharp.Tree;
using JetBrains.ReSharper.Psi.FSharp.Util;
using JetBrains.ReSharper.Psi.Impl.Special;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using Microsoft.FSharp.Compiler.SourceCodeServices;

namespace JetBrains.ReSharper.Psi.FSharp.Impl.DeclaredElement
{
  /// <summary>
  /// Field in a record or in a discriminated union. Compiled as property.
  /// </summary>
  internal class FSharpFieldProperty : FSharpTypeMember<FieldDeclaration>, IProperty
  {
    internal FSharpFieldProperty([NotNull] IFieldDeclaration declaration, FSharpSymbol symbol)
      : base(declaration)
    {
      IsVisibleFromFSharp = declaration.Identifier?.IdentifierToken != null;

      var field = symbol as FSharpField;
      if (field != null)
      {
        IsWritable = field.IsMutable;
        ReturnType = FSharpTypesUtil.GetType(field.FieldType, declaration, Module) ??
                     TypeFactory.CreateUnknownType(Module);
        ShortName = field.Name;
        IsStatic = false;
        return;
      }

      var unionCase = symbol as FSharpUnionCase;
      if (unionCase != null)
      {
        IsWritable = false;
        var containingType = declaration.GetContainingTypeDeclaration()?.DeclaredElement;
        ReturnType = containingType != null
          ? TypeFactory.CreateType(containingType)
          : TypeFactory.CreateUnknownType(Module);
        ShortName = unionCase.Name;
        IsStatic = true;
        return;
      }

      IsWritable = false;
      ReturnType = TypeFactory.CreateUnknownType(Module);
      ShortName = declaration.ShortName;

    }

    public override string ShortName { get; }

    public override DeclaredElementType GetElementType()
    {
      return CLRDeclaredElementType.PROPERTY;
    }

    public bool IsExplicitImplementation => false;
    public IList<IExplicitImplementation> ExplicitImplementations => EmptyList<IExplicitImplementation>.Instance;
    public bool CanBeImplicitImplementation => false;

    public InvocableSignature GetSignature(ISubstitution substitution)
    {
      return new InvocableSignature(this, substitution);
    }

    public IEnumerable<IParametersOwnerDeclaration> GetParametersOwnerDeclarations()
    {
      return EmptyList<IParametersOwnerDeclaration>.Instance;
    }

    public IList<IParameter> Parameters => EmptyList<IParameter>.Instance;
    public IType ReturnType { get; }
    public bool IsRefReturn => false;
    public IType Type => ReturnType;

    public string GetDefaultPropertyMetadataName()
    {
      return ShortName;
    }

    public bool IsReadable => true;
    public bool IsWritable { get; }
    public IAccessor Getter => new ImplicitAccessor(this, AccessorKind.GETTER);
    public IAccessor Setter => IsWritable ? new ImplicitAccessor(this, AccessorKind.SETTER) : null;

    public bool IsAuto => false;
    public bool IsDefault => false;
    public override bool IsStatic { get; }
    public override bool IsVisibleFromFSharp { get; }
  }
}