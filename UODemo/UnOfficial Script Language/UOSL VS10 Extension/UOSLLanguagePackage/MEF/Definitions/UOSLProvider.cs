using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
//using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace JoinUO.UOSL.Package.MEF
{

//    #region Provider definition
//    /// <summary>
//    /// This class causes a classifier to be added to the set of classifiers.
//    /// </summary>
//    [Export(typeof(IClassifierProvider))]
//    [ContentType("UOSL")]
//    internal class UOSLProvider : IClassifierProvider
//    {
//        /// <summary>
//        /// Import the classification registry to be used for getting a reference
//        /// to the custom classification type later.
//        /// </summary>
//        [Import]
//        internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

//        public IClassifier GetClassifier(ITextBuffer buffer)
//        {
//            return buffer.Properties.GetOrCreateSingletonProperty<UOSL>(delegate { return new UOSL(ClassificationRegistry); });
//        }
//    }
//    #endregion //provider def

//    #region Classifier
//    /// <summary>
//    /// Classifier that classifies all text as an instance of the OrinaryClassifierType
//    /// </summary>
//    class UOSL : IClassifier
//    {
//        IClassificationType _classificationType;

//        internal UOSL(IClassificationTypeRegistryService registry)
//        {
//            _classificationType = registry.GetClassificationType("UOSL");
//        }

//        /// <summary>
//        /// This method scans the given SnapshotSpan for potential matches for this classification.
//        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
//        /// </summary>
//        /// <param name="trackingSpan">The span currently being classified</param>
//        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
//        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
//        {
//            //create a list to hold the results
//            List<ClassificationSpan> classifications = new List<ClassificationSpan>();
//            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start, span.Length)),
//                                                           _classificationType));
//            return classifications;
//        }

//#pragma warning disable 67
//        // This event gets raised if a non-text change would affect the classification in some way,
//        // for example typing /* would cause the classification to change in C# without directly
//        // affecting the span.
//        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
//#pragma warning restore 67
//    }
//    #endregion //Classifier
}
