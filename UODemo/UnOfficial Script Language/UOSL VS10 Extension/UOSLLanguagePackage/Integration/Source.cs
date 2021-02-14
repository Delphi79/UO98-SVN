/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.Collections.Generic;
using JoinUO.UOSL.Service;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace JoinUO.UOSL.Package
{
    public class Source : Microsoft.VisualStudio.Package.Source
    {
        public Source(LanguageService service, IVsTextLines textLines, Colorizer colorizer)
            : base(service, textLines, colorizer)
        {
        }

        public LanguageOption option { get; set; }

        private object parseResult;
        public object ParseResult
        {
            get { return parseResult; }
            set { parseResult = value; }
        }

        private IList<TextSpan[]> braces;
        public IList<TextSpan[]> Braces
        {
            get { return braces; }
            set { braces = value; }
        }
    }
}