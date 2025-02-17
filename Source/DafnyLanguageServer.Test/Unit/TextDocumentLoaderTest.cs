﻿using Microsoft.Dafny.LanguageServer.Language;
using Microsoft.Dafny.LanguageServer.Language.Symbols;
using Microsoft.Dafny.LanguageServer.Workspace;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Dafny.LanguageServer.IntegrationTest.Unit {
  [TestClass]
  public class TextDocumentLoaderTest {
    private Mock<IDafnyParser> parser;
    private Mock<ISymbolResolver> symbolResolver;
    private Mock<IProgramVerifier> verifier;
    private Mock<ISymbolTableFactory> symbolTableFactory;
    private Mock<ICompilationStatusNotificationPublisher> notificationPublisher;
    private TextDocumentLoader textDocumentLoader;

    [TestInitialize]
    public void SetUp() {
      parser = new();
      symbolResolver = new();
      verifier = new();
      symbolTableFactory = new();
      notificationPublisher = new();
      textDocumentLoader = TextDocumentLoader.Create(
        parser.Object,
        symbolResolver.Object,
        verifier.Object,
        symbolTableFactory.Object,
        notificationPublisher.Object
      );
    }

    private static TextDocumentItem CreateTestDocument() {
      return new TextDocumentItem {
        LanguageId = "dafny",
        Version = 1,
        Text = ""
      };
    }

    [TestMethod]
    public async Task LoadReturnsCanceledTaskIfOperationIsCanceled() {
      parser.Setup(p => p.Parse(It.IsAny<TextDocumentItem>(), It.IsAny<ErrorReporter>(), It.IsAny<CancellationToken>()))
        .Throws<OperationCanceledException>();
      var task = textDocumentLoader.LoadAsync(CreateTestDocument(), true, default);
      try {
        await task;
        Assert.Fail("document load was not cancelled");
      } catch (Exception e) {
        Assert.IsInstanceOfType(e, typeof(OperationCanceledException));
        Assert.IsTrue(task.IsCanceled);
        Assert.IsFalse(task.IsFaulted);
      }
    }

    [TestMethod]
    public async Task LoadReturnsFaultedTaskIfAnyExceptionOccured() {
      parser.Setup(p => p.Parse(It.IsAny<TextDocumentItem>(), It.IsAny<ErrorReporter>(), It.IsAny<CancellationToken>()))
        .Throws<InvalidOperationException>();
      var task = textDocumentLoader.LoadAsync(CreateTestDocument(), true, default);
      try {
        await task;
        Assert.Fail("document load did not fail");
      } catch (Exception e) {
        Assert.IsNotInstanceOfType(e, typeof(OperationCanceledException));
        Assert.IsFalse(task.IsCanceled);
        Assert.IsTrue(task.IsFaulted);
      }
    }
  }
}
