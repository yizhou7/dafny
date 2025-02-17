﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.Dafny.LanguageServer.Language.Symbols {
  public class MethodSymbol : MemberSymbol, ILocalizableSymbol {
    /// <summary>
    /// Gets the method node representing the declaration of this symbol.
    /// </summary>
    public Method Declaration { get; }
    public object Node => Declaration;

    /// <summary>
    /// Gets the method parameters.
    /// </summary>
    public IList<VariableSymbol> Parameters { get; } = new List<VariableSymbol>();

    /// <summary>
    /// Gets the return values.
    /// </summary>
    public IList<VariableSymbol> Returns { get; } = new List<VariableSymbol>();

    /// <summary>
    /// Gets the block
    /// </summary>
    public ScopeSymbol? Block { get; set; }

    private IEnumerable<ISymbol> BlockAsEnumerable => Block != null ? new[] { Block } : Enumerable.Empty<ISymbol>();

    public override IEnumerable<ISymbol> Children => BlockAsEnumerable.Concat(Parameters).Concat(Returns);

    public MethodSymbol(ISymbol? scope, Method method) : base(scope, method) {
      Declaration = method;
    }

    public string GetDetailText(CancellationToken cancellationToken) {
      return $"{Declaration.WhatKind} {ClassPrefix}{Declaration.Name}({Declaration.Ins.AsCommaSeperatedText()}) returns ({Declaration.Outs.AsCommaSeperatedText()})";
    }

    public override TResult Accept<TResult>(ISymbolVisitor<TResult> visitor) {
      return visitor.Visit(this);
    }
  }
}
